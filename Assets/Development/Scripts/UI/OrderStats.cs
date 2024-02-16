using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OrderStats : MonoBehaviour
{
    [Header("UI Objects")]
    [SerializeField] public GameObject customerInfoRoot;
    [SerializeField] public Text customerNumberText;
    [SerializeField] public Text customerNameText;
    [SerializeField] public Text brewingStationText;
    [SerializeField] public Slider orderTimer;
    [SerializeField] public GameObject[] temperatureSegments;
    [SerializeField] public GameObject[] sweetnessSegments;
    [SerializeField] public GameObject[] spicinessSegments;
    [SerializeField] public GameObject[] strengthSegments;
    [SerializeField] public GameObject temperatureTargetAttributeSelector;
    [SerializeField] public GameObject sweetnessTargetAttributeSelector;
    [SerializeField] public GameObject spicinessTargetAttributeSelector;
    [SerializeField] public GameObject strengthTargetAttributeSelector;
    [SerializeField] public GameObject temperaturePotentialAttributeSelector;
    [SerializeField] public GameObject sweetnessPotentialAttributeSelector;
    [SerializeField] public GameObject spicinessPotentialAttributeSelector;
    [SerializeField] public GameObject strengthPotentialAttributeSelector;
    [SerializeField] public GameObject selectedByPlayerImage;
    [SerializeField] public Sprite OrderSelectorEmpty;
    [SerializeField] public Sprite OrderSelecctorSuccess;
    [SerializeField] public Sprite OrderTargetEmptyPolygon;
    [SerializeField] public Sprite OrderTargetEmptyRectangle;
    [SerializeField] public Sprite OrderTargetSuccessPolygon;
    [SerializeField] public Sprite OrderTargetSuccessRectangle;
    [SerializeField] public Sprite TransparentSegment;  
    [SerializeField] public Image resetMachineImage;
    [SerializeField] public Image previousMachineImage;
    [SerializeField] public Image nextMachineImage;

    [Header("Associated Brewing Station")]
    [SerializeField] public BrewingStation brewingStation;

    [Header("Private State Values")]
    [SerializeField] private CustomerBase orderOwner;
    [SerializeField] private GameObject temperatureTargetSegment;
    [SerializeField] private GameObject sweetnessTargetSegment;
    [SerializeField] private GameObject spicinessTargetSegment;
    [SerializeField] private GameObject strengthTargetSegment;
    [SerializeField] private int temperatureTargetValue;
    [SerializeField] private int sweetnessTargetValue;
    [SerializeField] private int spicinessTargetValue;
    [SerializeField] private int strengthTargetValue;
    [SerializeField] private int temperaturePotentialValue;
    [SerializeField] private int sweetnessPotentialValue;
    [SerializeField] private int spicinessPotentialValue;
    [SerializeField] private int strengthPotentialValue;
    [SerializeField] private int temperatureCumulativeValue;
    [SerializeField] private int sweetnessCumulativeValue;
    [SerializeField] private int spicinessCumulativeValue;
    [SerializeField] private int strengthCumulativeValue;

    public bool orderInProgress { get; set; }
    public List<PlayerController> currentPlayers;

    private void OnEnable()
    {
        PlayerController.OnInputChanged += InputUpdated;
        brewingStation.OnBrewingEmpty += OrderCompleted;
        brewingStation.OnBrewingDone += OrderCompleted;
    }

    private void OnDisable()
    {
        PlayerController.OnInputChanged -= InputUpdated;
        brewingStation.OnBrewingEmpty -= OrderCompleted;
        brewingStation.OnBrewingDone -= OrderCompleted;
    }

    private void Start()
    {
        ResetSegments(temperatureSegments);
        ResetSegments(sweetnessSegments);
        ResetSegments(spicinessSegments);
        ResetSegments(strengthSegments);
        orderInProgress = false;
        OrderInProgress();
    }

    private void Update()
    {
        if (orderInProgress == true)
        {
            UpdateTimer();
            SetTargetSegment(temperatureSegments, temperatureTargetValue, temperaturePotentialValue);
            SetTargetSegment(sweetnessSegments, sweetnessTargetValue, sweetnessPotentialValue);
            SetTargetSegment(spicinessSegments, spicinessTargetValue, spicinessPotentialValue);
            SetTargetSegment(strengthSegments, strengthTargetValue, strengthPotentialValue);
        }
    }

    private void InputUpdated(InputImagesSO inputImagesSO)
    {
        resetMachineImage.sprite = inputImagesSO.brewingStationEmpty;
        previousMachineImage.sprite = inputImagesSO.brewingStationSelectLeft;
        nextMachineImage.sprite = inputImagesSO.brewingStationSelectRight;
    }

    public int MapValue(int originalValue)
    {
        // Assuming originalValue is in the range of -7 to +7
        return originalValue + 7;
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
        SetTargetSegment(temperatureSegments, temperatureTargetValue, temperaturePotentialValue);
        SetTargetSegment(sweetnessSegments, sweetnessTargetValue, sweetnessPotentialValue);
        SetTargetSegment(spicinessSegments, spicinessTargetValue, spicinessPotentialValue);
        SetTargetSegment(strengthSegments, strengthTargetValue, strengthPotentialValue);
        OrderInProgress();
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
                temperatureTargetValue = 0;
                sweetnessTargetValue = 0;
                spicinessTargetValue = 0;
                strengthTargetValue = 0;

                ResetAll();
                orderOwner = null;
            }
            else if (orderInProgress)
            {
                imageColor.a = 1.0f;
            }
            image.color = imageColor;

            if (image.gameObject.name.Contains("Panel")) // all segment panels should be transparent images by default
            {
                image.sprite = TransparentSegment;
            }
        }
    }

    private void UpdateTimer()
    {
        if (orderOwner.GetCustomerState() != CustomerBase.CustomerState.Leaving)
            orderTimer.value = - (orderOwner.customerLeaveTime - orderOwner.orderTimer.Value) / orderOwner.customerLeaveTime;
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
        temperaturePotentialValue = value;
        SetPotentialSegment(temperaturePotentialAttributeSelector, temperatureSegments[MapValue(value)], temperaturePotentialValue, temperatureTargetValue);
    }

    public void SetPotentialSweetness(int value)
    {
        sweetnessPotentialValue = value;
        SetPotentialSegment(sweetnessPotentialAttributeSelector, sweetnessSegments[MapValue(value)], sweetnessPotentialValue, sweetnessTargetValue);
    }

    public void SetPotentialSpiciness(int value)
    {
        spicinessPotentialValue = value;
        SetPotentialSegment(spicinessPotentialAttributeSelector, spicinessSegments[MapValue(value)], spicinessPotentialValue, spicinessTargetValue);
    }

    public void SetPotentialStrength(int value)
    {   
        strengthPotentialValue = value;
        SetPotentialSegment(strengthPotentialAttributeSelector, strengthSegments[MapValue(value)], strengthPotentialValue, strengthTargetValue);
    }

    public void SetCumulativeTemperature(int value) => temperatureCumulativeValue = value;

    public void SetCumulativeSweetness(int value) => sweetnessCumulativeValue = value;

    public void SetCumulativeSpiciness(int value) => spicinessCumulativeValue = value;

    public void SetCumulativeStrength(int value) => strengthCumulativeValue = value;

    public void ResetSegments(GameObject[] segments)
    {
        foreach (var segment in segments)
        {
            segment.GetComponent<Image>().sprite = TransparentSegment;
        }
    }

    public void ResetAll()
    {
        ResetPotential();
        ResetCumulative();
        ResetSegments(temperatureSegments);
        ResetSegments(sweetnessSegments);
        ResetSegments(spicinessSegments);
        ResetSegments(strengthSegments);
        ResetSegmentsImages(temperatureSegments);
        ResetSegmentsImages(sweetnessSegments);
        ResetSegmentsImages(spicinessSegments);
        ResetSegmentsImages(strengthSegments);
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
        SetCumulativeTemperature(temperaturePotentialValue);
        SetCumulativeSweetness(sweetnessPotentialValue);
        SetCumulativeSpiciness(spicinessPotentialValue);
        SetCumulativeStrength(strengthPotentialValue);
    }

    // Player walks away without selecting anything
    public void SetPotentialToCumulative()
    {
        SetPotentialTemperature(temperatureCumulativeValue);
        SetPotentialSweetness(sweetnessCumulativeValue);
        SetPotentialSpiciness(spicinessCumulativeValue);
        SetPotentialStrength(strengthCumulativeValue);
    }

    // Player is considering an ingredient, but has not added it to the brewingstation
    public void HoverIngredient(IngredientSO currentIngredient)
    {
        SetPotentialTemperature(currentIngredient.temperature + temperatureCumulativeValue);
        SetPotentialSweetness(currentIngredient.sweetness + sweetnessCumulativeValue);
        SetPotentialSpiciness(currentIngredient.spiciness + spicinessCumulativeValue);
        SetPotentialStrength(currentIngredient.strength + strengthCumulativeValue);
    }

    private void SetTargetSegment(GameObject[] segments, int targetValue, int potentialValue)
    {
        // The zero (middle) segment of the orderstats has a different image so there are special cases
        // for that sprite... polygonSprite vs rectangleSprite
        
        if (GameValueHolder.Instance.difficultySettings.GetDrinkThreshold() == 1)
        {
            GameObject targetSegment = segments[targetValue];
            Sprite targetSprite;
            if (targetValue == MapValue(potentialValue))
                targetSprite = targetValue == 7 ? OrderTargetSuccessPolygon : OrderTargetSuccessRectangle;
            else
                targetSprite = targetValue == 7 ? OrderTargetEmptyPolygon : OrderTargetEmptyRectangle;

            targetSegment.GetComponent<Image>().sprite = targetSprite;
            targetSegment.SetActive(true);
        }

        //difficulty range
        if (GameValueHolder.Instance.difficultySettings.GetDrinkThreshold() == 3)
        {
            bool targetIs6To8 = targetValue == 6 || targetValue == 7 || targetValue == 8;
            bool isWithinTargetRange = Math.Abs(targetValue - MapValue(potentialValue)) <= 1;
            Sprite polygonSprite = isWithinTargetRange ? OrderTargetSuccessPolygon : OrderTargetEmptyPolygon;
            Sprite rectangleSprite = isWithinTargetRange ? OrderTargetSuccessRectangle : OrderTargetEmptyRectangle;

            if (targetIs6To8)
            {
                segments[targetValue].GetComponent<Image>().sprite = targetValue == 7 ? polygonSprite : rectangleSprite;
                segments[targetValue - 1].GetComponent<Image>().sprite = targetValue == 8 ? polygonSprite : rectangleSprite;
                segments[targetValue + 1].GetComponent<Image>().sprite = targetValue == 6 ? polygonSprite : rectangleSprite;
            }
            else
            {
                segments[targetValue - 1].GetComponent<Image>().sprite = rectangleSprite;
                segments[targetValue].GetComponent<Image>().sprite = rectangleSprite;
                segments[targetValue + 1].GetComponent<Image>().sprite = rectangleSprite;
            }

            segments[targetValue].SetActive(true);
            segments[targetValue - 1].SetActive(true);
            segments[targetValue + 1].SetActive(true);
        }
    }

    public void ResetSegmentsImages(GameObject[] segments)
    {
        // Reset all segments every time the segments are updated to clear any invalid colors
        foreach (var segment in segments)
        {
            segment.GetComponent<Image>().sprite = TransparentSegment;
            segment.SetActive(false);
        }
    }

    private void SetPotentialSegment(GameObject ingredientPotentialAttributeSelector, GameObject potentialSegment, int potentialValue, int targetValue)
    {
        if (targetValue == MapValue(potentialValue))
            ingredientPotentialAttributeSelector.GetComponent<Image>().sprite = OrderSelecctorSuccess;
        else
            ingredientPotentialAttributeSelector.GetComponent<Image>().sprite = OrderSelectorEmpty;

        // difficulty range
        if (GameValueHolder.Instance.difficultySettings.GetDrinkThreshold() == 3)
        {
            if (Math.Abs(targetValue - MapValue(potentialValue)) <= 1)
                ingredientPotentialAttributeSelector.GetComponent<Image>().sprite = OrderSelecctorSuccess;
        }
        potentialSegment.SetActive(true);
        ingredientPotentialAttributeSelector.SetActive(true);
        ingredientPotentialAttributeSelector.transform.position = potentialSegment.transform.position;
    }
}
