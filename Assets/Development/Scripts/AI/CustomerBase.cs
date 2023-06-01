using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CustomerBase : MonoBehaviour
{
    //public Transform target;
    public NavMeshAgent agent;

    public float distThreshold;
    private Vector3 Exit;

    public enum CustomerState
    {
        //add movetosit when sits are available
        Wandering, InLine, Ordering, Moving, Leaving, Insit
    }

    //Initial State
    public CustomerState currentState = CustomerState.Wandering;

    //Waiting In line to order Array
    public GameObject[] Line;
    public int LineIndex;

    //Walking around waiting for coffee Array


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

        Exit = new Vector3(1.5f, 0f, 15f);

        if (distThreshold <= 0) distThreshold = 0.5f;

        {
            //Setting Arrays - Subject to change, how to handle multiple customers
            //if (Line.Length <= 0) Line = GameObject.FindGameObjectsWithTag("Line");
            //

            //Initial State


            //we can add the randomization of meshes or skins here then add more stuff in specific classes?
        }
    }

    // Update is called once per frame
    public virtual void Update()
    {
        /*{
        if(currentState== CustomerState.Wandering)
        {

            
        }

        if (currentState == CustomerState.Ordering)
        {
            Order();

            //add the interact thing here when player interacts with player
            //for now im just gonna put a delay

            Invoke("OrderTaken", 10f);

        }

     /*   if (currentState == CustomerState.WaitingforOrder)
        {
            if (agent.remainingDistance < distThreshold)
            {
                currentState = CustomerState.Insit;

                
            }

            //if (target) agent.SetDestination(target.position);
        } 

        if (currentState == CustomerState.InLine)
        {
            //if (agent.remainingDistance < distThreshold) currentState = CustomerState.Ordering;
        }


       


        if (currentState == CustomerState.Moving)
        {
            if (agent.remainingDistance < distThreshold)
            {
                agent.isStopped = true;
                currentState = CustomerState.InLine;

            }
        }
    }*/

        if (agent.destination == null) return;


        if (currentState == CustomerState.Insit)
        {
            Invoke("CustomerLeave", 18f);
        }

        if (currentState == CustomerState.Moving)
        {
            if (agent.remainingDistance < distThreshold)
            {
                agent.isStopped = true;
                currentState = CustomerState.Wandering;

            }
        }



        Debug.Log("The customer is " + currentState + " and is going to " + agent.destination);


        


    }

    public virtual void OrderTaken()
    {
        //currentState = CustomerState.Waiting;
        //target = Chairs[chairNumber].transform;

        //if (target) agent.SetDestination(target.position);

        //start timer
        //spawn UI for things needed
        //something about recording and comparing if order is correct
    }

    public virtual void Order()
    {
        //UI - customer waiting for Player to hear order
        //Order
        //some other timer? them we could puit that in an invoke then make it leave
    }


    public virtual void CustomerLeave()
    {
        Walkto(Exit);
        StartCoroutine(Death());
    }

    public void Walkto(Vector3 Spot)
    {
        if (agent.isStopped) agent.isStopped = false;

       
        agent.SetDestination(Spot);

        currentState= CustomerState.Moving;
    }

    //willremove
    private IEnumerator Death()
    {
        yield return new WaitForSeconds(5f);

        Destroy(gameObject);
    }


}
