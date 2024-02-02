using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OrderManager : Singleton<OrderManager>
{
    public List<Order> orders = new List<Order>();
    public delegate void OrderUpdateHandler(Order order);
    public event OrderUpdateHandler OnOrderUpdated;
    private BrewingStation[] brewingStations;

    private void Update()
    {
        if (orders.Any(order => order.State == Order.OrderState.Waiting))
            TryStartOrder();

        //if (orders.Count > 0)
        //    foreach (Order order in orders)
        //        if (order.State == Order.OrderState.Waiting)
        //        {
        //            TryStartOrder();
        //            break;
        //        }
    }

    public void AddOrder(Order order)
    {
        orders.Add(order);
        TryStartOrder();
    }

    public void RemoveOrder(Order order)
    {
        orders.Remove(order);
        TryStartOrder();
    }

    public void GetNextOrder()
    {
        TryStartOrder();
    }

    public void TryStartOrder()
    {
        bool availableStationFound = false;
        brewingStations = FindObjectsOfType<BrewingStation>();
        foreach (BrewingStation brewingStation in brewingStations)
        {
            Debug.Log($"availablefororder: {brewingStation.availableForOrder}");
            if (brewingStation.availableForOrder)
            {
                availableStationFound = true;
                break;
            }
        }

        if (!availableStationFound)
        {
            Debug.Log("All brewing stations are busy");
            return;
        }

        if (availableStationFound)
        {
            foreach (Order order in orders)
            {
                if (order.State == Order.OrderState.Waiting)
                {
                    Debug.Log("FirstOrder: " + order.customer.customerName);
                    StartOrder(order);
                    order.State = Order.OrderState.Brewing;
                    return;
                }
            }
        }
    }

    public void StartOrder(Order order)
    {
        OnOrderUpdated?.Invoke(order);
    }
}