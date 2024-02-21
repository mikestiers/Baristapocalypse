using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Order: NetworkBehaviour
{
    public CustomerBase customer;
    private int number;
    private string Name;
    private List<IngredientSO> ingredientList;
    private Tuple<int, int, int, int> attributes;
    public CoffeeAttributes coffeeAttributes;
    private BrewingStation assignedBrewingStation;
    public NetworkVariable<OrderState> orderState = new NetworkVariable<OrderState>(OrderState.Waiting);

    public enum OrderState
    {
        Waiting,
        Brewing,
        BeingDelivered,
        Delivered,
        InQueue,
        OutOfTime
    }

    public void Initialize(CustomerBase customerOrder)
    {
        customer = customerOrder; // probably shouldn't do this.  customer.order.customer.order.customer.order..
        Name = customer.customerName;
        number = customer.customerNumber;
        coffeeAttributes = customer.coffeeAttributes;
        orderState.Value = OrderState.Waiting;
        //OrderManager.Instance.AddOrder(this);
    }

    public void SetOrderState(OrderState _orderState)
    {
        orderState.Value = _orderState;
    }

    public OrderState GetOrderState()
    {
        return orderState.Value;
    }
}
