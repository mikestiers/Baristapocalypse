using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CustomerBarFloor
{
    readonly private List<Chair> chairList;
    public CustomerBarFloor(GameObject[] chairLocations) 
    {
        chairList = new List<Chair>();
        foreach (GameObject chairPosition in chairLocations) 
        {
            chairList.Add(new Chair() { chairPosition = chairPosition.transform.position });
        }
    }

    public void TrySendToChair(CustomerBase customer)
    {
        Chair emptyChair = GetEmptyChair();
        if (emptyChair != null) 
        {
            emptyChair.SetCustomer(customer);
            customer.Walkto(emptyChair.GetPosition());
            customer.SetCustomerStateServerRpc(CustomerBase.CustomerState.Insit);

        }
    }

    private Chair GetEmptyChair()
    {
        List<Chair> emptyChairs= new();

        foreach (Chair chair in chairList)
        {
            if(chair.IsEmpty())
            {
                emptyChairs.Add(chair);
            }
        }

        if (emptyChairs.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, emptyChairs.Count);
            return emptyChairs[randomIndex];
        }

        return null;
    }

    public int GetTotalCustomersOnFloor()
    {
        int chairsOccupied = chairList.Count;

        foreach (Chair chair in chairList)
        {
            if (chair.IsEmpty())
            {
                chairsOccupied =- 1;
            }
        }

        return chairsOccupied;
    }

    private class Chair
    {
        public Vector3 chairPosition;
        public CustomerBase customerInThisChair;
        public bool IsEmpty()
        {
            return customerInThisChair == null;
        }
        public void SetCustomer(CustomerBase customer)
        {
            this.customerInThisChair= customer;
        }

        public Vector3 GetPosition()
        {
            return chairPosition;
        }

        public void ClearCustomer()
        {
            customerInThisChair = null;
        }
    }
}
