using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
public class CustomerBase : Base
{
    [Header("Navigation")]
    public NavMeshAgent agent;
    public bool frontofLine;
    public float distThreshold;
    public GameObject[] Line;
    public int LineIndex;
    private Transform exit;

    [Header("Identifiers")]
    public string customerName;
    public int customerNumber;

    [Header("Coffee Attributes")]
    public CoffeeAttributes coffeeAttributes;

    [Header("State Related")]
    public CustomerState currentState;
    public float? orderTimer = null;
    public float customerLeaveTime = 60f;
    public float deadTimerSeconds = 5.0f;

    [Header("Visuals")]
    [SerializeField] public Canvas customerNumberCanvas;
    [SerializeField] private Text customerNumberText;
    [SerializeField] private Text customerNameText;
    [SerializeField] private ParticleSystem interactParticle;
    [SerializeField] private DetachedHead detachedHead;
    [SerializeField] private ScoreTimerManager scoreTimerManager;
    [SerializeField] public GameObject customerDialogue;

    public enum CustomerState
    {
        Wandering, Waiting, Ordering, Moving, Leaving, Insit, Init, Loitering, PickedUp, Dead
    }

    public virtual void Start()
    {
        SetCustomerState(CustomerState.Init);
        SetCustomerVisualIdentifiers();

        agent = GetComponent<NavMeshAgent>();
        exit = CustomerManager.Instance.GetExit();
        if (distThreshold <= 0) distThreshold = 0.5f;
    }

    public virtual void Update()
    {
        if (orderTimer != null)
            orderTimer += Time.deltaTime;

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
                SetCustomerState(CustomerState.Ordering);
            }
            else
            {
                SetCustomerState(CustomerState.Waiting);
            }
        }
    }

    private void UpdateLeaving()
    {
        if (agent.remainingDistance < distThreshold)
        {
            Destroy(gameObject);
            Debug.Log("this is our disappearing customer issue"); // if you see this, the customer probably disappeared and the review didn't show.  something about being close to the entrance causes the player to destroy on leaving
            UIManager.Instance.RemoveCustomerUiOrder(this);
        }
    }

    private void UpdateInsit()
    {
        customerDialogue.SetActive(false);
        if (orderTimer >= customerLeaveTime)
            CustomerLeave();
    }

    private void UpdateInit()
    {
        // To be implmented or removed
    }

    private void UpdateLoitering()
    {
        // To be implmented or removed
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
        if (!IsOwner) return;

        // Customer is going to be thrown or assaulted with a weapon
        if (player.IsHoldingPickup && player.Pickup.attributes.Contains(Pickup.PickupAttribute.KillsCustomer))
        {
            HeadDetach();
            agent.speed = 0;
            return;
        }

        // Take customer order
        if (GetCustomerState() == CustomerState.Ordering)
        {
            CustomerManager.Instance.Leaveline();
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.interactCustomer);
            interactParticle.Play();
        }

        // Deliver customer order
        else if (GetCustomerState() == CustomerState.Insit && player.GetIngredient().CompareTag("CoffeeCup"))
        {
            player.GetIngredient().SetIngredientParent(this);
            JustGotHandedCoffee(this.GetIngredient().GetComponent<CoffeeAttributes>());
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.interactCustomer);
            interactParticle.Play();
        }
    }

    // CUSTOMER STATE METHODS
    // Setting or retrieving customer state should be done through
    // thse methods.  Do not set the state like customerstate = "Leaving"
    public CustomerState GetCustomerState()
    {
        return currentState;
    }

    public void SetCustomerState(CustomerState newState)
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
        UIManager.Instance.ShowCustomerUiOrder(this);
    }

    // CUSTOMER ACTION METHODS
    // These methods are when a customer is to perform an action
    // Should be called from the Update<action> method when customer state changes
    public virtual void Order()
    {
        StartOrderTimer();
        DisplayCustomerVisualIdentifiers();
        // which state sends it to find a seat?
    }

    [ClientRpc]
    private void OrderClientRpc()
    {
        Order();
    }

    public virtual void CustomerLeave()
    {
        SetCustomerState(CustomerState.Leaving);
        agent.SetDestination(exit.position);
    }

    public void Walkto(Vector3 Spot)
    {
        if (agent.isStopped) agent.isStopped = false;
        agent.SetDestination(Spot);
        SetCustomerState(CustomerState.Moving);
    }

    public void JustGotHandedCoffee(CoffeeAttributes coffee)
    {
        CustomerReaction(coffee, coffeeAttributes);
        UIManager.Instance.ShowCustomerReview(this);
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
        UIManager.Instance.RemoveCustomerUiOrder(this);
        CustomerManager.Instance.LineQueue.GetFirstInQueue(); // moves everyone up one and pops out position 0
        CustomerManager.Instance.LineQueue.RemoveFromQueue(this);
        Destroy(gameObject);
    }

    // CUSTOMER REACTION METHODS
    // This section is used for anything related to custome reactions
    // which are typically based off the quality of the drink or other environmental
    // factors, such as order wait time, wifi, radio, pat on the back, etc...
    private void CustomerReaction(CoffeeAttributes coffeeAttributes, CoffeeAttributes customerAttributes)
    {
        int result = 0;
        result += (Mathf.Abs(coffeeAttributes.GetSweetness() - customerAttributes.GetSweetness()) <= 5) ? 1 : -1;
        result += (Mathf.Abs(coffeeAttributes.GetBitterness() - customerAttributes.GetBitterness()) <= 5) ? 1 : -1;
        result += (Mathf.Abs(coffeeAttributes.GetSpiciness() - customerAttributes.GetSpiciness()) <= 5) ? 1 : -1;
        result += (Mathf.Abs(coffeeAttributes.GetTemperature() - customerAttributes.GetTemperature()) <= 5) ? 1 : -1;
        result += (Mathf.Abs(coffeeAttributes.GetStrength() - customerAttributes.GetStrength()) <= 5) ? 1 : -1;

        int minigameResult = coffeeAttributes.GetIsMinigamePerfect() ? 1 : 0;
        ScoreTimerManager.Instance.score += result * (minigameResult + 1);
        Debug.Log($"Result for {customerNumber}: {result}");
        switch (result)
        {
            case 5:
                Perfect();
                ScoreTimerManager.Instance.IncrementStreak();
                ScoreTimerManager.Instance.score += result * ScoreTimerManager.Instance.StreakCount;
                CustomerLeave();
                break;

            case 4:
            case 3:
            case 2:
            case 1:
                ScoreTimerManager.Instance.ResetStreak();
                CustomerLeave();
                break;

            case -1:
            case -2:

                Reorder();
                CancelInvoke("CustomerLeave");
                Order();
                break;

            case -3:
            case -4:
            case -5:

                Angry();
                ScoreTimerManager.Instance.score += result;
                CustomerLeave();
                break;
        }
    }

    private void Angry()
    {
        Debug.Log("the customer is not happy with the serving");
    }

    private void Perfect()
    {
        Debug.Log("you did great!");
    }

    private void Reorder()
    {
        Debug.Log("customer is not happy with the serving and wants you to try again");
    }

    public void StartOrderTimer()
    {
        orderTimer = 0f;
    }

    public void StopOrderTimer()
    {
        orderTimer = null;
    }
}
