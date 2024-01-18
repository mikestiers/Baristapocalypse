using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class BrewingStation : BaseStation, IHasProgress, IHasMinigameTiming
{
    public Transform orderStatsRoot;
    
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<IHasMinigameTiming.OnMinigameTimingEventArgs> OnMinigameTimingStarted;

    [SerializeField] private List<IngredientSO> ingredientSOList = new List<IngredientSO>();
    [SerializeField] private ParticleSystem interactParticle;
    [SerializeField] private TextMeshPro ingredientsIndicatorText;
    private string currentIngredientSOList;
    private List<String> validIngredientTagList = new List<String>();

    private int numIngredientsNeeded = 4;

    private NetworkVariable<float> brewingTimer = new NetworkVariable<float>(0f);
    [SerializeField] private BrewingRecipeSO brewingRecipeSO;
    private bool brewing;

    private float minigameTimer;
    private bool minigameTiming = false;
    private float maxMinigameTimer = 4.0f;
    private float minSweetSpotPosition = 0.1f;
    private float maxSweetSpotPosition = 0.9f;
    private float sweetSpotPosition;

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

        if (brewing)
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
        brewing = false;

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

                        if (orderStatsRoot.childCount > 0)
                        {
                            OrderStats orderStats = orderStatsRoot.GetChild(0).GetComponent<OrderStats>();
                            
                            orderStats.temperatureSegments.cumulativeIngredientsValue += ingredient.IngredientSO.temperature;
                            orderStats.sweetnessSegments.cumulativeIngredientsValue += ingredient.IngredientSO.sweetness;
                            orderStats.spicinessSegments.cumulativeIngredientsValue += ingredient.IngredientSO.spiciness;
                            orderStats.strengthSegments.cumulativeIngredientsValue += ingredient.IngredientSO.strength;

                            orderStats.temperatureSegments.potentialIngredientValue = 0;
                            orderStats.sweetnessSegments.potentialIngredientValue = 0;
                            orderStats.spicinessSegments.potentialIngredientValue = 0;
                            orderStats.strengthSegments.potentialIngredientValue = 0;
                        }

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
            Debug.Log("Too late");
        }
        Debug.Log("ingredientSOList.Count :" + ingredientSOList.Count);
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
            brewing = true;
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
    }

    private bool TryAddIngredient(IngredientSO ingredientSO)
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

