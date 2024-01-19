using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderStats : MonoBehaviour
{
    [Header("Customer Order")]
    [SerializeField] private Text customerNumberText;
    [SerializeField] private Text orderResponsibilityText;
    [SerializeField] public OrderStatsSegments temperatureSegments;
    [SerializeField] public OrderStatsSegments sweetnessSegments;
    [SerializeField] public OrderStatsSegments spicinessSegments;
    [SerializeField] public OrderStatsSegments strengthSegments;
    
    [Header("Customer Review")]
    [SerializeField] private GameObject customerReview;
    [SerializeField] private CustomerBase orderOwner;
    [SerializeField] private PlayerController orderResponsibility;

    public CustomerBase GetOrderOwner()
    {
        return orderOwner;
    }

    public PlayerController GetOrderResponsibility()
    {
        return orderResponsibility;
    }

    public void SetOrderResponsibility(PlayerController responsibility)
    {
        orderResponsibility = responsibility;
    }

    public void Initialize(CustomerBase customer)
    {
        orderOwner = customer;
        orderResponsibilityText.text = orderResponsibility.ToString();
        customerNumberText.text = customer.customerNumber.ToString();
        temperatureSegments.targetAttributeValue = customer.coffeeAttributes.GetTemperature();
        sweetnessSegments.targetAttributeValue = customer.coffeeAttributes.GetSweetness();
        spicinessSegments.targetAttributeValue = customer.coffeeAttributes.GetSpiciness();
        strengthSegments.targetAttributeValue = customer.coffeeAttributes.GetStrength();
    }
}
