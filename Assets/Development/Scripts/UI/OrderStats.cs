using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderStats : MonoBehaviour
{
    [Header("Customer Order")]
    [SerializeField] public Text customerNumberText;
    [SerializeField] public OrderStatsSegments temperatureSegments;
    [SerializeField] public OrderStatsSegments sweetnessSegments;
    [SerializeField] public OrderStatsSegments spicinessSegments;
    [SerializeField] public OrderStatsSegments strengthSegments;
    
    [Header("Customer Review")]
    [SerializeField] private GameObject customerReview;
    [SerializeField] private CustomerBase orderOwner;

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

    public void SetTemperature()
    {
        temperatureSegments.UpdateSegmentColors(temperatureSegments.cumulativeIngredientsValue);
    }

    public void SetSweetness()
    {
        sweetnessSegments.UpdateSegmentColors(sweetnessSegments.cumulativeIngredientsValue);
    }

    public void SetSpiciness()
    {
        spicinessSegments.UpdateSegmentColors(spicinessSegments.cumulativeIngredientsValue);
    }

    public void SetStrength()
    {
        strengthSegments.UpdateSegmentColors(strengthSegments.cumulativeIngredientsValue);
    }

    public void SetPotentialTemperature()
    {
        temperatureSegments.UpdateSegmentColors(temperatureSegments.potentialIngredientValue);
    }

    public void SetPotentialSweetness()
    {
        sweetnessSegments.UpdateSegmentColors(sweetnessSegments.potentialIngredientValue);
    }

    public void SetPotentialSpiciness()
    {
        spicinessSegments.UpdateSegmentColors(spicinessSegments.potentialIngredientValue);
    }

    public void SetPotentialStrength()
    {
        strengthSegments.UpdateSegmentColors(strengthSegments.potentialIngredientValue);
    }
}
