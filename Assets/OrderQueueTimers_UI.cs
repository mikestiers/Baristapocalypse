using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderQueueTimers_UI : MonoBehaviour
{
    public CustomerBase customer;
    public Transform queuePanel;

    public List<Slider> queueTimers;
    public Slider queueTimerSlider;

    public float test;

    public bool queueActive;

    public int listInsertID;

    // Start is called before the first frame update
    public void Awake()
    {
        listInsertID = 0;

    }

    public void SetOrderTime(CustomerBase customerOrder)
    {

        customer = customerOrder;
        Debug.Log("queue" + listInsertID);
        Debug.Log("eueuueu" + customer.orderTimer.Value);
        queueTimers[listInsertID].value = (customer.customerLeaveTime - customer.orderTimer.Value) / customer.customerLeaveTime;
        Debug.Log("WORK" + queueTimers[listInsertID].value);
        listInsertID++;
        //queueTimers.a
        
        //test = customerOrder.orderTimer.Value;
        //Debug.Log("hello" + customerOrder.orderTimer.Value);

    }

}
