using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.UI;
using static UnityEngine.Rendering.HableCurve;

public class BrewingStation : BaseStation, IHasProgress, IHasMinigameTiming
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<IHasMinigameTiming.OnMinigameTimingEventArgs> OnMinigameTimingStarted;

    [Header("Visuals")]
    [SerializeField] private ParticleSystem interactParticle;

    [Header("Order")]
    public Transform orderStatsRoot;
    public GameObject orderStatsPrefab;
    public OrderStats orderStats;
    public CustomerBase customer;

    [Header("Ingredients")]
    [SerializeField] public List<IngredientSO> ingredientSOList = new List<IngredientSO>();
    [SerializeField] private TextMeshPro ingredientsIndicatorText;
    [SerializeField] private string currentIngredientSOList;
    [SerializeField] private List<String> validIngredientTagList = new List<String>();
    [SerializeField] private int numIngredientsNeeded = 4;

    [Header("Brewing")]
    private NetworkVariable<float> brewingTimer = new NetworkVariable<float>(0f);
    [SerializeField] private BrewingRecipeSO brewingRecipeSO;
    private bool isBrewing;
    public bool orderAssigned;
    private float minigameTimer;
    private bool minigameTiming = false;
    private float maxMinigameTimer = 4.0f;
    private float minSweetSpotPosition = 0.1f;
    private float maxSweetSpotPosition = 0.9f;
    private float sweetSpotPosition;

    private void Start()
    {
        orderStats = Instantiate(orderStatsPrefab, orderStatsRoot).GetComponent<OrderStats>();
    }

    private void Awake()
    {
        validIngredientTagList.Add("CoffeeGrind");
        validIngredientTagList.Add("Milk");
        validIngredientTagList.Add("Sweetener");
        validIngredientTagList.Add("BioMatter");
    }

    public override void OnNetworkSpawn()
    {
        brewingTimer.OnValueChanged += BrewingTimer_OnValueChanged;
    }

    private void BrewingTimer_OnValueChanged(float previousValue, float newValue)
    {
        float brewingTimerMax = brewingRecipeSO != null ? brewingRecipeSO.brewingMax : 1f;
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = brewingTimer.Value / brewingTimerMax
        });
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }

        if (isBrewing)
        {
            brewingTimer.Value += Time.deltaTime;
            if (brewingTimer.Value >= brewingRecipeSO.brewingMax)
            {
                SpawnCoffeeDrinkServerRpc();   
                BrewingDoneServerRpc();   
            }
        }
        if (minigameTiming)
        {
            minigameTimer += Time.deltaTime;
            OnMinigameTimingStarted?.Invoke(this, new IHasMinigameTiming.OnMinigameTimingEventArgs
            {
                minigameTimingNormalized = (float)minigameTimer / maxMinigameTimer,
                sweetSpotPosition = sweetSpotPosition
            });
        }

        if (orderStats.GetOrderOwner() != null)
        {
            orderStats.OrderInProgress(true);
        }
        else if (orderStats.GetOrderOwner() == null)
        {
            orderStats.OrderInProgress(false);
        }
    }

    public void SetOrder(CustomerBase customerOrder)
    {
        orderStats.SetOrderOwner(customerOrder);  //<--- TURN THIS ON AND FIX / REMOVE WHAT BREAKS.  reveals lots of dead code
        orderStats.customerInfoRoot.SetActive(true);
        orderStats.customerNumberText.text = customerOrder.customerNumber.ToString();
        orderStats.customerNameText.text = customerOrder.customerName;
        orderStats.brewingStationText.text = this.name;
        orderStats.orderTimer.value = customerOrder.orderTimer.Value;
        orderStats.temperatureSegments.targetAttributeValue = customerOrder.coffeeAttributes.GetTemperature();
        orderStats.sweetnessSegments.targetAttributeValue = customerOrder.coffeeAttributes.GetSweetness();
        orderStats.spicinessSegments.targetAttributeValue = customerOrder.coffeeAttributes.GetSpiciness();
        orderStats.strengthSegments.targetAttributeValue = customerOrder.coffeeAttributes.GetStrength();
        orderStats.temperatureSegments.potentialIngredientValue = 0;
        orderStats.sweetnessSegments.potentialIngredientValue = 0;
        orderStats.spicinessSegments.potentialIngredientValue = 0;
        orderStats.strengthSegments.potentialIngredientValue = 0;
        orderStats.SetPotentialSweetness();
        orderStats.SetPotentialTemperature();
        orderStats.SetPotentialSpiciness();
        orderStats.SetPotentialStrength();
        orderAssigned = true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnCoffeeDrinkServerRpc()
    {
        SpawnCoffeeDrinkClientRpc();
    }

    [ClientRpc]
    private void SpawnCoffeeDrinkClientRpc()
    {
        Ingredient.SpawnIngredient(brewingRecipeSO.output, this);

        CoffeeAttributes coffeeAttributes = this.GetIngredient().GetComponent<CoffeeAttributes>();
        foreach (IngredientSO ingredient in ingredientSOList)
        {
            coffeeAttributes.AddTemperature(ingredient.temperature);
            coffeeAttributes.AddSweetness(ingredient.sweetness);
            coffeeAttributes.AddSpiciness(ingredient.spiciness);
            coffeeAttributes.AddStrength(ingredient.strength);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void BrewingDoneServerRpc()
    {
        BrewingDoneClientRpc();
    }
    
    [ClientRpc]
    private void BrewingDoneClientRpc()
    {
        ingredientSOList.Clear();
        isBrewing = false;
        orderAssigned = false;

        //setup minigame
        minigameTiming = true;
        minigameTimer = 0;
        sweetSpotPosition = UnityEngine.Random.Range(minSweetSpotPosition, maxSweetSpotPosition);
    }

    public override void Interact(PlayerController player)
    {
        if (!HasIngredient())
        {
            if (player.GetNumberOfIngredients() >= 1)
            {
                SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.interactStation);
                interactParticle.Play();

                foreach (Ingredient i in player.GetIngredientsList())
                {
                    if (TryAddIngredient(i.GetIngredientSO()))
                    {
                        AddIngredientToListSOServerRpc(BaristapocalypseMultiplayer.Instance.GetIngredientSOIndex(player.GetIngredient().GetIngredientSO()));

                        player.RemoveIngredientInListByReference(i);
                        Ingredient.DestroyIngredient(i);

                        InteractLogicPlaceObjectOnBrewingServerRpc();

                        break;
                    }
                }
            }
        }
        else
        {
            if (HasIngredient() && !minigameTiming)
            {
                GetIngredient().SetIngredientParent(player);
            }
        }

        // Start brewing for ingredients in the machine.  This is for adding directly from stations instead of player hands
        if (ingredientSOList.Count >= numIngredientsNeeded)
            InteractLogicPlaceObjectOnBrewingServerRpc();

        if (minigameTiming)
        {
            //Debug.Log("Minigame timing: " + minigameTimer / maxMinigameTimer);
            //Debug.Log("SweetSpot position: " + sweetSpotPosition);
            //Debug.Log("Timing calc: " + Mathf.Abs((minigameTimer / maxMinigameTimer) - sweetSpotPosition));
            float timingPressed = Mathf.Abs((minigameTimer / maxMinigameTimer) - sweetSpotPosition);
            bool minigameResult = false;
            if (timingPressed <= 0.1f)
            {
                minigameResult = true;
            }
            else if ((minigameTimer / maxMinigameTimer) < sweetSpotPosition)
            {
                minigameResult = false;
            }
            else if ((minigameTimer / maxMinigameTimer) > sweetSpotPosition)
            {
                minigameResult = false;
            }
            if (this.GetIngredient().GetComponent<CoffeeAttributes>() != null)
            {
                this.GetIngredient().GetComponent<CoffeeAttributes>().SetIsMinigamePerfect(minigameResult);
            }
            minigameTiming = false;
            OnMinigameTimingStarted?.Invoke(this, new IHasMinigameTiming.OnMinigameTimingEventArgs
            {
                minigameTimingNormalized = 0f,
                sweetSpotPosition = sweetSpotPosition
            });
        }
        if (minigameTimer >= maxMinigameTimer)
        {
            minigameTiming = false;
        }
        PrintHeldIngredientList();
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnBrewingServerRpc()
    {
        brewingTimer.Value = 0f;
        InteractLogicPlaceObjectOnBrewingClientRpc();
    }

    [ClientRpc]
    private void InteractLogicPlaceObjectOnBrewingClientRpc()
    {
        if (ingredientSOList.Count >= numIngredientsNeeded)
        {
            isBrewing = true;
            ingredientsIndicatorText.SetText("");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddIngredientToListSOServerRpc(int ingredientSOIndex)
    {
        AddIngredientToListSOClientRpc(ingredientSOIndex);
    }

    [ClientRpc]
    private void AddIngredientToListSOClientRpc(int ingredientSOIndex)
    {
        IngredientSO ingredientSO = BaristapocalypseMultiplayer.Instance.GetIngredientSOFromIndex(ingredientSOIndex);
        ingredientSOList.Add(ingredientSO);

        orderStats.temperatureSegments.cumulativeIngredientsValue += ingredientSO.temperature;
        orderStats.sweetnessSegments.cumulativeIngredientsValue += ingredientSO.sweetness;
        orderStats.spicinessSegments.cumulativeIngredientsValue += ingredientSO.spiciness;
        orderStats.strengthSegments.cumulativeIngredientsValue += ingredientSO.strength;
    }

    public bool TryAddIngredient(IngredientSO ingredientSO)
    {
        if (!ValidIngredient(ingredientSO.objectTag))
        {
            return false;
        }

        foreach (IngredientSO ingredient in ingredientSOList)
        {
            //currentIngredientSOList += ingredient.name + "\n";
            if (ingredient.objectTag == ingredientSO.objectTag)
            {
                return false;
            }
        }
        return true;
    }

    private bool ValidIngredient(String ingredientTag)
    {
        if (validIngredientTagList.Contains(ingredientTag))
        {
            return true;
        }
        return false;
    }

    private void PrintHeldIngredientList()
    {
        PrintHeldIngredientListServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void PrintHeldIngredientListServerRpc()
    {
        PrintHeldIngredientListClientRpc();
    }

    [ClientRpc]
    private void PrintHeldIngredientListClientRpc()
    {
        currentIngredientSOList = null;
        foreach (IngredientSO ingredient in ingredientSOList)
        {
            currentIngredientSOList += ingredient.name + "\n";
        }
        ingredientsIndicatorText.text = currentIngredientSOList;
    }
}

