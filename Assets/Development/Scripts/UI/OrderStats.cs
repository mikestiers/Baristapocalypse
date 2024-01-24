using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class OrderStats : MonoBehaviour
{
    [Header("Customer Order")]
    [SerializeField] public GameObject customerInfoRoot;
    [SerializeField] public Text customerNumberText;
    [SerializeField] public Text customerNameText;
    [SerializeField] public Text brewingStationText;
    [SerializeField] public Slider orderTimer;
    [SerializeField] public OrderStatsSegments temperatureSegments;
    [SerializeField] public OrderStatsSegments sweetnessSegments;
    [SerializeField] public OrderStatsSegments spicinessSegments;
    [SerializeField] public OrderStatsSegments strengthSegments;
    [SerializeField] public GameObject selectedByPlayerImage;
    [SerializeField] public List<PlayerController> currentPlayers;

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

    // Fade of unfade the order stats
    public void SetInactive(bool isInactive)
    {
        Image[] images = GetComponentsInChildren<Image>(true); // The 'true' parameter includes inactive GameObjects
        foreach (Image image in images)
        {
            Color imageColor = image.GetComponent<Image>().color;
            if (isInactive)
            {
                imageColor.a = 0.2f;
                selectedByPlayerImage.SetActive(false);
            }
            else
            {
                imageColor.a = 1.0f;
                //selectedByPlayerImage.SetActive(true);
            }
            image.color = imageColor;
        }
    }

    public List<PlayerController> GetActivePlayers()
    {
        return currentPlayers;
    }

    public void SetActivePlayer(PlayerController player)
    {
        if (currentPlayers.Count > 0)
        {
            currentPlayers.Add(player);
            selectedByPlayerImage.SetActive(true);
        }
    }

    public void RemoveActivePlayer(PlayerController player)
    {
        currentPlayers.Remove(player);
        if (currentPlayers.Count == 0)
        {
            selectedByPlayerImage.SetActive(false);
        }
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
