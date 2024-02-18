using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.UI;
using static UnityEngine.Rendering.HableCurve;
using Unity.VisualScripting;

public class BrewingStation : BaseStation, IHasProgress, IHasMinigameTiming
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<IHasMinigameTiming.OnMinigameTimingEventArgs> OnMinigameTimingStarted;

    [Header("Visuals")]
    [SerializeField] private ParticleSystem interactParticle;

    [Header("Order")]
    private Order currentOrder;

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
    public bool availableForOrder = true;
    private float minigameTimer;
    private bool minigameTiming = false;
    private float maxMinigameTimer = 4.0f;
    private float minSweetSpotPosition = 0.1f;
    private float maxSweetSpotPosition = 0.9f;
    private float sweetSpotPosition;

    public delegate void OnBrewingDoneHandler(object sender, EventArgs e);
    public event OnBrewingDoneHandler OnBrewingDone;

    public delegate void OnBrewingEmptyHandler(object sender, EventArgs e);
    public event OnBrewingEmptyHandler OnBrewingEmpty;


    protected virtual void RaiseBrewingDone()
    {
        currentOrder.State = Order.OrderState.Finished;
    }

    protected virtual void RaiseBrewingEmpty()
    {
        OnBrewingEmpty?.Invoke(this, EventArgs.Empty);
    }
    
    protected virtual void CustomerOrderTimeExpired()
    {
        currentOrder.State = Order.OrderState.OutOfTime;
        OnBrewingDone?.Invoke(this, EventArgs.Empty);

        resetStation();
        OrderManager.Instance.FinishOrder(currentOrder);
    }

    //private void OnEnable()
    //{
    //    OrderManager.Instance.OnOrderUpdated += ProcessOrder;
    //}

    //private void OnDisable()
    //{
    //    OrderManager.Instance.OnOrderUpdated -= ProcessOrder;
    //}

    //private void ProcessOrder(Order order)
    //{
    //    SetOrder(order);
    //}

    private void Start()
    {
        RaiseBrewingEmpty();
        Empty();
    }

    private void resetStation()
    {
        isBrewing = false;
        availableForOrder = true;
        ingredientSOList.Clear();
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

        PrintHeldIngredientList(); // putting this here until we have a way to show activity in the station

        if (!IsServer)
        {
            return;
        }

        if (isBrewing)
        {
            brewingTimer.Value += Time.deltaTime;
            if (brewingTimer.Value >= 0)
            {
                SpawnCoffeeDrinkServerRpc();   
                BrewingDoneServerRpc();
            }
        }

        if (!availableForOrder)
        {
            if (currentOrder.customer.GetCustomerState() == CustomerBase.CustomerState.Leaving)
            {
                Debug.Log("Wokin");
                CustomerOrderTimeExpiredRpc();
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

    public void SetOrder(Order order)
    {
        currentOrder = order;
        availableForOrder = false;
        order.State = Order.OrderState.Brewing;
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
        RaiseBrewingDone();
        ingredientSOList.Clear();
        isBrewing = false;

        //setup minigame
        minigameTiming = true;
        minigameTimer = 0;
        sweetSpotPosition = UnityEngine.Random.Range(minSweetSpotPosition, maxSweetSpotPosition);
    }

    private void CustomerOrderTimeExpiredRpc()
    {
        CustomerOrderTimeExpired();
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
            GetIngredient().SetIngredientParent(player);
        }
        if (minigameTimer >= maxMinigameTimer)
        {
            minigameTiming = false;
            GetIngredient().SetIngredientParent(player);
        }
        PrintHeldIngredientList();
    }

    public void InteractLogicPlaceObjectOnBrewing()
    {
        InteractLogicPlaceObjectOnBrewingServerRpc();
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

    public void AddIngredientToListSO(int ingredientSOIndex)
    {
        AddIngredientToListSOServerRpc(ingredientSOIndex);
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

    public bool TryAddIngredient(IngredientSO ingredientSO)
    {
        if (!ValidIngredient(ingredientSO.objectTag))
        {
            return false;
        }

        foreach (IngredientSO ingredient in ingredientSOList)
        {
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

    public void Empty()
    {
        ingredientSOList.Clear();
    }
}

