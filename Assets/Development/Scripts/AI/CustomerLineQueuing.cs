using System.Collections.Generic;
using UnityEngine;

public class CustomerLineQueuing
{
    private List<Vector3> positionList;
    private List<CustomerBase> customerList;

    public CustomerLineQueuing(List<Vector3> positionList)
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
        if(customerList.IndexOf(customer) == 0) customer.frontofLine = true;
        customer.inLine.Value = true;
        customer.currentPosInLine = customerList.IndexOf(customer);
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
            customer.currentPosInLine = -1;
            RelocateAllCustomer(0);
            return customer;
        }
    }

    public CustomerBase RemoveFromQueue(CustomerBase customer)
    {
        if (customerList.Count == 0)
            return null;
        else
        {
            Debug.Log($"Thrown customer {customer.customerNumber}");
            for (int i = 0; i < customerList.Count; i++)
            {
                CustomerBase customerBase = customerList[i];
                Debug.Log($"Thrown customer index {i}");
                customerList.RemoveAt(i);
            }
            return customer;
        }
    }

    public void RemoveCustomerInPos(int LinePos)
    {
        if (customerList.Count == 0) return;
        else
        {
            customerList.RemoveAt(LinePos);
            RelocateAllCustomer(LinePos);
        }
    }


    //Moves Customer Through the line
   private void RelocateAllCustomer(int startpos)
    {
        for (int i = startpos; i < customerList.Count; i++)
        {
            customerList[i].Walkto(positionList[i]);
            if(i == 0) customerList[i].frontofLine = true;
            customerList[i].currentPosInLine = i;
        }
    }
    
    public int GetLineCount()
    {
        int TotalCount = customerList.Count;
        
        return TotalCount;
    }
}
