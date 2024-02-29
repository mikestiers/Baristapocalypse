using JetBrains.Annotations;
using System;
using System.Collections;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static BrewingStation;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public class CustomerBase : Base
{
    [Header("Navigation")]
    public NavMeshAgent agent;
    public bool frontofLine;
    public NetworkVariable<bool> inLine = new NetworkVariable<bool>();
    public bool atSit = false; //for tables customer rotation orientation 
    public bool leaving = false;
    public bool makingAMess = false;
    public bool moving;
    public float distThreshold;
    public GameObject[] Line;
    public int LineIndex;
    private Transform exit;
    public int currentPosInLine;

    [Header("Identifiers")]
    public NetworkVariable<FixedString32Bytes> customerName = new NetworkVariable<FixedString32Bytes>();
    public NetworkVariable<int> customerNumber = new NetworkVariable<int>();
    private bool orderBeingServed;

    [Header("Coffee Attributes")]
    public CoffeeAttributes coffeeAttributes;
    public OrderInfo order;

    [Header("State Related")]
    public NetworkVariable<CustomerState> currentState = new NetworkVariable<CustomerState>(CustomerState.Init);
    public float orderTimer = -1f;
    public float? messTime = null;
    public float? lineTime = null;
    public float customerLeaveTime;
    public float maxInLineTime;
    public float deadTimerSeconds = 5.0f;

    [Header("Visuals")]
    [SerializeField] public Canvas customerNumberCanvas;
    [SerializeField] private Text customerNumberText;
    [SerializeField] private Text customerNameText;
    [SerializeField] private ParticleSystem interactParticle;
    [SerializeField] private DetachedHead detachedHead;
    [SerializeField] private ScoreTimerManager scoreTimerManager;
    [SerializeField] private MessSO spillPrefab;
    [SerializeField] private Transform spillSpawnPoint;
    [SerializeField] private PickupSO pickupSO;

    [Header("Customer Review")]
    public GameObject customerReviewPrefab;
    private GameObject customerReviewPanel;

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
    private Animator customerAnimator;
    private readonly int Customer1_IdleHash = Animator.StringToHash("Customer1_Idle");
    private readonly int Customer1_WalkHash = Animator.StringToHash("Customer1_Walk");
    private readonly int Customer1_StruggleHash = Animator.StringToHash("Customer1_Struggle");


    public delegate void CustomerLeaveEvent(int customerNumber);
    public static event CustomerLeaveEvent OnCustomerLeave;
    
    public enum CustomerState
    {
        Wandering, Waiting, Ordering, Moving, Leaving, Insit, Init, Loitering, PickedUp, Dead, Drinking, Sitting
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
        if (distThreshold <= 0) distThreshold = 0.5f;
        

        customerReviewPanel = GameObject.FindGameObjectWithTag("CustomerReviewPanel");

    }

    public virtual void Update()
    {
        if (IsOwner)
        {
            if (orderTimer >= 0f)
            {
                orderTimer += Time.deltaTime;
            }

            //Debug.LogWarning("CustomerState " + currentState.Value);

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
    }


    // UPDATE<action> METHODS
    // Any Update<action> method is called by the Update() switch case.
    // When a customer's state has changed, the appropriate Update<action> method is called
    private void UpdateWandering()
    {
        customerAnimator.CrossFadeInFixedTime(Customer1_WalkHash, CrossFadeDuration); // Customer1 walk animation

        // To be implmented or removed
    }

    private void UpdateWaiting()
    {
        customerAnimator.CrossFadeInFixedTime(Customer1_IdleHash, CrossFadeDuration); // Customer1 idle animation
        // To be implmented or removed
        if (makingAMess == true) SetCustomerState(CustomerState.Loitering);

        if (inLine.Value == true && lineTime == null) lineTime = 0.0f;
        else if (inLine.Value == false) lineTime = null;

        if (inLine.Value == true && lineTime > (maxInLineTime)) 
        {
            CustomerManager.Instance.customerLeaveIncrease();
            CustomerManager.Instance.ReduceCustomerLeftoServe();
            CustomerManager.Instance.LineQueue.RemoveCustomerInPos(currentPosInLine);
            CustomerLeave();
            inLine.Value = false;
        }

    }

    private void UpdateOrdering()
    {
        customerAnimator.CrossFadeInFixedTime(Customer1_IdleHash, CrossFadeDuration); // Customer1 idle animation
        if (orderTimer < 0)
        {
            //Order();
            OrderClientRpc();
        }

        if (inLine.Value == true && lineTime == null) lineTime = 0.0f;
        else if (inLine.Value == false) lineTime = null;

        if (inLine.Value == true && lineTime > (maxInLineTime))
        {
            CustomerManager.Instance.customerLeaveIncrease();
            CustomerManager.Instance.ReduceCustomerLeftoServe();
            CustomerManager.Instance.LineQueue.RemoveCustomerInPos(currentPosInLine);
            CustomerLeave();
            inLine.Value = false;
        }
    }

    private void UpdateMoving()
    {
        if (agent.remainingDistance < distThreshold)
        {
            agent.isStopped = true;
            if (frontofLine == true)
            {
                SetCustomerState(CustomerState.Ordering);
            }
            else
            {
                SetCustomerState(CustomerState.Waiting);
            }

            moving = false;
        }
    }

    private void UpdateLeaving()
    {
        messTime = null;
        leaving = true;

        if (agent.remainingDistance < distThreshold)
        {
            Destroy(gameObject);
        }
    }

    private void UpdateInsit()
    {
        if (agent.remainingDistance < distThreshold && atSit == false) atSit = true;

        if (atSit) SetCustomerState(CustomerState.Sitting);   
    }

    private void UpdateInit()
    {
        // To be implmented or removed
    }

    private void UpdateLoitering()
    {
        if (leaving == true)
        {
            SetCustomerState(CustomerState.Leaving);
            agent.SetDestination(exit.position);
        }
    

        // To be implmented or removed
        if(messTime >= GameValueHolder.Instance.difficultySettings.GetLoiterMessEverySec())
        {
            CreateMess();
            RestartMessTimer();
        }

        StartCoroutine(TryGoToRandomPoint(5f));
    }

    private void UpdateDrinking()
    {
       //update DRINKING ANIMATION
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

    private void UpdatePickedUp()
    {
        customerAnimator.CrossFadeInFixedTime(Customer1_StruggleHash, CrossFadeDuration); // Customer1 idle animation
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
            customerAnimator.CrossFadeInFixedTime(Customer1_IdleHash, CrossFadeDuration); // Customer1 idle animation
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

        /*
        {  if(makingAMess == true)
        {
            SetCustomerState(CustomerState.Leaving);
            agent.SetDestination(exit.position);
            makingAMess = false;
            leaving = true;

            //CustomerManager.Instance.ReduceCustomerInStore(); //reduce from counter to stop the waves when enough
            //UIManager.Instance.customersInStore.text = ("Customers in Store: ") + CustomerManager.Instance.GetCustomerLeftinStore().ToString();
            //if (CustomerManager.Instance.GetCustomerLeftinStore() <= 0) CustomerManager.Instance.NextWave(); // Check if Last customer in Wave trigger next Shift
        }
        */
        
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
        StartOrderTimer();
        // DisplayCustomerVisualIdentifiers();
        // which state sends it to find a seat?
    }

    [ClientRpc]
    private void OrderClientRpc()
    {
        Order();
    }

    private IEnumerator Drink()
    {
        SetCustomerState(CustomerState.Drinking);
        //Start Animation for drinking
        float drinkingDur = Random.Range(GameValueHolder.Instance.difficultySettings.GetMinDrinkingDurationTime(), GameValueHolder.Instance.difficultySettings.GetMaxDrinkingDurationTime());

        yield return new WaitForSeconds(drinkingDur);

        CustomerLeave();

    }

    public virtual void CustomerLeave()
    {
        if (agent.isStopped) agent.isStopped = false;
        
        atSit = false;

        if (GetCustomerState() == CustomerState.Drinking && Random.Range(0, 100) <= GameValueHolder.Instance.difficultySettings.GetChanceToMess()) CreateMess();
        if (Random.Range(0, 100) < GameValueHolder.Instance.difficultySettings.GetChanceToLoiter())
        {
            SetCustomerState(CustomerState.Loitering);
            messTime = 0f;
            makingAMess = true;
            moving = false;
        }
        else
        {
            customerAnimator.CrossFadeInFixedTime(Customer1_WalkHash, CrossFadeDuration); // Customer1 walk animation
            SetCustomerState(CustomerState.Leaving);
            agent.SetDestination(exit.position);


            //CustomerManager.Instance.ReduceCustomerInStore(); //reduce from counter to stop the waves when enough
            //UIManager.Instance.customersInStore.text = ("Customers in Store: ") + CustomerManager.Instance.GetCustomerLeftinStore().ToString();
            //if (CustomerManager.Instance.GetCustomerLeftinStore() <= 0) CustomerManager.Instance.NextWave(); // Check if Last customer in Wave trigger next Shift
        }

        if (OnCustomerLeave != null)
        {
            OnCustomerLeave?.Invoke(customerNumber.Value);
            OrderManager.Instance.FinishOrder(order);
        }
    }

    public void Walkto(Vector3 Spot)
    {
        WalkToClientRpc(Spot.x, Spot.y, Spot.z);
        
    }

    [ClientRpc]
    private void WalkToClientRpc(float x, float y, float z)
    {
        if (agent.isStopped) agent.isStopped = false;
        agent.SetDestination(new Vector3(x, y, z));
        customerAnimator.CrossFadeInFixedTime(Customer1_WalkHash, CrossFadeDuration); // Customer1 walk animation
        SetCustomerState(CustomerState.Moving);
        moving = true;
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
        OrderManager.Instance.FinishOrder(order);
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

    public int GetCustomerNumber()
    {
        return customerNumber.Value;
    }

    public void SetOrder(OrderInfo order)
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
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.interactCustomer);
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
