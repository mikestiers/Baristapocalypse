using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Profiling;


public class OrderManager : Singleton<OrderManager>
{
    [SerializeField] private List<OrderInfo> orders = new List<OrderInfo>();
    [SerializeField] private Order orderPrefab;
    public delegate void OrderUpdateHandler(OrderInfo order);
    public event OrderUpdateHandler OnOrderUpdated;
    public BrewingStation[] brewingStations; // Assign in inspector, do not find in active game
    public OrderStats[] orderStats; // Assign in inspector, do not find in active game
    BrewingStation availableBrewingStation;
    OrderStats associatedOrderStats;

    public event EventHandler OnOrderSpawned;
    public event EventHandler OnOrderCompleted;

    public void SpawnOrder(CustomerBase customer)
    {
        OrderInfo newOrder = new OrderInfo(customer);
        customer.SetOrder(newOrder);

        AddOrder(newOrder);
    }

    public void AddOrder(OrderInfo order)
    {
        AddOrderServerRpc(order);
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddOrderServerRpc(OrderInfo order)
    {
        AddOrderClientRpc(order);
    }

    [ClientRpc]
    private void AddOrderClientRpc(OrderInfo order)
    {
        orders.Add(order);
        OnOrderSpawned?.Invoke(this, EventArgs.Empty);
        TryStartOrder();
    }

    public void FinishOrder(OrderInfo order)
    {
        if (order == null) return;
        if (!orders.Contains(order)) return;

        FinishOrderClientRpc(order);
    }

    [ClientRpc]
    private void FinishOrderClientRpc(OrderInfo order)
    {
        orders.Remove(order);
        order.SetOrderState(OrderState.Delivered);
        OnOrderCompleted?.Invoke(this, EventArgs.Empty);
        TryStartOrder();
    }

    public void GetNextOrder()
    {
        TryStartOrder();
    }

    public OrderInfo GetFirstOrder()
    {
        return orders.FirstOrDefault();
    }

    public void TryStartOrder()
    {
        bool availableStationFound = false;

        // We are doing this instead of finding the objects dynamically because
        // we are avoiding having brewingstations reference orderstats due to networking
        // issues
        for (int i = 0; i < brewingStations.Length; i++)
        {
            BrewingStation brewingStation = brewingStations[i];
            if (brewingStation.availableForOrder.Value)
            {
                availableStationFound = true;
                availableBrewingStation = brewingStations[i];
                associatedOrderStats = orderStats[i];
                
                break;
            }
        }

        if (!availableStationFound)
        {
            return;
        }

        if (availableStationFound)
        {
            foreach (OrderInfo order in orders)
            {
                if (order.GetOrderState() == OrderState.Waiting)
                {
                    Debug.Log("GOT THE ORDER " + order.number);
                    StartOrder(order);
                    order.SetOrderState(OrderState.Brewing);
                    return;
                }
            }
        }
    }

    public void StartOrder(OrderInfo order)
    {
        Debug.Log("STARTING ORDER");
        //OnOrderUpdated?.Invoke(order);
        availableBrewingStation.SetOrder(order);
        associatedOrderStats.SetOrderInfo(order);
    }

    public List<OrderInfo> GetOrdersList()
    {
        return orders;
    }
}