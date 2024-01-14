using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayAttributes : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI customerSentence;
    [SerializeField] private Canvas customerNumberCanvas;
    [SerializeField] private Text customerNumberText;
    [SerializeField] private Text customerNameText;
    [SerializeField] private CustomerBase customerBase;
    private CoffeeAttributes coffeeAttributes;

    public void Start()
    {
        coffeeAttributes = GetComponentInParent<CoffeeAttributes>();
        if (customerSentence)
        {
            customerSentence.text = GetCustomerDialogue();
        }
        
    }

    // Build the sentence describing the drink for the customer's order dialogue
    private string GetCustomerDialogue()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("I want");
        sb.Append(GetPrefix(coffeeAttributes.GetBitterness()) + "bitterness,");
        sb.Append(GetPrefix(coffeeAttributes.GetSweetness()) + "sweetness,");
        sb.Append(GetPrefix(coffeeAttributes.GetStrength()) + "strength,");
        sb.Append(GetPrefix(coffeeAttributes.GetTemperature()) + "temperature,");
        sb.Append(GetPrefix(coffeeAttributes.GetSpiciness()) + "spiciness.");
        return sb.ToString();
    }

    // Translating drink attribute values to relevant words
    private string GetPrefix(int i)
    {
        switch (i)
        {
            case 0:
                return " no ";
            case 1:
            case 2:
                return " subtle ";
            case 3:
            case 4:
                return " mild ";
            case 5:
            case 6:
                return " strong ";
            case 7:
            case 8:
                return " powerful ";
            case 9:
            case 10:
                return " potent ";
        }
        return "";
    }
}