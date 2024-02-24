using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Unity.Collections;


public enum OrderState
{
    Waiting,
    Brewing,
    BeingDelivered,
    Delivered
}

public struct Order : INetworkSerializable, IEquatable<Order>
{
    //public int number;
    //public FixedString32Bytes orderName;
    public int coffeeAttributesSweetness;
    public int coffeeAttributesTemperature;
    public int coffeeAttributesSpiciness;
    public int coffeeAttributesStrength;
    public OrderState orderState;


    public Order(CustomerBase customerOrder)
    {
       //orderName = new FixedString32Bytes(customerOrder.customerName);
        //number = customerOrder.customerNumber.Value;
        coffeeAttributesSweetness = customerOrder.coffeeAttributes.GetSweetness();
        coffeeAttributesTemperature = customerOrder.coffeeAttributes.GetTemperature();
        coffeeAttributesSpiciness = customerOrder.coffeeAttributes.GetSpiciness();
        coffeeAttributesStrength = customerOrder.coffeeAttributes.GetStrength();
        orderState = OrderState.Waiting;
    }

    public void SetOrderState(OrderState _orderState)
    {
        orderState = _orderState;
    }

    public OrderState GetOrderState()
    {
        return orderState;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        //serializer.SerializeValue(ref number);
       // serializer.SerializeValue(ref orderName);
        serializer.SerializeValue(ref coffeeAttributesSpiciness);
        serializer.SerializeValue(ref coffeeAttributesTemperature);
        serializer.SerializeValue(ref coffeeAttributesSweetness);
        serializer.SerializeValue(ref coffeeAttributesStrength);
        serializer.SerializeValue(ref orderState);
    }

    public bool Equals(Order other)
    {
        return false;
    }
}
