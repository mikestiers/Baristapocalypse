using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static BrewingStation;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public class CustomerBase : Base
{
    [Header("Navigation")]
    public NavMeshAgent agent;
    public bool frontofLine;
    public bool inLine;
    public bool atSit = false; //for tables customer rotation orientation 
    public bool leaving = false;
    public bool makingAMess = false;
    public bool moving;
    public float distThreshold;
    public GameObject[] Line;
    public int LineIndex;
    private Vector3 exit;
    public int currentPosInLine;

    [Header("Identifiers")]
    public NetworkVariable<FixedString32Bytes> customerName = new NetworkVariable<FixedString32Bytes>();
    public NetworkVariable<int> customerNumber = new NetworkVariable<int>();
    private bool orderBeingServed;

    [Header("Coffee Attributes")]
    public CoffeeAttributes coffeeAttributes;
    private OrderInfo order;

    [Header("State Related")]
    public NetworkVariable<CustomerState> currentState = new NetworkVariable<CustomerState>(CustomerState.Init);
    public float orderTimer = -1f;
    public float? messTime = null;
    public float? lineTime = null;
    public float customerLeaveTime;
    public float maxInLineTime;
    public float deadTimerSeconds = 5.0f;
    private Coroutine randomPointCoroutine;

    [Header("Visuals")]
    [SerializeField] public Canvas customerNumberCanvas;
    [SerializeField] private Text customerNumberText;
    [SerializeField] private Text customerNameText;
    [SerializeField] private ParticleSystem interactParticle;
    [SerializeField] private DetachedHead detachedHead;
    [SerializeField] private ScoreTimerManager scoreTimerManager;

    [SerializeField] private PickupSO pickupSO;

    [Header("Customer Review")]
    public GameObject customerReviewPrefab;
    private GameObject customerReviewPanel;
    private int customerInstanceReviewScore;

    // Animation interaction with brewing machine// dirty fix for when player controller is redone
    public event Action animationSwitch;
    private readonly int BP_Brista_PutDown_LowHash = Animator.StringToHash("BP_Brista_PutDown_Low");
    private readonly int MovementHash = Animator.StringToHash("Movement");
    private const float CrossFadeDuration = 0.1f;
    private float animationWaitTime = 1.2f;
    private bool isGivingOrderToCustomer = false;

    // Customer Animations
    [Header("Customer Animations")]
    [SerializeField] private GameObject bodiesContainerObject;
    [HideInInspector] public bool isPickedUp = false;
    private Animator customerAnimator;
    private readonly int Customer_IdleHash = Animator.StringToHash("Customer_Idle");
    private readonly int Customer_WalkHash = Animator.StringToHash("Customer_Walk");
    private readonly int Customer_StruggleHash = Animator.StringToHash("Customer_Struggle");
    private readonly int Customer_SitDownHash = Animator.StringToHash("Customer_SitDown");
    private List<int> customerBadDrinkChairHashList = new List<int>();
    private List<int> customerGoodDrinkChairHashList = new List<int>();
    private List<int> customerImpatientHashList = new List<int>();
    private bool isImpatient = false;
    private CustomerRandomizer customerRandomizer;

    [Header("Spills")]
    private bool hasDrink = false;
    [SerializeField] private bool hasSpilledCup = false;
    [SerializeField] private float maxSpillTime;
    [SerializeField] private float minSpillTime;
    [SerializeField] private float chanceToSpill;
    [SerializeField] private MessSO spillPrefab;
    [SerializeField] private Transform spillSpawnPoint;
    public delegate void CustomerLeaveEvent(int customerNumber);
    public static event CustomerLeaveEvent OnCustomerLeave;

    public delegate void OrderTimerChanged(OrderInfo orderInfo, float timer);
    public static event OrderTimerChanged OnOrderTimerChanged;

    public enum CustomerState
    {
        Wandering, Waiting, Ordering, Moving, Leaving, Insit, Init, Loitering, PickedUp, Dead, Drinking, Sitting
    }

    public void Awake()
    {
        customerRandomizer = GetComponent<CustomerRandomizer>();

        // Add Bad Drink Animation Reaction to list
        customerBadDrinkChairHashList.Add(Animator.StringToHash("Customer_Bad_Drink_Chair_1"));
        customerBadDrinkChairHashList.Add(Animator.StringToHash("Customer_Bad_Drink_Chair_2"));
        customerBadDrinkChairHashList.Add(Animator.StringToHash("Customer_Bad_Drink_Chair_3"));

        // Add Bad Drink Animation Reaction to list
        customerGoodDrinkChairHashList.Add(Animator.StringToHash("Customer_Good_Drink_Chair_1"));
        customerGoodDrinkChairHashList.Add(Animator.StringToHash("Customer_Good_Drink_Chair_2"));
        customerGoodDrinkChairHashList.Add(Animator.StringToHash("Customer_Good_Drink_Chair_3"));

        // Add Impatient Animation Reaction to list
        customerImpatientHashList.Add(Animator.StringToHash("Customer_Impatient_1"));
        customerImpatientHashList.Add(Animator.StringToHash("Customer_Impatient_2"));
        customerImpatientHashList.Add(Animator.StringToHash("Customer_Impatient_3"));

    }

    public virtual void Start()
    {
        if (IsOwner)
        {
            SetCustomerState(CustomerState.Init);
        }

        customerAnimator = bodiesContainerObject.GetComponentInChildren<Animator>();

        SetCustomerVisualIdentifiers();

        customerLeaveTime = Random.Range(GameValueHolder.Instance.difficultySettings.GetMinWaitTime(), GameValueHolder.Instance.difficultySettings.GetMaxWaitTime());
        maxInLineTime = Random.Range(GameValueHolder.Instance.difficultySettings.GetMinInLineWaitTime(), GameValueHolder.Instance.difficultySettings.GetMaxInLineWaitTime());

        agent = GetComponent<NavMeshAgent>();
        exit = CustomerManager.Instance.GetExit();
        if (distThreshold <= 0) distThreshold = 0.1f;

        GetIngredientTransform().SetParent(customerRandomizer.currentCustomerHoldPoint.transform);
        GetIngredientTransform().localPosition = new Vector3(0.067f, -0.121f, 0.055f);
        GetIngredientTransform().localEulerAngles = new Vector3(0, 30, 90);

        customerReviewPanel = GameObject.FindGameObjectWithTag("CustomerReviewPanel");
    }

    public virtual void Update()
    {
        if (IsOwner)
        {
            if (orderTimer >= 0f)
            {
                orderTimer += Time.deltaTime;
                OnOrderTimerChanged?.Invoke(order, orderTimer);
            }

            if (messTime != null)
                messTime += Time.deltaTime;

            if (lineTime != null)
            {
                lineTime += Time.deltaTime;
            }
        }

        switch (currentState.Value)
        {
            case CustomerState.Wandering:
                UpdateWandering();
                break;
            case CustomerState.Waiting:
                UpdateWaiting();
                break;
            case CustomerState.Ordering:
                UpdateOrdering();
                break;
            case CustomerState.Moving:
                UpdateMoving();
                break;
            case CustomerState.Leaving:
                UpdateLeaving();
                break;
            case CustomerState.Insit:
                UpdateInsit();
                break;
            case CustomerState.Init:
                UpdateInit();
                break;
            case CustomerState.Loitering:
                UpdateLoitering();
                break;
            case CustomerState.PickedUp:
                UpdatePickedUp();
                break;
            case CustomerState.Dead:
                UpdateDead();
                break;
            case CustomerState.Drinking:
                UpdateDrinking();
                break;
            case CustomerState.Sitting:
                UpdateSitting();
                break;
        }

        Debug.LogWarning("CustomerState " + currentState.Value);
    }


    // UPDATE<action> METHODS
    // Any Update<action> method is called by the Update() switch case.
    // When a customer's state has changed, the appropriate Update<action> method is called
    private void UpdateWandering()
    {
        //customerAnimator.CrossFadeInFixedTime(Customer1_WalkHash, CrossFadeDuration); // Customer1 walk animation

        // To be implmented or removed
    }

    private void UpdateWaiting()
    {
        
        //customerAnimator.CrossFadeInFixedTime(Customer1_IdleHash, CrossFadeDuration); // Customer1 idle animation
        // To be implmented or removed
        if (makingAMess == true) SetCustomerState(CustomerState.Loitering);

        if (inLine == true && lineTime == null)
        {
            isImpatient = false;
            lineTime = 0.0f;
        }
        else if (inLine == false) lineTime = null;

        if (lineTime > (maxInLineTime / 4) && !isImpatient)
        {
            isImpatient = true;
            if (customerImpatientHashList.Count > 0)
            {
                int randomIndex = Random.Range(0, customerImpatientHashList.Count);
                int randomHash = customerImpatientHashList[randomIndex];
                customerAnimator.CrossFadeInFixedTime(randomHash, CrossFadeDuration);
            }
        }

        if (inLine == true && lineTime > (maxInLineTime)) 
        {
            CustomerManager.Instance.customerLeaveIncrease();
            CustomerManager.Instance.ReduceCustomerLeftoServe();
            CustomerManager.Instance.LineQueue.RemoveCustomerInPos(currentPosInLine);
            CustomerLeave();
            inLine = false;
        }

    }

    private void UpdateOrdering()
    {
        
        if (inLine == true && lineTime == null)
        {
            isImpatient = false;
            lineTime = 0.0f;
        }
        else if (inLine == false) lineTime = null;

        if (lineTime > (maxInLineTime / 4) && !isImpatient)
        {
            isImpatient = true;
            if (customerImpatientHashList.Count > 0)
            {
                int randomIndex = Random.Range(0, customerImpatientHashList.Count);
                int randomHash = customerImpatientHashList[randomIndex];
                customerAnimator.CrossFadeInFixedTime(randomHash, CrossFadeDuration);
            }
        }

        if (inLine == true && lineTime > (maxInLineTime))
        {
            CustomerManager.Instance.customerLeaveIncrease();
            CustomerManager.Instance.ReduceCustomerLeftoServe();
            CustomerManager.Instance.LineQueue.RemoveCustomerInPos(currentPosInLine);
            CustomerLeave();
            inLine = false;
        }
    }

    private void UpdateMoving()
    {
        if (agent.remainingDistance < distThreshold)
        {
            agent.isStopped = true;
            if (makingAMess == true) 
            {
                customerAnimator.CrossFadeInFixedTime(Customer_IdleHash, CrossFadeDuration);
                SetCustomerState(CustomerState.Waiting);
            } 
            if (frontofLine == true)
            {
                customerAnimator.CrossFadeInFixedTime(Customer_IdleHash, CrossFadeDuration); // Customer1 idle animation
                SetCustomerState(CustomerState.Ordering);
            }
            if(inLine && frontofLine != true)
            {
                customerAnimator.CrossFadeInFixedTime(Customer_IdleHash, CrossFadeDuration); // Customer1 idle animatio
                SetCustomerState(CustomerState.Waiting);
            }
            if (!inLine && makingAMess == false)
            {
                SetCustomerState(CustomerState.Insit);
            }

            moving = false;
        }
    }

    private void UpdateLeaving()
    {
        if (!IsServer) return;
        messTime = null;
        leaving = true;
    }

    private void UpdateInsit()
    {
        if (agent.remainingDistance < distThreshold && atSit == false) atSit = true;

        if (atSit)
        {
            customerAnimator.CrossFadeInFixedTime(Customer_SitDownHash, CrossFadeDuration);

            if (orderTimer < 0)
            {
                Order();
            }
            SetCustomerState(CustomerState.Sitting);
        }

    }

    private void UpdateInit()
    {
        // To be implmented or removed
    }

    private void UpdateLoitering()
    {
        // To be implmented or removed
        if(messTime >= GameValueHolder.Instance.difficultySettings.GetLoiterMessEverySec())
        {
            CreateMess();
            RestartMessTimer();
        }

        if (randomPointCoroutine == null)
        {
            randomPointCoroutine = StartCoroutine(TryGoToRandomPoint(5f));
        }
        else StartCoroutine(TryGoToRandomPoint(5f));

    }

    private void UpdateDrinking()
    {
        if (hasDrink == true)
        {
           StartCoroutine(SpillTimer());
        }
    }

    public IEnumerator TryGoToRandomPoint(float delay)
    {
        if (leaving == true || moving == true) yield break;

        moving = true;

        yield return new WaitForSeconds(delay);

        float _radius = 5f;

        Vector3 randomPoint = Random.insideUnitSphere * _radius;
        randomPoint += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, _radius, NavMesh.AllAreas))
        {
            // 'hit.position' contains the valid random point on the NavMesh
            Debug.Log("Random point: " + hit.position);

            Walkto(hit.position);
        }
        else
        {
            // No valid point found within the specified radius
            Debug.LogWarning("Could not find a valid random point on the NavMesh.");
        }
    }

    private void StopRandomPointCoroutine()
    {
        if (randomPointCoroutine != null)
        {
            StopCoroutine(randomPointCoroutine);
            randomPointCoroutine = null;
        }
    }

    public void StopRandomPointCoroutineImmediately()
    {
        StopRandomPointCoroutine();
    }

    private void UpdatePickedUp()
    {
        if (isPickedUp == true) 
        {
            StopRandomPointCoroutineImmediately();
            customerAnimator.CrossFadeInFixedTime(Customer_StruggleHash, CrossFadeDuration);
            isPickedUp = false;
        }
        //Remove order from list if picked up
        if (OnCustomerLeave != null)
        {
            OnCustomerLeave?.Invoke(customerNumber.Value);
        }
    }

    private void UpdateSitting()
    {
        // sitting animation
        if (atSit)
        {
            //customerAnimator.CrossFadeInFixedTime(Customer1_IdleHash, CrossFadeDuration); // Customer1 idle animation
        }

        if (!orderBeingServed)
            DisplayCustomerVisualIdentifiers();
        orderBeingServed = true;
        if (orderTimer >= customerLeaveTime)
        {
            CustomerManager.Instance.customerLeaveIncrease();
            CustomerManager.Instance.ReduceCustomerLeftoServe();
            GameManager.Instance.moneySystem.ResetStreak();
            CustomerLeave();

            Debug.LogWarning("Unhappy Customer");
        }

    }

    private void UpdateDead()
    {
        // To be implmented or removed
    }

    // INTERACTION
    // Anything related to interacting with the customer goes here
    // This includes delivering a drink, picking up, throwing, assaulting, etc...
    public override void Interact(PlayerController player)
    {
        // Customer is going to be thrown or assaulted with a weapon
        if (player.HasPickup() && player.Pickup.attributes.Contains(Pickup.PickupAttribute.KillsCustomer))
        {
            HeadDetach();
            agent.speed = 0;
            return;
        }

        // Take customer order
        if (GetCustomerState() == CustomerState.Ordering)
        {
            OrderManager.Instance.SpawnOrder(this);
            if (TutorialManager.Instance != null && TutorialManager.Instance.tutorialEnabled && !TutorialManager.Instance.firstBrewStarted)
                TutorialManager.Instance.StartFirstBrew(order);
            LeaveLineServerRpc();
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.interactCustomer);
            interactParticle.Play();
        }

        // Deliver customer order
        else if (GetCustomerState() == CustomerState.Sitting && player.GetIngredient().CompareTag("CoffeeCup") && !isGivingOrderToCustomer)
        {
            isGivingOrderToCustomer = true;
            DropCupAnimation(player);// Play animation and handles delivering the drink
   
        }
        
        else if (GetCustomerState()== CustomerState.Leaving && player.GetIngredient().CompareTag("CoffeeCup") && !isGivingOrderToCustomer)
        {
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.failedInteration);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void LeaveLineServerRpc()
    {
        LeaveLineClientRpc();
    }

    [ClientRpc]
    private void LeaveLineClientRpc()
    {
        CustomerManager.Instance.Leaveline();
    }

    // CUSTOMER STATE METHODS
    // Setting or retrieving customer state should be done through
    // thse methods.  Do not set the state like customerstate = "Leaving"
    public CustomerState GetCustomerState()
    {
        return currentState.Value;
    }

    public void SetCustomerState(CustomerState customerState)
    {
        if (!IsOwner) return;
        SetCustomerStateServerRpc(customerState);
    }

    [ServerRpc]
    private void SetCustomerStateServerRpc(CustomerState customerState)
    {
        currentState.Value = customerState;
    }

    // CUSTOMER IDENTIFICATION METHODS
    // These methods are for setting or displaying visual identifiers
    // such as customer names, reviews, dialogue, numbers, etc...
     public void SetCustomerName(FixedString32Bytes newName)
    {
        customerName.Value = newName;
    }

    public void SetCustomerVisualIdentifiers()
    {
        SetCustomerVisualIdentifiersClientRpc();
    }

    [ClientRpc]
    private void SetCustomerVisualIdentifiersClientRpc()
    {
        customerNumberText.text = customerNumber.Value.ToString();
        customerNameText.text = customerName.Value.ToString();
        customerNumberCanvas.enabled = false;
    }

    public void DisplayCustomerVisualIdentifiers()
    {
        DisplayCustomerVisualIdentifiersClientRpc();
    }

    [ClientRpc]
    private void DisplayCustomerVisualIdentifiersClientRpc()
    {
        customerNumberCanvas.enabled = true;
        //UIManager.Instance.ShowCustomerUiOrder(this);
    }

    // CUSTOMER ACTION METHODS
    // These methods are when a customer is to perform an action
    // Should be called from the Update<action> method when customer state changes
    public virtual void Order()
    {
        OrderClientRpc();
        // DisplayCustomerVisualIdentifiers();
        // which state sends it to find a seat?
    }

    [ClientRpc]
    private void OrderClientRpc()
    {
        StartOrderTimer();
    }

    private IEnumerator Drink()
    {
        SetCustomerState(CustomerState.Drinking);

        // Start Random Drink Reaction Animation
        if (customerInstanceReviewScore <= 3)
        {
            if (customerBadDrinkChairHashList.Count > 0)
            {
                int randomIndex = Random.Range(0, customerBadDrinkChairHashList.Count);
                int randomHash = customerBadDrinkChairHashList[randomIndex];
                customerAnimator.CrossFadeInFixedTime(randomHash, CrossFadeDuration);
            }
        }
        else if (customerInstanceReviewScore >= 4)
        {
            if (customerBadDrinkChairHashList.Count > 0)
            {
                int randomIndex = Random.Range(0, customerGoodDrinkChairHashList.Count);
                int randomHash = customerGoodDrinkChairHashList[randomIndex];
                customerAnimator.CrossFadeInFixedTime(randomHash, CrossFadeDuration);
            }
        }

        float drinkingDur = Random.Range(GameValueHolder.Instance.difficultySettings.GetMinDrinkingDurationTime(), GameValueHolder.Instance.difficultySettings.GetMaxDrinkingDurationTime());       
        yield return new WaitForSeconds(drinkingDur);

        CustomerLeave();

    }

    public virtual void CustomerLeave()
    {
        if (agent.isStopped) agent.isStopped = false;
        
        atSit = false;

        if (GetCustomerState() == CustomerState.Drinking && Random.Range(0, 100) <= GameValueHolder.Instance.difficultySettings.GetChanceToMess()) CreateMess();
        if (GetCustomerState() == CustomerState.Drinking && Random.Range(0, 100) < GameValueHolder.Instance.difficultySettings.GetChanceToLoiter())
        {
            messTime = 0f;
            makingAMess = true;
            moving = false;
            SetCustomerState(CustomerState.Loitering);
        }
        else
        {
            customerAnimator.CrossFadeInFixedTime(Customer_WalkHash, CrossFadeDuration); // Customer1 walk animation
            agent.SetDestination(exit);
            SetCustomerState(CustomerState.Leaving);
        }

        if (OnCustomerLeave != null)
        {
            OnCustomerLeave?.Invoke(customerNumber.Value);
            OrderManager.Instance.FinishOrder(order);
        }
    }

    public void Walkto(Vector3 Spot)
    {
        customerAnimator.CrossFadeInFixedTime(Customer_WalkHash, CrossFadeDuration); // Customer1 walk animation
        SetCustomerState(CustomerState.Moving);
        moving = true;
        WalkToClientRpc(Spot.x, Spot.y, Spot.z);
    }

    [ClientRpc]
    private void WalkToClientRpc(float x, float y, float z)
    {
        if (agent.isStopped) agent.isStopped = false;
        agent.SetDestination(new Vector3(x, y, z));
    }

    public void JustGotHandedCoffee()
    {
        if (TutorialManager.Instance != null && TutorialManager.Instance.tutorialEnabled && !TutorialManager.Instance.firstDrinkDelivered)
            TutorialManager.Instance.FirstDrinkDelivered();
        JustGotHandedCoffeeServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void JustGotHandedCoffeeServerRpc()
    {
        JustGotHandedCoffeeClientRpc();
    }

    [ClientRpc]
    private void JustGotHandedCoffeeClientRpc()
    {
        CustomerManager.Instance.customerServedIncrease();
        CustomerManager.Instance.ReduceCustomerLeftoServe();
        CustomerReviewManager.Instance.CustomerReviewEvent(this);
        customerInstanceReviewScore = CustomerReviewManager.Instance.reviewScore;
        if (OnCustomerLeave != null) OnCustomerLeave?.Invoke(customerNumber.Value);
        OrderManager.Instance.FinishOrder(order);
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.yorpReview);
        StopOrderTimer();
        StartCoroutine(Drink());
    }

    void HeadDetach()
    {
        detachedHead.Initialize();
        SetCustomerState(CustomerState.Dead);
        StartCoroutine(DeadTimer());
    }

    public void Dead()
    {
        SetCustomerState(CustomerState.Dead);
        StartCoroutine(DeadTimer());
    }

    IEnumerator DeadTimer()
    {
        yield return new WaitForSeconds(deadTimerSeconds);
        CustomerManager.Instance.LineQueue.GetFirstInQueue(); // moves everyone up one and pops out position 0
        CustomerManager.Instance.LineQueue.RemoveFromQueue(this);
        Destroy(gameObject);
    }

    public void SpawnSpill()
    {
        if (GameManager.Instance.canSpawnSpill == true && hasSpilledCup == false)
        {
            if ((Random.value < chanceToSpill))
            {
                Spill.CreateSpill(spillPrefab, this);
                GameManager.Instance.AddSpill();
                hasSpilledCup = true;
            } 
            Debug.Log("Failed to spawn spill");
        }
    }
    
    IEnumerator SpillTimer()
    {
        yield return new WaitForSeconds(Random.Range(minSpillTime, maxSpillTime));
        SpawnSpill();
        
    }
    public int GetCustomerNumber()
    {
        return customerNumber.Value;
    }

    public void SetOrder(OrderInfo order)
    {
        SetOrderServerRpc(order);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetOrderServerRpc(OrderInfo order)
    {
        this.order = order;
    }

    private void Reorder()
    {
        Debug.Log("customer is not happy with the serving and wants you to try again");

       //customerReactionIndicator.CustomerSad();
    }

    public void StartOrderTimer()
    {
        StartOrderTimerServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartOrderTimerServerRpc()
    {
        orderTimer = 0f;
    }

    public void StopOrderTimer()
    {
        orderTimer = -1f;
    }

    public void RestartMessTimer()
    {
        messTime = 0f;  
    }

    public void StopMessTimer()
    {
        messTime = null;
    }
    public void CreateMess()
    {
        SpawnMessServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnMessServerRpc()
    {
        SpawnMessClientRpc();
    }

    [ClientRpc]
    public void SpawnMessClientRpc()
    {
        Pickup.SpawnPickupItem(pickupSO, this);
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.cupDropped);
        //Instantiate(spillPrefab.prefab, spillSpawnPoint.position, Quaternion.identity);
        messTime = 0f;
    }

    private void DropCupAnimation(PlayerController player)
    {
        StartCoroutine(ResetAnimation(player));
    }

    private IEnumerator ResetAnimation(PlayerController player)
    {
        player.anim.CrossFadeInFixedTime(BP_Brista_PutDown_LowHash, CrossFadeDuration);
        player.movementToggle = false;

        yield return new WaitForSeconds(animationWaitTime);
        player.GetIngredient().SetIngredientParent(this);
        JustGotHandedCoffee();
        player.RemoveIngredientInListByReference(player.GetIngredient());
        interactParticle.Play();
        player.anim.CrossFadeInFixedTime(MovementHash, CrossFadeDuration);

        animationSwitch?.Invoke();
        isGivingOrderToCustomer = false;
        player.movementToggle = true;
    }

    public override void OnDestroy()
    {
        Debug.Log("destroying customer");
        base.OnDestroy();
        if (HasIngredient())
        {
            Debug.Log("destroying customer has ingredient");
            Ingredient.DestroyIngredient(GetIngredient());
        }
    }

}
