using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CustomerBase : BaseStation
{
    //public Transform target;
    public NavMeshAgent agent;
    private Transform exit;

    public float distThreshold;

    public enum CustomerState
    {
        //add movetosit when sits are available
        Wandering, InLine, Ordering, Moving, Leaving, Insit
    }

    public CoffeeAttributes coffeeAttributes;

    //Initial State
    public CustomerState currentState = CustomerState.Wandering;

    //Waiting In line to order Array
    public GameObject[] Line;
    public int LineIndex;

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
        exit = C_Manager.Instance.GetExit();

        if (distThreshold <= 0) distThreshold = 0.5f;
    }


    // Update is called once per frame
    public virtual void Update()
    {
        if (agent.destination == null) return;


        if (currentState == CustomerState.Insit)
        {
            Invoke("CustomerLeave", 60f);
        }

        if (currentState == CustomerState.Moving)
        {
            if (agent.remainingDistance < distThreshold)
            {
                agent.isStopped = true;
                currentState = CustomerState.Wandering;

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
        //UI - customer waiting for Player to hear order
        //Order
        //some other timer? them we could puit that in an invoke then make it leave
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
        ScoreTimerManager.Instance.GetScoreComparison(coffee, coffeeAttributes);
        CustomerLeave();
    }

    public override void Interact(PlayerStateMachine player)
    {
        //check customer state
        if (currentState == CustomerState.Wandering)
        {
            //Order();
            C_Manager.Instance.Leaveline();
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
                }
            }
        }
    }
}
