using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class OrderManager : Singleton<OrderManager>
{
    public List<Slider> timers = new List<Slider>();
    public List<Order> orders = new List<Order>();
    public List<Order> orderQueue = new List<Order>();
    public delegate void OrderUpdateHandler(Order order);
    public event OrderUpdateHandler OnOrderUpdated;
    public BrewingStation[] brewingStations; // Assign in inspector, do not find in active game
    public OrderStats[] orderStats; // Assign in inspector, do not find in active game
    BrewingStation availableBrewingStation;
    OrderStats associatedOrderStats;

    // TEST//
    public int amountOfOrders;
    public Slider slider;
    public Transform sliderLocation;

    private void Start()
    {
        amountOfOrders = 0;
    }

    private void Update()
    {
        if (orders.Any(order => order.State == Order.OrderState.Waiting))
            TryStartOrder();

        if (orderQueue.Count >= 1)
        {
            for (int i = 0; i < timers.Count; i++)
                timers[i].value = (orderQueue[i].customer.customerLeaveTime - orderQueue[i].customer.orderTimer.Value) / orderQueue[i].customer.customerLeaveTime;
        }

    }

    public void AddOrder(Order order)
    {
        if (amountOfOrders < 2)
            orders.Add(order);

        else
        {
            orderQueue.Add(order);
            var newTimer = Instantiate(slider, sliderLocation);
            newTimer.value = (order.customer.customerLeaveTime - order.customer.orderTimer.Value) / order.customer.customerLeaveTime;
            timers.Add(newTimer);
            Debug.Log("TESTIN" + newTimer.value);

            //ordersInQueue++;
        }

        TryStartOrder();
        amountOfOrders++;
    }

    public void TakeOrderFromQueue()
    {
        orders.Add(orderQueue[0]);
        orderQueue.Remove(orderQueue[0]);
    }

    public void FinishOrder(Order order)
    {
        Debug.Log("HELYO");

        orders.Remove(order);

        if (orderQueue.Count >= 1)
        {
            GetNextOrder();
            TryStartOrder();
        }
        
        amountOfOrders--;
    }

    public void GetNextOrder()
    {
        Destroy(timers[0].gameObject);
        timers.Remove(timers[0]);

        // Sets the next customer in queue's state to "waiting" //
        orderQueue[0].State = Order.OrderState.Waiting;

        // Removes finished order and places a new order from OrderQueue list //
        TakeOrderFromQueue();
        TryStartOrder();
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
            Debug.Log($"availablefororder: {brewingStation.availableForOrder}");
            if (brewingStation.availableForOrder)
            {
                availableStationFound = true;
                availableBrewingStation = brewingStations[i];
                associatedOrderStats = orderStats[i];
                Debug.Log($"Element number (index) of available brewing station: {i}");
                break;
            }
        }


        if (!availableStationFound)
        {
            foreach (Order order in orderQueue)
            {
                if (order.State == Order.OrderState.Waiting)
                {
                    Debug.Log(order.customer.customerName + "is in queue");
                    order.State = Order.OrderState.InQueue;
                    return;
                }
            }
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
        //OnOrderUpdated?.Invoke(order);
        availableBrewingStation.SetOrder(order);
        associatedOrderStats.SetOrderInfo(order);
    }
}