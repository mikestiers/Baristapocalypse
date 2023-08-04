using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;


public class C_LineQueuing
{
    private List<Vector3> positionList;
    private List<CustomerBase> customerList;
    //public event EventHandler OnCustomerAdded;
    //public event EventHandler OnCustomerArrivedAtFrontOfQueue;
    

    //sorry if it seam all over the place -> no worries madood its all good
    public C_LineQueuing(List<Vector3> positionList)
    {
        this.positionList = positionList;
        customerList = new List<CustomerBase>();
    }

    public bool CanAddCustomer()
    {
        return customerList.Count < positionList.Count;
    }

    public void AddCustomer(CustomerBase customer)
    {
        customerList.Add(customer);

        customer.Walkto(positionList[customerList.IndexOf(customer)]);

        if (customerList.IndexOf(customer) == 0) customer.frontofLine = true;
    }


    //Gets the customer at front of queue
    public CustomerBase GetFirstInQueue()
    {
        if (customerList.Count == 0)
        {
            return null;
        }
        else
        {
            CustomerBase customer = customerList[0];
            customerList.RemoveAt(0);
            RelocateAllCustomer();
            return customer;
        }
    }


    //Moves Customer Through the line
   private void RelocateAllCustomer()
    {
        for (int i = 0; i < customerList.Count; i++)
        {
            customerList[i].Walkto(positionList[i]);
            if (i == 0) customerList[i].frontofLine = true;
        }
    }

    public void OrderTaken(CustomerBase customer)
    {

    } 
}
