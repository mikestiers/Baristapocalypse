using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderStats : MonoBehaviour
{
    [Header("Customer Order")]
    [SerializeField] private Slider temperatureSlider;
    [SerializeField] private Slider sweetnessSlider;
    [SerializeField] private Slider spicinessSlider;
    [SerializeField] private Slider strengthSlider;
    [SerializeField] private Text customerNumberText;
    
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
        temperatureSlider.value = customer.coffeeAttributes.GetTemperature() * .10f;
        sweetnessSlider.value = customer.coffeeAttributes.GetSweetness() * .10f;
        spicinessSlider.value = customer.coffeeAttributes.GetSpiciness() * .10f;
        strengthSlider.value = customer.coffeeAttributes.GetStrength() * .10f;
        customerNumberText.text = customer.customerNumber.ToString();
    }
}
