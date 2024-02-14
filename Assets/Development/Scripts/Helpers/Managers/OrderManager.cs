using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Profiling;


public class OrderManager : Singleton<OrderManager>
{
    [SerializeField] private List<Order> orders = new List<Order>();
    [SerializeField] private Order orderPrefab;
    public delegate void OrderUpdateHandler(Order order);
    public event OrderUpdateHandler OnOrderUpdated;
    public BrewingStation[] brewingStations; // Assign in inspector, do not find in active game
    public OrderStats[] orderStats; // Assign in inspector, do not find in active game
    BrewingStation availableBrewingStation;
    OrderStats associatedOrderStats;

    private void Update()
    {
        if (orders.Any(order => order.GetOrderState() == Order.OrderState.Waiting))
            TryStartOrder();
    }

    public void SpawnOrder(CustomerBase customer)
    {
        if (IsServer)
        {
            Order newOrder = Instantiate(orderPrefab);

            NetworkObject newOrderNetworkObject = newOrder.GetComponent<NetworkObject>();
            newOrderNetworkObject.Spawn(true);

            newOrder.Initialize(customer);
            customer.SetOrder(newOrder);

            AddOrder(newOrder);
        }
    }

    public void AddOrder(Order order)
    {
        orders.Add(order);
        TryStartOrder();
    }

    public void FinishOrder(Order order)
    {
        // orders.Remove(order);
        order.SetOrderState(Order.OrderState.Delivered);
        TryStartOrder();
    }

    public void GetNextOrder()
    {
        TryStartOrder();
    }

    public Order GetFirstOrder()
    {
        return orders.FirstOrDefault();
    }

    public void TryStartOrder()
    {
        TryStartOrderServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void TryStartOrderServerRpc()
    {
        TryStartOrderClientRpc();
    }

    [ClientRpc]
    private void TryStartOrderClientRpc()
    {
        bool availableStationFound = false;

        // We are doing this instead of finding the objects dynamically because
        // we are avoiding having brewingstations reference orderstats due to networking
        // issues
        for (int i = 0; i < brewingStations.Length; i++)
        {
            BrewingStation brewingStation = brewingStations[i];
            if (brewingStation.availableForOrder)
            {
                availableStationFound = true;
                availableBrewingStation = brewingStations[i];
                associatedOrderStats = orderStats[i];
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
                Debug.Log("Order " + order);
                if (order.GetOrderState() == Order.OrderState.Waiting)
                {
                    StartOrder(order);
                    order.SetOrderState(Order.OrderState.Brewing);
                    return;
                }
            }
        }
    }

    public void StartOrder(Order order)
    {
        //OnOrderUpdated?.Invoke(order);
        availableBrewingStation.SetOrder(order);
        associatedOrderStats.SetOrderInfo(order);
    }
}