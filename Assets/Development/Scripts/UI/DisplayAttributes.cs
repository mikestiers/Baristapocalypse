using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayAttributes : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private GameObject hudOrder;
    [SerializeField] private Slider bitternessSlider;
    [SerializeField] private Slider sweetnessSlider;
    [SerializeField] private Slider strengthSlider;
    [SerializeField] private Slider temperatureSlider;
    [SerializeField] private Slider spicinessSlider;
    private CoffeeAttributes coffeeAttributes;
    [SerializeField] private CustomerBase customerBase;
    // Start is called before the first frame update
    void Start()
    {
        coffeeAttributes = GetComponentInParent<CoffeeAttributes>();
        Debug.Log("Line Index for customer " + customerBase.name + " at index " + customerBase.LineIndex);
        //textMeshPro.text = "Bitterness: " + coffeeAttributes.GetBitterness() + "\n" +
        //    "Sweetness: " + coffeeAttributes.GetSweetness() + "\n" +
        //    "Strength: " + coffeeAttributes.GetStrength() + "\n" +
        //    "Temperature: " + coffeeAttributes.GetTemperature() + "\n" +
        //    "Spiciness: " + coffeeAttributes.GetSpiciness();
        hudOrder.SetActive(false);
        bitternessSlider.value = coffeeAttributes.GetBitterness() * .10f;
        sweetnessSlider.value = coffeeAttributes.GetSweetness() * .10f;
        strengthSlider.value = coffeeAttributes.GetStrength() * .10f;
        temperatureSlider.value = coffeeAttributes.GetTemperature() * .10f;
        spicinessSlider.value = coffeeAttributes.GetSpiciness() * .10f;

        textMeshPro.enabled = false;
        textMeshPro.text = GetCustomerDialogue();
    }

    private string GetCustomerDialogue()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("I want ");
        sb.Append(GetPrefix(coffeeAttributes.GetBitterness()) + "bitterness ");
        sb.Append(GetPrefix(coffeeAttributes.GetSweetness()) + "sweetness ");
        sb.Append(GetPrefix(coffeeAttributes.GetStrength()) + "strength ");
        sb.Append(GetPrefix(coffeeAttributes.GetTemperature()) + "temperature ");
        sb.Append(GetPrefix(coffeeAttributes.GetSpiciness()) + "spiciness.");
        return sb.ToString();
    }

    private string GetPrefix(int i)
    {
        switch(i)
        {
            case 0:
                return "no ";
            case 1: case 2:
                return " subtle ";
            case 3: case 4:
                return " mild ";
            case 5: case 6:
                return " strong ";
            case 7: case 8:
                return "powerfull ";
            case 9: case 10:
                return " potent ";
        }
        return "";
    }

    // Update is called once per frame
    void Update()
    {
        textMeshPro.enabled = customerBase.currentState == CustomerBase.CustomerState.Wandering ? true : false;
        hudOrder.SetActive(customerBase.currentState == CustomerBase.CustomerState.Wandering ? true : false);

    }
}
