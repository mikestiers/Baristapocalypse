using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Lobbies.Models;
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
    [SerializeField] public Image resetMachineImage;
    [SerializeField] public bool orderInProgress { get; set; }
    [SerializeField] private CustomerBase orderOwner;
    [SerializeField] public BrewingStation brewingStation;

    private void OnEnable()
    {
        //OrderManager.Instance.OnOrderUpdated += SetOrderInfo;
        brewingStation.OnBrewingEmpty += OrderCompleted;
        brewingStation.OnBrewingDone += OrderCompleted;
    }

    private void OnDisable()
    {
        //OrderManager.Instance.OnOrderUpdated -= SetOrderInfo;
        brewingStation.OnBrewingEmpty -= OrderCompleted;
        brewingStation.OnBrewingDone -= OrderCompleted;
    }

    private void Start()
    {
        orderInProgress = false;
        OrderInProgress();
    }

    private void OrderCompleted(object sender, EventArgs e)
    {
        orderInProgress = false;
        OrderInProgress();
    }

    public void SetOrderInfo(Order order)
    {
        SetOrderOwner(order.customer);
        customerInfoRoot.SetActive(true);
        customerNumberText.text = order.customer.customerNumber.ToString();
        customerNameText.text = order.customer.customerName;
        orderTimer.value = order.customer.orderTimer.Value;
        temperatureSegments.targetAttributeValue = order.customer.coffeeAttributes.GetTemperature();
        sweetnessSegments.targetAttributeValue = order.customer.coffeeAttributes.GetSweetness();
        spicinessSegments.targetAttributeValue = order.customer.coffeeAttributes.GetSpiciness();
        strengthSegments.targetAttributeValue = order.customer.coffeeAttributes.GetStrength();
        ResetPotential();
        orderInProgress = true;
        OrderInProgress();
    }

    private void Update()
    {
        if (orderInProgress == true)
        {
            UpdateTimer();
        }
    }

    public CustomerBase GetOrderOwner()
    {
        return orderOwner;
    }

    public void SetOrderOwner(CustomerBase customer)
    {
        orderOwner = customer;
    }

    // Fade or unfade the order stats
    public void OrderInProgress()
    {
        Image[] images = GetComponentsInChildren<Image>(); // not including 'true' parameter because it includes inactive objects and the segments are not active by default
        foreach (Image image in images)
        {
            Color imageColor = image.GetComponent<Image>().color;
            if (!orderInProgress)
            {
                imageColor.a = 0.2f;
                selectedByPlayerImage.SetActive(false);
                customerInfoRoot.SetActive(false);
                temperatureSegments.targetAttributeValue = 0;
                sweetnessSegments.targetAttributeValue = 0;
                spicinessSegments.targetAttributeValue = 0;
                strengthSegments.targetAttributeValue = 0;
                ResetCumulative();
                orderOwner = null;
            }
            else if (orderInProgress)
            {
                imageColor.a = 1.0f;
            }
            image.color = imageColor;

            if (image.gameObject.name.Contains("Panel"))
            {
                Color tempcolor = image.GetComponentInParent<Image>().color;
                tempcolor.a = 0.0f;
                image.GetComponentInParent<Image>().color = tempcolor;
                image.color = tempcolor;
            }
        }
    }

    private void UpdateTimer()
    {
        if (orderOwner.GetCustomerState() != CustomerBase.CustomerState.Leaving)
            orderTimer.value = (orderOwner.customerLeaveTime - orderOwner.orderTimer.Value) / orderOwner.customerLeaveTime;
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

    public void SetPotentialTemperature(int value)
    {
        temperatureSegments.potentialIngredientValue = value;
        temperatureSegments.UpdateSegments(value);
    }

    public void SetPotentialSweetness(int value)
    {
        sweetnessSegments.potentialIngredientValue = value;
        sweetnessSegments.UpdateSegments(value);
    }

    public void SetPotentialSpiciness(int value)
    {
        spicinessSegments.potentialIngredientValue = value;
        spicinessSegments.UpdateSegments(value);
    }

    public void SetPotentialStrength(int value)
    {
        strengthSegments.potentialIngredientValue = value;
        strengthSegments.UpdateSegments(value);
    }

    public void SetCumulativeTemperature(int value)
    {
        temperatureSegments.cumulativeIngredientsValue = value;
        temperatureSegments.UpdateSegments(value);
    }

    public void SetCumulativeSweetness(int value)
    {
        sweetnessSegments.cumulativeIngredientsValue = value;
        sweetnessSegments.UpdateSegments(value);
    }

    public void SetCumulativeSpiciness(int value)
    {
        spicinessSegments.cumulativeIngredientsValue = value;
        spicinessSegments.UpdateSegments(value);
    }

    public void SetCumulativeStrength(int value)
    {
        strengthSegments.cumulativeIngredientsValue = value;
        strengthSegments.UpdateSegments(value);
    }

    private void ResetPotential()
    {
        SetPotentialSweetness(0);
        SetPotentialTemperature(0);
        SetPotentialSpiciness(0);
        SetPotentialStrength(0);
    }

    private void ResetCumulative()
    {
        SetCumulativeTemperature(0);
        SetCumulativeSweetness(0);
        SetCumulativeSpiciness(0);
        SetCumulativeStrength(0);
    }

    public void SetCumulativeToPotential()
    {
        SetCumulativeTemperature(temperatureSegments.potentialIngredientValue);
        SetCumulativeSweetness(sweetnessSegments.potentialIngredientValue);
        SetCumulativeSpiciness(spicinessSegments.potentialIngredientValue);
        SetCumulativeStrength(strengthSegments.potentialIngredientValue);
    }

    // Likely when the player walks away without selecting anything
    public void SetPotentialToCumulative()
    {
        SetPotentialTemperature(temperatureSegments.cumulativeIngredientsValue);
        SetPotentialSweetness(sweetnessSegments.cumulativeIngredientsValue);
        SetPotentialSpiciness(spicinessSegments.cumulativeIngredientsValue);
        SetPotentialStrength(strengthSegments.cumulativeIngredientsValue);
    }

    public void HoverIngredient(IngredientSO currentIngredient)
    {
        SetPotentialTemperature(currentIngredient.temperature + temperatureSegments.cumulativeIngredientsValue);
        SetPotentialSweetness(currentIngredient.sweetness + sweetnessSegments.cumulativeIngredientsValue);
        SetPotentialSpiciness(currentIngredient.spiciness + spicinessSegments.cumulativeIngredientsValue);
        SetPotentialStrength(currentIngredient.strength + strengthSegments.cumulativeIngredientsValue);
    }
}
