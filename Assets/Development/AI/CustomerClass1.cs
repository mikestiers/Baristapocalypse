using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerClass1 : CustomerBase
{
    public enum CustomerState
    {
        //add movetosit when sits are available
        InLine, Ordering, Waiting, Leaving
    }

    //Initial State
    public CustomerState currentState = CustomerState.InLine;

    //Waiting In line to order Array
    public GameObject[] Line;
    public int LineIndex;
    //Walking around waiting for coffee Array
    public GameObject[] Waypoint;
    public int WaypointIndex;

    /// <summary>
    ///  For the arrays im thinking of moving them to some other script like a level script or something to be actually be 
    ///  accesible by all customer prefabs on what Index they currently are in 
    ///  (im thinking of copying something like the queue DT with had from programming fundi to the line array)
    ///  we can also add sits as an array instead of waypoints
    /// </summary>


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        //Setting Arrays - Subject to change, how to handle multiple customers
        if (Line.Length <= 0) Line = GameObject.FindGameObjectsWithTag("Line");
        if (Waypoint.Length <= 0) Waypoint = GameObject.FindGameObjectsWithTag("Waypoint");

        //Initial State
        if (currentState == CustomerState.InLine)
        {
            target = Line[LineIndex].transform;
            if (target) agent.SetDestination(target.position);
        }

    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (currentState == CustomerState.Ordering)
        {
            Order();

            //add the interact thing here when player interacts with player
            //for now im just gonna put a delay
            Invoke("OrderTaken", 10f);

        }

        if (currentState == CustomerState.Waiting)
        {
            if (agent.remainingDistance < distThreshold)
            {
                WaypointIndex++;
                WaypointIndex %= Waypoint.Length;
                target = Waypoint[WaypointIndex].transform;

                if (target) agent.SetDestination(target.position);
            }

        }

        if (currentState == CustomerState.InLine)
        {
            if (agent.remainingDistance < distThreshold) currentState = CustomerState.Ordering;
        }

        Debug.Log("The customer is " + currentState +  " and is going to " + target.gameObject);
    }

    public virtual void OrderTaken()
    {
        currentState = CustomerState.Waiting;
        target = Waypoint[WaypointIndex].transform;

        if (target) agent.SetDestination(target.position);

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
   
}
