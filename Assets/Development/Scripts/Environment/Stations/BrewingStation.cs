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

public class BrewingStation : BaseStation, IHasMinigameTiming
{
    public event EventHandler<IHasMinigameTiming.OnMinigameTimingEventArgs> OnMinigameTimingStarted;

    //[SerializeField] private MinigameQTE minigameQTE;
 
    [Header("Visuals")]
    [SerializeField] private ParticleSystem interactParticle;
    [SerializeField] private Transform playerLerpingPosition;
    private float playerLerpingDuration = 1.0f;

    [Header("Order")]
    private OrderInfo currentOrder;

    [Header("Ingredients")]
    [SerializeField] public List<IngredientSO> ingredientSOList = new List<IngredientSO>();
    [SerializeField] private TextMeshPro ingredientsIndicatorText;
    [SerializeField] private string currentIngredientSOList;
    [SerializeField] private List<String> validIngredientTagList = new List<String>();
    [SerializeField] private int numIngredientsNeeded = 1;

    [Header("Brewing")]
    [SerializeField] private BrewingRecipeSO brewingRecipeSO;
    public NetworkVariable<bool> availableForOrder = new NetworkVariable<bool>(true);
    private NetworkVariable<float> minigameTimer = new NetworkVariable<float>(0f);
    private NetworkVariable<bool> isMinigameRunning = new NetworkVariable<bool>(false);
    private float maxMinigameTimer = 4.0f;
    private float minSweetSpotPosition = 0.1f;
    private float maxSweetSpotPosition = 0.9f;
    private NetworkVariable<float> sweetSpotPosition = new NetworkVariable<float>();
    public NetworkVariable<bool> canEmptyBrewingStation = new NetworkVariable<bool>(true);
    [SerializeField] public float brewingStationEmptyCooldown = 10.0f;

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
    private PlayerController currentPlayerController = null;
    private PlayerController brewingPlayer;
    private float previousFrameTime;
    private readonly int BP_Barista_PickUpHash = Animator.StringToHash("BP_Barista_PickUp");
    private readonly int Barista_BrewingHash = Animator.StringToHash("Barista_Brewing");
    private const float CrossFadeDuration = 0.1f;
    private float animationWaitTime;

    private NetworkVariable<bool> isminigameEnded = new NetworkVariable<bool>(true);

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
        minigameTimer.OnValueChanged += MinigameTimer_OnValueChanged;
        CustomerBase.OnCustomerLeave += CustomerBase_OnCustomerLeave;
    }

    public override void OnNetworkDespawn()
    {
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
            Empty();

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

    private void Update()
    {
        PrintHeldIngredientList();
        if (!IsServer)
        {
            return;
        }
        if (isMinigameRunning.Value)
        {
            minigameTimer.Value += Time.deltaTime;

            if (minigameTimer.Value >= maxMinigameTimer && !isminigameEnded.Value)
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
    private void MinigameDoneServerRpc()
    {
        minigameTimer.Value = 0f;
        isMinigameRunning.Value = false;
        OnBrewingDone?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void MinigameStartedServerRpc()
    {
        animationWaitTime = 1f; //PlayerController.Instance.anim.GetCurrentAnimatorStateInfo(0).normalizedTime; this is giving a delay of like 1 sec , i believe is because i'm playimg the animation faster than original
        SpawnCoffeeDrinkServerRpc();

        Empty();

        minigameTimer.Value = 0f;
        isMinigameRunning.Value = true;
        
        sweetSpotPosition.Value = UnityEngine.Random.Range(minSweetSpotPosition, maxSweetSpotPosition);
    }

    public override void Interact(PlayerController player)
    {
        if (player.HasIngredient()) return;
        if (player.HasPickup()) return;
        //Setup brewing controller stuff
        if (currentPlayerController == null && isminigameEnded.Value && ingredientSOList.Count >= numIngredientsNeeded)
        {
            Debug.LogWarning("setting new currentPlayerController");
            currentPlayerController = player; // Reference for animations
            currentPlayerController.GetInstanceID();
            SetIsMinigameEndedServerRpc(false);
        }
        if (currentPlayerController == null) return;

        if (player.GetInstanceID() == currentPlayerController.GetInstanceID())
        {
            Debug.LogWarning("Players are matching");
            //Stop Brewing Minigame
            if (isMinigameRunning.Value)
            {
                Debug.LogWarning("Ending Brewing minigame");
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
                if (GetIngredient().GetComponent<CoffeeAttributes>() != null)
                {
                    GetIngredient().GetComponent<CoffeeAttributes>().SetIsMinigamePerfect(minigameResult);
                }

                MinigameEnded();
            }

            //Begin Brewing Minigame
            if (!isMinigameRunning.Value && ingredientSOList.Count >= numIngredientsNeeded)
            {
                Debug.LogWarning("Beginning Brewing minigame");
                MinigameStartedServerRpc();

                player.anim.CrossFadeInFixedTime(Barista_BrewingHash, CrossFadeDuration);
                StartCoroutine(LerpPlayerToLerpingPoint(playerLerpingPosition.position, playerLerpingPosition.rotation, playerLerpingDuration, player));
                player.movementToggle = false;
                InteractLogicPlaceObjectOnBrewing();
            }
        }
    }

    IEnumerator LerpPlayerToLerpingPoint(Vector3 lerpPosition, Quaternion targetRotation, float duration, PlayerController player)
    {
        float time = 0;
        Vector3 startPosition = player.transform.position;
        Vector3 targetPositionNoY = new Vector3(lerpPosition.x, startPosition.y, lerpPosition.z);
        Quaternion startRotation = player.transform.rotation;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            Vector3 lerpedPosition = Vector3.Lerp(startPosition, targetPositionNoY, t);
            player.transform.position = new Vector3(lerpedPosition.x, player.transform.position.y, lerpedPosition.z);
            player.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);

            yield return null;
        }

        // updateplayer reaches exactly the target position
        player.transform.position = new Vector3(lerpPosition.x, player.transform.position.y, lerpPosition.z);
        player.transform.rotation = targetRotation;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetIsMinigameEndedServerRpc(bool isMinigameEnded)
    {
        isminigameEnded.Value = isMinigameEnded;
    }

    public void MinigameEnded()
    { 
        if (TutorialManager.Instance != null && TutorialManager.Instance.tutorialEnabled && !TutorialManager.Instance.firstDrinkReady)
            TutorialManager.Instance.FirstDrinkReady();

        PickCupAnimationServerRpc();// plays animation and sets cup in hand (SetIngredientParent(player))
        MinigameDoneServerRpc();
        PrintHeldIngredientList();
    }

    public void InteractLogicPlaceObjectOnBrewing()
    {
        InteractLogicPlaceObjectOnBrewingServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnBrewingServerRpc()
    {
        InteractLogicPlaceObjectOnBrewingClientRpc();
    }

    [ClientRpc]
    private void InteractLogicPlaceObjectOnBrewingClientRpc()
    {
        if (ingredientSOList.Count >= numIngredientsNeeded)
        {
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

        if (isMinigameRunning.Value)
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
        EmptyClientRpc();
        TurnAllEmissiveOff();
    }

    [ClientRpc]
    private void EmptyClientRpc()
    {
        ingredientSOList.Clear();
    }

    [ServerRpc(RequireOwnership =false)]
    private void PickCupAnimationServerRpc()
    {
        PickCupAnimationClientRpc();
    }

    [ClientRpc]
    private void PickCupAnimationClientRpc()
    {
        PickCupAnimation();
    }

    private void PickCupAnimation()
    {
        StartCoroutine(ResetAnimation());
    }

    private IEnumerator ResetAnimation()
    {
        currentPlayerController.anim.CrossFadeInFixedTime(BP_Barista_PickUpHash, CrossFadeDuration);
        currentPlayerController.movementToggle = false;

        yield return new WaitForSeconds(animationWaitTime);
        GetIngredient().SetIngredientParent(currentPlayerController);
        animationSwitch?.Invoke();
        currentPlayerController.movementToggle = true;
        currentPlayerController = null;
        SetIsMinigameEndedServerRpc(true);
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

