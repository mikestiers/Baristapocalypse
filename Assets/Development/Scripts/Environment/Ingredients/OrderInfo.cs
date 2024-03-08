using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Unity.Collections;
using Unity.VisualScripting;

[System.Serializable]
public class OrderInfo : INetworkSerializable, IEquatable<OrderInfo>
{
    public FixedString32Bytes orderName;
    public int number;
    public float orderTimer;
    public float customerLeaveTime;
    public int coffeeAttributesSweetness;
    public int coffeeAttributesTemperature;
    public int coffeeAttributesSpiciness;
    public int coffeeAttributesStrength;
    public OrderState orderState;

    public OrderInfo() { }

    public OrderInfo(CustomerBase customerOrder)
    {
        orderName = new FixedString32Bytes(customerOrder.customerName.Value);
        number = customerOrder.customerNumber.Value;
        orderTimer = customerOrder.orderTimer;
        customerLeaveTime = customerOrder.customerLeaveTime;
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
        serializer.SerializeValue(ref orderName);
        serializer.SerializeValue(ref number);
        serializer.SerializeValue(ref orderTimer);
        serializer.SerializeValue(ref customerLeaveTime);
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
