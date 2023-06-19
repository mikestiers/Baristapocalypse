using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayAttributes : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro;
    private CoffeeAttributes coffeeAttributes;
    // Start is called before the first frame update
    void Start()
    {
        coffeeAttributes = GetComponentInParent<CoffeeAttributes>();

        textMeshPro.text = "Bitterness: " + coffeeAttributes.GetBitterness() + "\n" +
            "Sweetness: " + coffeeAttributes.GetSweetness() + "\n" +
            "Strength: " + coffeeAttributes.GetStrength() + "\n" +
            "Temperature: " + coffeeAttributes.GetTemperature() + "\n" +
            "Spiciness: " + coffeeAttributes.GetSpiciness();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
