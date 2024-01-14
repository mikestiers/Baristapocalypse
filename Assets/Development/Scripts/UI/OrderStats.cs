using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderStats : MonoBehaviour
{
    [Header("Customer Order")]
    [SerializeField] private Text customerNumberText;
    [SerializeField] private OrderStatsSegments temperatureSegments;
    [SerializeField] private OrderStatsSegments sweetnessSegments;
    [SerializeField] private OrderStatsSegments spicinessSegments;
    [SerializeField] private OrderStatsSegments strengthSegments;
    
    [Header("Customer Review")]
    [SerializeField] private GameObject customerReview;

    private CustomerBase orderOwner;

    public CustomerBase GetOrderOwner()
    {
        return orderOwner;
    }

    public void Initialize(CustomerBase customer)
    {
        orderOwner = customer;
        customerNumberText.text = customer.customerNumber.ToString();
        temperatureSegments.targetAttributeValue = customer.coffeeAttributes.GetTemperature();
        sweetnessSegments.targetAttributeValue = customer.coffeeAttributes.GetSweetness();
        spicinessSegments.targetAttributeValue = customer.coffeeAttributes.GetSpiciness();
        strengthSegments.targetAttributeValue = customer.coffeeAttributes.GetStrength();
    }
}
