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
    private OrderInfo currentOrder;

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
    public NetworkVariable<bool> availableForOrder = new NetworkVariable<bool>(true);
    private NetworkVariable<float> minigameTimer = new NetworkVariable<float>(0f);
    private bool minigameTiming = false;
    private float maxMinigameTimer = 4.0f;
    private float minSweetSpotPosition = 0.1f;
    private float maxSweetSpotPosition = 0.9f;
    private NetworkVariable<float> sweetSpotPosition = new NetworkVariable<float>();

    public delegate void OnBrewingDoneHandler(object sender, EventArgs e);
    public event OnBrewingDoneHandler OnBrewingDone;

    public delegate void OnBrewingEmptyHandler(object sender, EventArgs e);
    public event OnBrewingEmptyHandler OnBrewingEmpty;


    protected virtual void RaiseBrewingDone()
    {
        currentOrder.SetOrderState(OrderState.BeingDelivered);
        OnBrewingDone?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void RaiseBrewingEmpty()
    {
        OnBrewingEmpty?.Invoke(this, EventArgs.Empty);
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
        minigameTimer.OnValueChanged += MinigameTimer_OnValueChanged;
    }

    private void MinigameTimer_OnValueChanged(float previousValue, float newValue)
    {
        OnMinigameTimingStarted?.Invoke(this, new IHasMinigameTiming.OnMinigameTimingEventArgs
        {
            minigameTimingNormalized = (float)minigameTimer.Value / maxMinigameTimer,
            sweetSpotPosition = sweetSpotPosition.Value
        });
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
        if (minigameTiming)
        {
            minigameTimer.Value += Time.deltaTime;
        }
    }

    public void SetOrder(OrderInfo order)
    {
        currentOrder = order;
        availableForOrder.Value = false;
        order.SetOrderState(OrderState.Brewing);
    }

    [ServerRpc]
    private void SpawnCoffeeDrinkServerRpc()
    {
        Ingredient.SpawnIngredient(brewingRecipeSO.output, this);
        SpawnCoffeeDrinkClientRpc();
    }

    [ClientRpc]
    private void SpawnCoffeeDrinkClientRpc()
    {
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
        sweetSpotPosition.Value = UnityEngine.Random.Range(minSweetSpotPosition, maxSweetSpotPosition);
        BrewingDoneClientRpc();
        RaiseBrewingDone();
    }

    [ClientRpc]
    private void BrewingDoneClientRpc()
    {
        ingredientSOList.Clear();
        isBrewing = false;
        availableForOrder.Value = true;

        //setup minigame
        minigameTiming = true;
        minigameTimer.Value = 0f;
    }

    [ServerRpc(RequireOwnership = false)]
    private void MinigameDoneServerRpc()
    {
        minigameTimer.Value = 0f;
        MinigameDoneClientRpc();
    }

    [ClientRpc]
    private void MinigameDoneClientRpc()
    {
        minigameTiming = false;
    }

    public override void Interact(PlayerController player)
    {
        if (!player.IsLocalPlayer)
        {
            Debug.LogWarning("me local player");
            return;
        }
        // Start brewing for ingredients in the machine.  This is for adding directly from stations instead of player hands
        if (ingredientSOList.Count >= numIngredientsNeeded)
        {
            InteractLogicPlaceObjectOnBrewing();
        }

        if (minigameTiming)
        {
            float timingPressed = Mathf.Abs((minigameTimer.Value/ maxMinigameTimer) - sweetSpotPosition.Value);
            bool minigameResult = false;
            if (timingPressed <= 0.1f)
            {
                minigameResult = true;
            }
            else if ((minigameTimer.Value / maxMinigameTimer) < sweetSpotPosition.Value)
            {
                minigameResult = false;
            }
            else if ((minigameTimer.Value / maxMinigameTimer) > sweetSpotPosition.Value)
            {
                minigameResult = false;
            }
            if (this.GetIngredient().GetComponent<CoffeeAttributes>() != null)
            {
                this.GetIngredient().GetComponent<CoffeeAttributes>().SetIsMinigamePerfect(minigameResult);
            }
            MinigameDoneServerRpc();

            if (TutorialManager.Instance != null && TutorialManager.Instance.tutorialEnabled && !TutorialManager.Instance.firstDrinkReady)
                TutorialManager.Instance.FirstDrinkReady();

            GetIngredient().SetIngredientParent(player);
        }
        if (minigameTimer.Value >= maxMinigameTimer)
        {
            MinigameDoneServerRpc();
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

