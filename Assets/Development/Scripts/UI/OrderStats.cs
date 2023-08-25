using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderStats : MonoBehaviour
{
    [Header("Customer Order")]
    [SerializeField] private Slider bitternessSlider;
    [SerializeField] private Slider sweetnessSlider;
    [SerializeField] private Slider strengthSlider;
    [SerializeField] private Slider temperatureSlider;
    [SerializeField] private Slider spicinessSlider;
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
        bitternessSlider.value = customer.coffeeAttributes.GetBitterness() * .10f;
        sweetnessSlider.value = customer.coffeeAttributes.GetSweetness() * .10f;
        strengthSlider.value = customer.coffeeAttributes.GetStrength() * .10f;
        temperatureSlider.value = customer.coffeeAttributes.GetTemperature() * .10f;
        spicinessSlider.value = customer.coffeeAttributes.GetSpiciness() * .10f;
        customerNumberText.text = customer.customerNumber.ToString();
    }

    // Return the customer review object to UIManager so that it can be enabled
    public GameObject GetCustomerReview()
    {
        return customerReview;
    }
}
