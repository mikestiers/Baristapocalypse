using System;
using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public class CustomerBase : Base
{
    [Header("Navigation")]
    public NavMeshAgent agent;
    public bool frontofLine;
    public bool inLine;
    public bool leaving = false;
    public bool makingAMess = false;
    public bool moving;
    public float distThreshold;
    public GameObject[] Line;
    public int LineIndex;
    private Transform exit;
    public int currentPosInLine;

    [Header("Identifiers")]
    public string customerName;
    public int customerNumber;
    private bool orderBeingServed;

    [Header("Coffee Attributes")]
    public CoffeeAttributes coffeeAttributes;
    public Order order;

    [Header("State Related")]
    public CustomerState currentState;
    public float? orderTimer = null;
    public float? messTime = null;
    public float customerLeaveTime;
    public float deadTimerSeconds = 5.0f;

    [Header("Visuals")]
    [SerializeField] public Canvas customerNumberCanvas;
    [SerializeField] private Text customerNumberText;
    [SerializeField] private Text customerNameText;
    [SerializeField] private ParticleSystem interactParticle;
    [SerializeField] private DetachedHead detachedHead;
    [SerializeField] private ScoreTimerManager scoreTimerManager;
    [SerializeField] public GameObject customerDialogue;
    [SerializeField] private MessSO spillPrefab;
    [SerializeField] private Transform spillSpawnPoint;
    [SerializeField] private PickupSO pickupSO;

    [Header("Customer Review")]
    public GameObject customerReviewPrefab;
    private GameObject customerReviewPanel;

    public enum CustomerState
    {
        Wandering, Waiting, Ordering, Moving, Leaving, Insit, Init, Loitering, PickedUp, Dead
    }

    public virtual void Start()
    {
        SetCustomerStateServerRpc(CustomerState.Init);
        SetCustomerVisualIdentifiers();

        customerLeaveTime = Random.Range(GameManager.Instance.difficultySettings.GetMinWaitTime(), GameManager.Instance.difficultySettings.GetMaxWaitTime());

        agent = GetComponent<NavMeshAgent>();
        exit = CustomerManager.Instance.GetExit();
        if (distThreshold <= 0) distThreshold = 0.5f;
        
        customerReviewPanel = GameObject.FindGameObjectWithTag("CustomerReviewPanel");
    }

    public virtual void Update()
    {
        if (orderTimer != null)
            orderTimer += Time.deltaTime;

        if (messTime != null)
            messTime += Time.deltaTime; 

        switch (currentState)
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
        }
    }

    // UPDATE<action> METHODS
    // Any Update<action> method is called by the Update() switch case.
    // When a customer's state has changed, the appropriate Update<action> method is called
    private void UpdateWandering()
    {
        // To be implmented or removed
    }

    private void UpdateWaiting()
    {
        // To be implmented or removed
        if (makingAMess == true) SetCustomerStateServerRpc(CustomerState.Loitering);


    }

    private void UpdateOrdering()
    {
        if (orderTimer == null)
        {
            //Order();
            OrderClientRpc();
        }
    }

    private void UpdateMoving()
    {
        if (agent.remainingDistance < distThreshold)
        {
            agent.isStopped = true;
            if (frontofLine == true)
            {
                SetCustomerStateServerRpc(CustomerState.Ordering);
            }
            else
            {
                SetCustomerStateServerRpc(CustomerState.Waiting);
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
            Debug.Log("this is our disappearing customer issue"); // if you see this, the customer probably disappeared and the review didn't show.  something about being close to the entrance causes the player to destroy on leaving
            //UIManager.Instance.RemoveCustomerUiOrder(this);
        }
    }

    private void UpdateInsit()
    {
        customerDialogue.SetActive(false);
        if (!orderBeingServed)
            DisplayCustomerVisualIdentifiers();
        orderBeingServed = true;
        if (orderTimer >= customerLeaveTime)
        {
            CustomerManager.Instance.customerLeaveIncrease();
            CustomerLeave();

            Debug.LogWarning("Unhappy Customer");
        }
            
    }

    private void UpdateInit()
    {
        // To be implmented or removed
    }

    private void UpdateLoitering()
    {
        if (leaving == true)
        {
            SetCustomerStateServerRpc(CustomerState.Leaving);
            agent.SetDestination(exit.position);
        }
    

        // To be implmented or removed
        if(messTime >= GameManager.Instance.difficultySettings.GetLoiterMessEverySec())
        {
            CreateMess();
            RestartMessTimer();
        }

        StartCoroutine(TryGoToRandomPoint(5f));
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
        // To be implmented or removed
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
            Order order = new Order();
            order.Initialize(this);
            LeaveLineServerRpc();
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.interactCustomer);
            interactParticle.Play();
        }

        // Deliver customer order
        else if (GetCustomerState() == CustomerState.Insit && player.GetIngredient().CompareTag("CoffeeCup"))
        {
            player.GetIngredient().SetIngredientParent(this);
            JustGotHandedCoffee(this.GetIngredient().GetComponent<CoffeeAttributes>());
            player.RemoveIngredientInListByReference(player.GetIngredient());
            CustomerManager.Instance.customerServedIncrease();

            SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.interactCustomer);
            interactParticle.Play();
        }

        if(makingAMess == true)
        {
            SetCustomerStateServerRpc(CustomerState.Leaving);
            agent.SetDestination(exit.position);
            makingAMess = false;
            leaving = true;

            //CustomerManager.Instance.ReduceCustomerInStore(); //reduce from counter to stop the waves when enough
            //UIManager.Instance.customersInStore.text = ("Customers in Store: ") + CustomerManager.Instance.GetCustomerLeftinStore().ToString();
            //if (CustomerManager.Instance.GetCustomerLeftinStore() <= 0) CustomerManager.Instance.NextWave(); // Check if Last customer in Wave trigger next Shift
        }
        
    }

    [ServerRpc(RequireOwnership = false)]
    private void LeaveLineServerRpc()
    {
        Debug.Log("serverrpc");
        LeaveLineClientRpc();
    }

    [ClientRpc]
    private void LeaveLineClientRpc()
    {
        Debug.Log("customer leaving");
        CustomerManager.Instance.Leaveline();
       
    }

    // CUSTOMER STATE METHODS
    // Setting or retrieving customer state should be done through
    // thse methods.  Do not set the state like customerstate = "Leaving"
    public CustomerState GetCustomerState()
    {
        return currentState;
    }

    //Maybe dont need, can try to get rid of it later
    [ServerRpc(RequireOwnership = false)]
    public void SetCustomerStateServerRpc(CustomerState newState)
    {
        SetCustomerStateClientRpc(newState);
    }

    [ClientRpc]
    public void SetCustomerStateClientRpc(CustomerState newState)
    {
        currentState = newState;
    }

    // CUSTOMER IDENTIFICATION METHODS
    // These methods are for setting or displaying visual identifiers
    // such as customer names, reviews, dialogue, numbers, etc...
     public void SetCustomerName(String newName)
    {
        customerName = newName;
    }

    public void SetCustomerVisualIdentifiers()
    {
        customerNumberText.text = customerNumber.ToString();
        customerNameText.text = customerName;
        customerDialogue.SetActive(false);
        customerNumberCanvas.enabled = false;
       
    }

    public void DisplayCustomerVisualIdentifiers()
    {
        customerNumberCanvas.enabled = true;
        customerDialogue.SetActive(true);
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

    public virtual void CustomerLeave()
    {
        if (Random.Range(0, 100) <= GameManager.Instance.difficultySettings.GetChanceToMess()) CreateMess();
        if (Random.Range(0, 100) < GameManager.Instance.difficultySettings.GetChanceToLoiter())
        {
            SetCustomerStateServerRpc(CustomerState.Loitering);
            messTime = 0f;
            makingAMess = true;
            moving = false;
        }
        else
        {
            SetCustomerStateServerRpc(CustomerState.Leaving);
            agent.SetDestination(exit.position);


            //CustomerManager.Instance.ReduceCustomerInStore(); //reduce from counter to stop the waves when enough
            //UIManager.Instance.customersInStore.text = ("Customers in Store: ") + CustomerManager.Instance.GetCustomerLeftinStore().ToString();
            //if (CustomerManager.Instance.GetCustomerLeftinStore() <= 0) CustomerManager.Instance.NextWave(); // Check if Last customer in Wave trigger next Shift
        }
    }

    public void Walkto(Vector3 Spot)
    {
        if (agent.isStopped) agent.isStopped = false;
        agent.SetDestination(Spot);
        SetCustomerStateServerRpc(CustomerState.Moving);
        moving = true;
    }

    public void JustGotHandedCoffee(CoffeeAttributes coffee)
    {
        CustomerReviewManager.Instance.ShowCustomerReview(this);
        StopOrderTimer();
        CustomerLeave();
    }

    void HeadDetach()
    {
        detachedHead.Initialize();
        SetCustomerStateServerRpc(CustomerState.Dead);
        StartCoroutine(DeadTimer());
    }

    public void Dead()
    {
        SetCustomerStateServerRpc(CustomerState.Dead);
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
        return customerNumber;
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
        orderTimer = null;
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

    private void OnDestroy()
    {
       if(Application.isPlaying) CustomerManager.Instance.ReduceCustomerInStore();
    }
}
