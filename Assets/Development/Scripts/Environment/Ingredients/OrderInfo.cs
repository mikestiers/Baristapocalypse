using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Unity.Collections;
using Unity.VisualScripting;

[System.Serializable]
public class OrderInfo : INetworkSerializable, IEquatable<OrderInfo>
{
    public CustomerBase customer;
    public int number;
    public FixedString32Bytes orderName;
    public int coffeeAttributesSweetness;
    public int coffeeAttributesTemperature;
    public int coffeeAttributesSpiciness;
    public int coffeeAttributesStrength;
    public Order.OrderState orderState;
    public OrderInfo() { }

    public OrderInfo(CustomerBase customerOrder)
    {
        orderName = new FixedString32Bytes(customerOrder.customerName);
        number = customerOrder.customerNumber;
        coffeeAttributesSweetness = customerOrder.coffeeAttributes.GetSweetness();
        coffeeAttributesTemperature = customerOrder.coffeeAttributes.GetTemperature();
        coffeeAttributesSpiciness = customerOrder.coffeeAttributes.GetSpiciness();
        coffeeAttributesStrength = customerOrder.coffeeAttributes.GetStrength();
        orderState = Order.OrderState.Waiting;
    }

    public void SetOrderState(Order.OrderState _orderState)
    {
        orderState = _orderState;
    }

    public Order.OrderState GetOrderState()
    {
        return orderState;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref number);
        serializer.SerializeValue(ref orderName);
        serializer.SerializeValue(ref coffeeAttributesSpiciness);
        serializer.SerializeValue(ref coffeeAttributesTemperature);
        serializer.SerializeValue(ref coffeeAttributesSweetness);
        serializer.SerializeValue(ref coffeeAttributesStrength);
        serializer.SerializeValue(ref orderState);
    }

    public bool Equals(OrderInfo other)
    {
        if ((number == other.number)) //&&(orderName == other.orderName))
        {
            return true;
        }
        return false;
    }

}
