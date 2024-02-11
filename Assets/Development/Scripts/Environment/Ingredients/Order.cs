using System;
using System.Collections.Generic;
using UnityEngine;

public class Order: MonoBehaviour
{
    public CustomerBase customer;
    private int number;
    private string Name;
    private List<IngredientSO> ingredientList;
    private Tuple<int, int, int, int> attributes;
    public CoffeeAttributes coffeeAttributes;
    private BrewingStation assignedBrewingStation;
    public OrderState State { get; set; }

    public enum OrderState
    {
        Waiting,
        Brewing,
        BeingDelivered,
        Delivered
    }

    public void Initialize(CustomerBase customerOrder)
    {
        customer = customerOrder; // probably shouldn't do this.  customer.order.customer.order.customer.order..
        Name = customer.customerName;
        number = customer.customerNumber;
        coffeeAttributes = customer.coffeeAttributes;
        State = OrderState.Waiting;
        OrderManager.Instance.AddOrder(this);
    }
}
