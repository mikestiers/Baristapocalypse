using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class DisplayAttributes : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro;
    private CoffeeAttributes coffeeAttributes;
    [SerializeField] private CustomerBase customerBase;
    // Start is called before the first frame update
    void Start()
    {
        coffeeAttributes = GetComponentInParent<CoffeeAttributes>();

        //textMeshPro.text = "Bitterness: " + coffeeAttributes.GetBitterness() + "\n" +
        //    "Sweetness: " + coffeeAttributes.GetSweetness() + "\n" +
        //    "Strength: " + coffeeAttributes.GetStrength() + "\n" +
        //    "Temperature: " + coffeeAttributes.GetTemperature() + "\n" +
        //    "Spiciness: " + coffeeAttributes.GetSpiciness();
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
        // do to other attributes
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
    }
}
