using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.UI;
using static UnityEngine.Rendering.HableCurve;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;
using Unity.Services.Lobbies.Models;

public class BrewingStation : BaseStation, IHasProgress, IHasMinigameTiming
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<IHasMinigameTiming.OnMinigameTimingEventArgs> OnMinigameTimingStarted;

    //[SerializeField] private MinigameQTE minigameQTE;
 
    [Header("Visuals")]
    [SerializeField] private ParticleSystem interactParticle;

    [Header("Order")]
    private OrderInfo currentOrder;

    [Header("Ingredients")]
    [SerializeField] public List<IngredientSO> ingredientSOList = new List<IngredientSO>();
    [SerializeField] private TextMeshPro ingredientsIndicatorText;
    [SerializeField] private string currentIngredientSOList;
    [SerializeField] private List<String> validIngredientTagList = new List<String>();
    [SerializeField] private int numIngredientsNeeded = 1;

    [Header("Brewing")]
    private NetworkVariable<float> brewingTimer = new NetworkVariable<float>(0f);
    [SerializeField] private BrewingRecipeSO brewingRecipeSO;
    private bool isBrewing;
    public NetworkVariable<bool> availableForOrder = new NetworkVariable<bool>(true);
    private NetworkVariable<float> minigameTimer = new NetworkVariable<float>(0f);
    private NetworkVariable<bool> minigameTiming = new NetworkVariable<bool>(false);
    private float maxMinigameTimer = 4.0f;
    private float minSweetSpotPosition = 0.1f;
    private float maxSweetSpotPosition = 0.9f;
    private NetworkVariable<float> sweetSpotPosition = new NetworkVariable<float>();

    [Header("Emissions")]
    [SerializeField] private EmissiveControl[] bioMatterTubing;
    [SerializeField] private EmissiveControl bioMatterFloorPlate;
    [SerializeField] private EmissiveControl[] liquidTubing;
    [SerializeField] private EmissiveControl liquidFloorPlate;
    [SerializeField] private EmissiveControl[] coffeeBeanTubing;
    [SerializeField] private EmissiveControl coffeeBeanFloorPlate;
    [SerializeField] private EmissiveControl[] sweetenerTubing;
    [SerializeField] private EmissiveControl sweetenerFloorPlate;

    public delegate void OnBrewingDoneHandler(object sender, EventArgs e);
    public event OnBrewingDoneHandler OnBrewingDone;

    public delegate void OnBrewingEmptyHandler(object sender, EventArgs e);
    public event OnBrewingEmptyHandler OnBrewingEmpty;

    // Animation interaction with brewing machine
    public event Action animationSwitch;//*******************************
    private PlayerController playerController;
    private float previousFrameTime;
    private readonly int BP_Barista_PickUpHash = Animator.StringToHash("BP_Barista_PickUp");
    private readonly int Barista_BrewingHash = Animator.StringToHash("Barista_Brewing");
    private const float CrossFadeDuration = 0.1f;
    private float animationWaitTime;

    private void Start()
    {
        TurnAllEmissiveOff();
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
        CustomerBase.OnCustomerLeave += CustomerBase_OnCustomerLeave;
    }

    public override void OnNetworkDespawn()
    {
        brewingTimer.OnValueChanged -= BrewingTimer_OnValueChanged;
        minigameTimer.OnValueChanged -= MinigameTimer_OnValueChanged;
        CustomerBase.OnCustomerLeave -= CustomerBase_OnCustomerLeave;
    }

    protected virtual void RaiseBrewingEmpty()
    {
        OnBrewingEmpty?.Invoke(this, EventArgs.Empty);
    }

    private void CustomerBase_OnCustomerLeave(int customerIndex)
    {
        if (currentOrder == null) return;
        if (currentOrder.number == customerIndex)
        {
            sweetSpotPosition.Value = UnityEngine.Random.Range(minSweetSpotPosition, maxSweetSpotPosition);
            availableForOrder.Value = true;
            ingredientSOList.Clear();
            isBrewing = false;

            currentOrder.SetOrderState(OrderState.BeingDelivered);

            OrderManager.Instance.FinishOrder(currentOrder);
        }
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
                animationWaitTime = 1.2f; //PlayerController.Instance.anim.GetCurrentAnimatorStateInfo(0).normalizedTime; this is giving a delay of like 1 sec , i believe is because i'm playimg the animation faster than original
                BrewingDoneServerRpc();
            }
        }
        if (minigameTiming.Value)
        {
            minigameTimer.Value += Time.deltaTime;

            if (minigameTimer.Value >= maxMinigameTimer)
            {
                MinigameEnded();
            }
        }
    }

    public void SetOrder(OrderInfo order)
    {
        SetOrderServerRpc(order);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetOrderServerRpc(OrderInfo order)
    {
        availableForOrder.Value = false;
        SetOrderClientRpc(order);
    }

    [ClientRpc]
    private void SetOrderClientRpc(OrderInfo order)
    {
        currentOrder = order;
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

    }

    [ClientRpc]
    private void BrewingDoneClientRpc()
    {
        ingredientSOList.Clear();
        isBrewing = false;
        TurnAllEmissiveOff();

        //minigameQTE.StartMinigame();
        minigameTimer.Value = 0f;
        minigameTiming.Value = true;
        
    }

    [ServerRpc(RequireOwnership = false)]
    private void MinigameDoneServerRpc()
    {
        minigameTimer.Value = 0f;
        minigameTiming.Value = false;
    }

    public override void Interact(PlayerController player)
    {
        if (!player.IsLocalPlayer)
        {
            Debug.LogWarning("me local player");
            return;
        }
    
        playerController = player; // Reference for animations
        // Start brewing for ingredients in the machine.  This is for adding directly from stations instead of player hands
        if (ingredientSOList.Count >= numIngredientsNeeded)
        {
            player.anim.CrossFadeInFixedTime(Barista_BrewingHash, CrossFadeDuration);
            player.movementToggle = false;
            InteractLogicPlaceObjectOnBrewing();
        }

        if (minigameTiming.Value)
        {
            float timingPressed = Mathf.Abs((minigameTimer.Value / maxMinigameTimer) - sweetSpotPosition.Value);
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

            MinigameEnded();
        }
        
        PrintHeldIngredientList();
    }


    public void MinigameEnded()
    {
        if (TutorialManager.Instance != null && TutorialManager.Instance.tutorialEnabled && !TutorialManager.Instance.firstDrinkReady)
            TutorialManager.Instance.FirstDrinkReady();

        MinigameDoneServerRpc();
        PrintHeldIngredientList();
        PickCupAnimation(playerController);// plays animation and sets cup in hand (SetIngredientParent(player))
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
        TurnOnEmissive(ingredientSO);
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

        if(isBrewing || minigameTiming.Value)
        {
            return false;
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
        TurnAllEmissiveOff();
    }

    private void PickCupAnimation(PlayerController player)
    {
        StartCoroutine(ResetAnimation(player));
    }

    private IEnumerator ResetAnimation(PlayerController player)
    {
        player.anim.CrossFadeInFixedTime(BP_Barista_PickUpHash, CrossFadeDuration);
        player.movementToggle = false;

        yield return new WaitForSeconds(animationWaitTime);
        player.movementToggle = true;
        GetIngredient().SetIngredientParent(player);
        animationSwitch?.Invoke();
    }

    private void TurnAllEmissiveOff()
    {
        TurnAllEmissiveOffClientRpc();
    }

    [ClientRpc]
    private void TurnAllEmissiveOffClientRpc()
    {
        for (int i = 0; i < bioMatterTubing.Length; i++)
        {
            bioMatterTubing[i].SetEmissive(false);
        }
        for (int i = 0; i < liquidTubing.Length; i++)
        {
            liquidTubing[i].SetEmissive(false);
        }
        for (int i = 0; i < coffeeBeanTubing.Length; i++)
        {
            coffeeBeanTubing[i].SetEmissive(false);
        }
        for (int i = 0; i < sweetenerTubing.Length; i++)
        {
            sweetenerTubing[i].SetEmissive(false);
        }

        bioMatterFloorPlate.SetEmissive(false);
        liquidFloorPlate.SetEmissive(false);
        coffeeBeanFloorPlate.SetEmissive(false);
        sweetenerFloorPlate.SetEmissive(false);
    }

    private void TurnOnEmissive(IngredientSO ingredientSO)
    {
        TurnOnEmissiveClientRpc(BaristapocalypseMultiplayer.Instance.GetIngredientSOIndex(ingredientSO));
    }

    [ClientRpc]
    private void TurnOnEmissiveClientRpc(int ingredientSOIndex)
    {
        IngredientSO ingredientSO = BaristapocalypseMultiplayer.Instance.GetIngredientSOFromIndex(ingredientSOIndex);
        switch (ingredientSO.objectTag)
        {
            case "Sweetener":
                for (int i = 0; i < sweetenerTubing.Length; i++)
                {
                    sweetenerTubing[i].SetEmissive(true);
                }
                sweetenerFloorPlate.SetEmissive(true);
                break;
            case "Milk":
                for (int i = 0; i < liquidTubing.Length; i++)
                {
                    liquidTubing[i].SetEmissive(true);
                }
                liquidFloorPlate.SetEmissive(true);
                break;
            case "BioMatter":
                for (int i = 0; i < bioMatterTubing.Length; i++)
                {
                    bioMatterTubing[i].SetEmissive(true);
                }
                bioMatterFloorPlate.SetEmissive(true);
                break;
            case "CoffeeGrind":
                for (int i = 0; i < coffeeBeanTubing.Length; i++)
                {
                    coffeeBeanTubing[i].SetEmissive(true);
                }
                coffeeBeanFloorPlate.SetEmissive(true);
                break;
            default:
                Debug.LogWarning("Emissive tag wrong");
                break;
        }
    }
}

