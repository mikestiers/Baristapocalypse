using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class OrderManager : Singleton<OrderManager>
{
    [SerializeField] private List<OrderInfo> orders = new List<OrderInfo>();
    [SerializeField] private List<Slider> timers = new List<Slider>();
    [SerializeField] private List<OrderInfo> orderQueue = new List<OrderInfo>();

    public delegate void OrderUpdateHandler(Order order);
    public event OrderUpdateHandler OnOrderUpdated;
    public BrewingStation[] brewingStations; // Assign in inspector, do not find in active game
    public OrderStats[] orderStats; // Assign in inspector, do not find in active game
    BrewingStation availableBrewingStation;
    OrderStats associatedOrderStats;

    // TEST 
    public Slider slider;
    public Transform sliderLocation;

    private void Update()
    {
        if (orders.Any(order => order.GetOrderState() == Order.OrderState.Waiting))
            TryStartOrder();

        if (orderQueue.Count >= 1 && timers.Count >= 1)
            UpdateQueueTimers();

        
     
    }

    public void SpawnOrder(CustomerBase customer)
    {
        Order order = new Order();
        order.Initialize(customer);
    }

    public void UpdateQueueTimers()
    {
       for (int i = 0; i < orderQueue.Count; i++)
       {
            timers[i].value = -(orderQueue[i].customer.customerLeaveTime - orderQueue[i].customer.orderTimer.Value) / orderQueue[i].customer.customerLeaveTime;

            if (timers[i].value >= 0)
            {
                Destroy(timers[i].gameObject);
                timers.Remove(timers[i]);
                return;
            }
       }
    }

    public void AddOrder(OrderInfo order)
    {
        if (orders.Count < 2 && orderQueue.Count <= 0)
        {
            orders.Add(order);
            Debug.Log("WOKIN2");
        }

        else
        {
            orderQueue.Add(order);
            var newTimer = Instantiate(slider, sliderLocation);
            newTimer.value = order.customer.orderTimer.Value;
            timers.Add(newTimer);
            Debug.Log("TESTIN" + newTimer.value);
        }
    }

    public void TakeOrderFromQueue()
    {
        orders.Add(orderQueue[0]);
        orderQueue.Remove(orderQueue[0]);
    }
    public void FinishOrder(OrderInfo order)
    {
        Debug.Log("WOKIN5");
        // orders.Remove(order);
        orders.Remove(order);

        if (orderQueue.Count >= 1)
        {
            GetNextOrder();
        }

        order.SetOrderState(Order.OrderState.Delivered);
        TryStartOrder();
    }

    public void GetNextOrder()
    {
        Destroy(timers[0].gameObject);
        timers.Remove(timers[0]);

        // Sets the next customer in queue's state to "waiting" //
        orderQueue[0].SetOrderState(Order.OrderState.Waiting);

        // Removes finished order and places a new order from OrderQueue list //
        TakeOrderFromQueue();
    }

    public OrderInfo GetFirstOrder()
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
            if (brewingStation.availableForOrder.Value == true)
            {
                availableStationFound = true;
                availableBrewingStation = brewingStations[i];
                associatedOrderStats = orderStats[i];
                break;
            }
        }


        if (!availableStationFound)
        {
            foreach (OrderInfo order in orderQueue)
            {
                if (order.GetOrderState() == Order.OrderState.Waiting)
                {
                    Debug.Log(order.customer.customerName + "is in queue");
                    order.SetOrderState(Order.OrderState.InQueue);
                    return;
                }
            }
        }

        if (availableStationFound)
        {
            foreach (OrderInfo order in orders)
            {
                if (order.GetOrderState() == Order.OrderState.Waiting)
                {
                    Debug.Log("FirstOrder: " + order.customer.customerName);
                    StartOrder(order);
                    order.SetOrderState(Order.OrderState.Brewing);
                    return;
                }
            }
        }
    }

    public void StartOrder(OrderInfo order)
    {
        //OnOrderUpdated?.Invoke(order);
        availableBrewingStation.SetOrder(order);
        associatedOrderStats.SetOrderInfo(order);
    }

    public OrderInfo GetOrderFromListByIndex(int index)
    {
        return orders[index];
    }
}