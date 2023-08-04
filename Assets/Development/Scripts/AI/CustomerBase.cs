using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms.Impl;
using static UnityEngine.EventSystems.EventTrigger;

[RequireComponent(typeof(NavMeshAgent))]
public class CustomerBase : BaseStation
{
    //public Transform target;
    public NavMeshAgent agent;
    private Transform exit;
    public bool frontofLine;

    public float distThreshold;

    public static CustomerBase Instance { get; private set; }

    public enum CustomerState
    {
        //add movetosit when sits are available
        Wandering, Waiting, Ordering, Moving, Leaving, Insit, Init
    }

    public CoffeeAttributes coffeeAttributes;
    [SerializeField] private ParticleSystem interactParticle;
    //Initial State
    public CustomerState currentState = CustomerState.Init;

    //Waiting In line to order Array
    public GameObject[] Line;
    public int LineIndex;

    [SerializeField] private ScoreTimerManager scoreTimerManager;

    /// <summary>
    ///  For the arrays im thinking of moving them to some other script like a level script or something to be actually be 
    ///  accesible by all customer prefabs on what Index they currently are in 
    ///  (im thinking of copying something like the queue DT with had from programming fundi to the line array)
    ///  we can also add sits as an array instead of waypoints
    /// </summary>


    /// <summary> Suggested stuff in common
    /// timer for time to deal with? we can have a universal starting one then we can override it after in like different character classes
    /// name? for if we want to pursue name writing in cup and calling, or just to have it here in base class
    /// 
    /// </summary>



    // Start is called before the first frame update
    public virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        exit = CustomerManager.Instance.GetExit();

        if (distThreshold <= 0) distThreshold = 0.5f;
    }


    // Update is called once per frame
    public virtual void Update()
    {
        if (agent.destination == null) return;


        if (currentState == CustomerState.Insit)
        {
            
        }

        if (currentState == CustomerState.Moving)
        {
            if (agent.remainingDistance < distThreshold)
            {
                agent.isStopped = true;
                if (frontofLine == true)
                {
                    //TODO:  Make these things like SetCustomerState(Ordering)
                    currentState = CustomerState.Ordering;
                    UIManager.Instance.ShowCustomerUiOrder(this);
                    return;
                }
                else
                    currentState = CustomerState.Waiting;
            }
        }

      
        if(currentState == CustomerState.Leaving)
        {
            if(agent.remainingDistance < distThreshold)
            {
                Destroy(gameObject);
            }
        }
        //Debug.Log("The customer is " + currentState + " and is going to " + agent.destination);
    }

    //Get customer order
    //UI displays attributes
    public virtual void Order()
    {
        Invoke("CustomerLeave", 60f);
    }


    public virtual void CustomerLeave()
    {
        currentState = CustomerState.Leaving;
        agent.SetDestination(exit.position);
    }

    public void Walkto(Vector3 Spot)
    {
        if (agent.isStopped) agent.isStopped = false;

       
        agent.SetDestination(Spot);

        currentState = CustomerState.Moving;
    }

    public void JustGotHandedCoffee(CoffeeAttributes coffee)
    {
        CustomerReaction(coffee, coffeeAttributes);
    }

    public override void Interact(PlayerStateMachine player)
    {
        //check customer state
        if (currentState == CustomerState.Ordering)
        {
            Order();
            CustomerManager.Instance.Leaveline();
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.interactCustomer);
            interactParticle.Play();
        }

        //if waiting for order
        if (currentState == CustomerState.Insit)
        {
            if (player.HasIngredient())
            {
                //give coffee to customer
                if (player.GetIngredient().CompareTag("CoffeeCup")) {
                    player.GetIngredient().SetIngredientParent(this);
                    JustGotHandedCoffee(this.GetIngredient().GetComponent<CoffeeAttributes>());
                    SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.interactCustomer);
                    interactParticle.Play();
                }
            }
        }

    }

    private void CustomerReaction(CoffeeAttributes coffeeAttributes, CoffeeAttributes customerAttributes)
    {
        int result = 0;
        result += (Mathf.Abs(coffeeAttributes.GetSweetness() - customerAttributes.GetSweetness()) <= 5) ? 1 : -1;
        result += (Mathf.Abs(coffeeAttributes.GetBitterness() - customerAttributes.GetBitterness()) <= 5) ? 1 : -1;
        result += (Mathf.Abs(coffeeAttributes.GetSpiciness() - customerAttributes.GetSpiciness()) <= 5) ? 1 : -1;
        result += (Mathf.Abs(coffeeAttributes.GetTemperature() - customerAttributes.GetTemperature()) <= 5) ? 1 : -1;
        result += (Mathf.Abs(coffeeAttributes.GetStrength() - customerAttributes.GetStrength()) <= 5) ? 1 : -1;

        ScoreTimerManager.Instance.score += result;

        switch (result)
        {
            case 5:

                Perfect();
                ScoreTimerManager.Instance.score += result;
                CustomerLeave();

                break;
            case 4:
            case 3:
            case 2:
            case 1:

                CustomerLeave();

                break;

            case -1:
            case -2:

                Reorder();
                CancelInvoke("CustsomerLeave");
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


}
