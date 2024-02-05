using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Lobbies.Models;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.HableCurve;

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
    [SerializeField] public GameObject[] temperatureSegmentsObject;
    [SerializeField] public GameObject[] sweetnessSegmentsObject;
    [SerializeField] public GameObject[] spicinessSegmentsObject;
    [SerializeField] public GameObject[] strengthSegmentsObject;
    [SerializeField] public GameObject temperatureTargetAttributeSelector;
    [SerializeField] public GameObject sweetnessTargetAttributeSelector;
    [SerializeField] public GameObject spicinessTargetAttributeSelector;
    [SerializeField] public GameObject strengthTargetAttributeSelector;
    [SerializeField] public GameObject temperatureTargetSegment;
    [SerializeField] public GameObject sweetnessTargetSegment;
    [SerializeField] public GameObject spicinessTargetSegment;
    [SerializeField] public GameObject strengthTargetSegment;
    [SerializeField] public GameObject temperaturePotentialAttributeSelector;
    [SerializeField] public GameObject sweetnessPotentialAttributeSelector;
    [SerializeField] public GameObject spicinessPotentialAttributeSelector;
    [SerializeField] public GameObject strengthPotentialAttributeSelector;
    [SerializeField] public GameObject selectedByPlayerImage;
    [SerializeField] public List<PlayerController> currentPlayers;
    [SerializeField] public Image resetMachineImage;
    [SerializeField] public bool orderInProgress { get; set; }
    [SerializeField] private CustomerBase orderOwner;
    [SerializeField] public BrewingStation brewingStation;

    private int temperatureTargetValue;
    private int sweetnessTargetValue;
    private int spicinessTargetValue;
    private int strengthTargetValue;


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
        ResetSegments(temperatureSegmentsObject);
        ResetSegments(sweetnessSegmentsObject);
        ResetSegments(spicinessSegmentsObject);
        ResetSegments(strengthSegmentsObject);
        orderInProgress = false;
        OrderInProgress();
    }

    public int MapValue(int originalValue)
    {
        // Assuming originalValue is in the range of -7 to +7
        return originalValue + 7;
    }

    public void ResetSegments(GameObject[] segments)
    {
        foreach (var segment in segments)
        {
            Color segmentColor = segment.GetComponent<Image>().color;
            segmentColor.a = 0.0f;
            segment.GetComponent<Image>().color = segmentColor;
            segment.SetActive(false);
        }
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
        temperatureTargetValue = MapValue(order.customer.coffeeAttributes.GetTemperature());
        sweetnessTargetValue = MapValue(order.customer.coffeeAttributes.GetSweetness());
        spicinessTargetValue = MapValue(order.customer.coffeeAttributes.GetSpiciness());
        strengthTargetValue = MapValue(order.customer.coffeeAttributes.GetStrength());
        ResetPotential();
        orderInProgress = true;
        SetTargetSegments();
        OrderInProgress();
    }

    private void Update()
    {
        if (orderInProgress == true)
        {
            UpdateTimer();
            SetTargetSegments();
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
        ResetSegmentsColour(temperatureSegmentsObject);
        temperatureSegments.potentialIngredientValue = value;
        temperatureSegments.UpdateSegments(value);
        SetPotentialSegment(temperaturePotentialAttributeSelector, temperatureSegmentsObject[MapValue(value)]);
    }

    public void SetPotentialSweetness(int value)
    {
        ResetSegmentsColour(sweetnessSegmentsObject);
        sweetnessSegments.potentialIngredientValue = value;
        sweetnessSegments.UpdateSegments(value);
        SetPotentialSegment(sweetnessPotentialAttributeSelector, sweetnessSegmentsObject[MapValue(value)]);
    }

    public void SetPotentialSpiciness(int value)
    {
        ResetSegmentsColour(spicinessSegmentsObject);
        spicinessSegments.potentialIngredientValue = value;
        spicinessSegments.UpdateSegments(value);
        SetPotentialSegment(spicinessPotentialAttributeSelector, spicinessSegmentsObject[MapValue(value)]);
    }

    public void SetPotentialStrength(int value)
    {   
        ResetSegmentsColour(strengthSegmentsObject);
        strengthSegments.potentialIngredientValue = value;
        strengthSegments.UpdateSegments(value);
        SetPotentialSegment(strengthPotentialAttributeSelector, strengthSegmentsObject[MapValue(value)]);
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

    public void ResetAll()
    {
        ResetPotential();
        ResetCumulative();
        ResetSegments(temperatureSegmentsObject);
        ResetSegments(sweetnessSegmentsObject);
        ResetSegments(spicinessSegmentsObject);
        ResetSegments(strengthSegmentsObject);
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

    // Player has selected an ingredient and added it to the brewing station
    public void SetCumulativeToPotential()
    {
        SetCumulativeTemperature(temperatureSegments.potentialIngredientValue);
        SetCumulativeSweetness(sweetnessSegments.potentialIngredientValue);
        SetCumulativeSpiciness(spicinessSegments.potentialIngredientValue);
        SetCumulativeStrength(strengthSegments.potentialIngredientValue);
    }

    // Player walks away without selecting anything
    public void SetPotentialToCumulative()
    {
        SetPotentialTemperature(temperatureSegments.cumulativeIngredientsValue);
        SetPotentialSweetness(sweetnessSegments.cumulativeIngredientsValue);
        SetPotentialSpiciness(spicinessSegments.cumulativeIngredientsValue);
        SetPotentialStrength(strengthSegments.cumulativeIngredientsValue);
    }

    // Player is considering an ingredient, but has not added it to the brewingstation
    public void HoverIngredient(IngredientSO currentIngredient)
    {
        SetPotentialTemperature(currentIngredient.temperature + temperatureSegments.cumulativeIngredientsValue);
        SetPotentialSweetness(currentIngredient.sweetness + sweetnessSegments.cumulativeIngredientsValue);
        SetPotentialSpiciness(currentIngredient.spiciness + spicinessSegments.cumulativeIngredientsValue);
        SetPotentialStrength(currentIngredient.strength + strengthSegments.cumulativeIngredientsValue);
    }

    private void SetTargetSegments()
    {
        temperatureTargetSegment = temperatureSegmentsObject[temperatureTargetValue];
        Color temperatureTargetSegmentColor = temperatureTargetSegment.GetComponent<Image>().color;
        temperatureTargetSegmentColor.a = 1.0f;
        temperatureTargetSegment.GetComponent<Image>().color = temperatureTargetSegmentColor;
        temperatureTargetSegment.SetActive(true);
        temperatureTargetAttributeSelector.transform.SetParent(temperatureTargetSegment.transform);
        temperatureTargetAttributeSelector.transform.position = temperatureTargetSegment.transform.position;
        temperatureTargetAttributeSelector.SetActive(true);

        sweetnessTargetSegment = sweetnessSegmentsObject[sweetnessTargetValue];
        Color sweetnessTargetSegmentColor = temperatureTargetSegment.GetComponent<Image>().color;
        sweetnessTargetSegmentColor.a = 1.0f;
        sweetnessTargetSegment.GetComponent<Image>().color = sweetnessTargetSegmentColor;
        sweetnessTargetSegment.SetActive(true);
        sweetnessTargetAttributeSelector.transform.SetParent(sweetnessTargetSegment.transform);
        sweetnessTargetAttributeSelector.transform.position = sweetnessTargetSegment.transform.position;
        sweetnessTargetAttributeSelector.SetActive(true);

        spicinessTargetSegment = spicinessSegmentsObject[spicinessTargetValue];
        Color spicinessTargetSegmentColor = spicinessTargetSegment.GetComponent<Image>().color;
        spicinessTargetSegmentColor.a = 1.0f;
        spicinessTargetSegment.GetComponent<Image>().color = spicinessTargetSegmentColor;
        spicinessTargetSegment.SetActive(true);
        spicinessTargetAttributeSelector.transform.SetParent(spicinessTargetSegment.transform);
        spicinessTargetAttributeSelector.transform.position = spicinessTargetSegment.transform.position;
        spicinessTargetAttributeSelector.SetActive(true);

        strengthTargetSegment = strengthSegmentsObject[strengthTargetValue];
        Color strengthTargetSegmentColor = strengthTargetSegment.GetComponent<Image>().color;
        strengthTargetSegmentColor.a = 1.0f;
        strengthTargetSegment.GetComponent<Image>().color = strengthTargetSegmentColor;
        strengthTargetSegment.SetActive(true);
        strengthTargetAttributeSelector.transform.SetParent(sweetnessTargetSegment.transform);
        strengthTargetAttributeSelector.transform.position = sweetnessTargetSegment.transform.position;
        strengthTargetAttributeSelector.SetActive(true);

        //difficulty range
        //Color targetSegmentRangeColor = Color.green;
        //targetSegmentColor.a = 1.0f;

        //if (GameManager.Instance.difficultySettings.GetDrinkThreshold() == 3)
        //{
        //    segments[MapValue(targetAttributeValue) - 1].GetComponent<Image>().color = targetSegmentRangeColor;
        //    segments[MapValue(targetAttributeValue) + 1].GetComponent<Image>().color = targetSegmentRangeColor;
        //    segments[MapValue(targetAttributeValue) - 1].SetActive(true);
        //    segments[MapValue(targetAttributeValue) + 1].SetActive(true);
        //}
    }

    public void ResetSegmentsColour(GameObject[] segments)
    {
        // Reset all segments every time the segments are updated to clear any invalid colors
        foreach (var segment in segments)
        {
            Color segmentColor = segment.GetComponent<Image>().color;
            segmentColor.a = 0.0f;
            segment.SetActive(false);
        }
    }

    private void SetPotentialSegment(GameObject ingredientPotentialAttributeSelector, GameObject potentialSegment)
    {
        ingredientPotentialAttributeSelector.SetActive(true);
        ingredientPotentialAttributeSelector.transform.SetParent(potentialSegment.transform);
        ingredientPotentialAttributeSelector.transform.position = potentialSegment.transform.position;
        ingredientPotentialAttributeSelector.SetActive(true);
    }
}
