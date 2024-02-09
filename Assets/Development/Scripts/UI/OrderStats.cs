using System;
using System.Collections.Generic;
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
    [SerializeField] public GameObject[] temperatureSegments;
    [SerializeField] public GameObject[] sweetnessSegments;
    [SerializeField] public GameObject[] spicinessSegments;
    [SerializeField] public GameObject[] strengthSegments;
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
    [SerializeField] public Sprite OrderSelectorEmpty;
    [SerializeField] public Sprite OrderSelecctorSuccess;
    [SerializeField] public Sprite OrderTargetEmptyPolygon;
    [SerializeField] public Sprite OrderTargetEmptyRectangle;
    [SerializeField] public Sprite OrderTargetSuccessPolygon;
    [SerializeField] public Sprite OrderTargetSuccessRectangle;
    [SerializeField] public Sprite TransparentSegment;  


    [SerializeField] public List<PlayerController> currentPlayers;
    [SerializeField] public Image resetMachineImage;
    [SerializeField] public bool orderInProgress { get; set; }
    [SerializeField] private CustomerBase orderOwner;
    [SerializeField] public BrewingStation brewingStation;

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
        ResetSegments(temperatureSegments);
        ResetSegments(sweetnessSegments);
        ResetSegments(spicinessSegments);
        ResetSegments(strengthSegments);
        orderInProgress = false;
        OrderInProgress();
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
        SetPotentialTemperatureSegment(temperaturePotentialAttributeSelector, temperatureSegments[MapValue(value)]);
    }

    public void SetPotentialSweetness(int value)
    {
        sweetnessPotentialValue = value;
        SetPotentialSweetnessSegment(sweetnessPotentialAttributeSelector, sweetnessSegments[MapValue(value)]);
    }

    public void SetPotentialSpiciness(int value)
    {
        spicinessPotentialValue = value;
        SetPotentialSpicinessSegment(spicinessPotentialAttributeSelector, spicinessSegments[MapValue(value)]);
    }

    public void SetPotentialStrength(int value)
    {   
        strengthPotentialValue = value;
        SetPotentialStrengthSegment(strengthPotentialAttributeSelector, strengthSegments[MapValue(value)]);
    }

    public void SetCumulativeTemperature(int value)
    {
        temperatureCumulativeValue = value;
    }

    public void SetCumulativeSweetness(int value)
    {
        sweetnessCumulativeValue = value;
    }

    public void SetCumulativeSpiciness(int value)
    {
        spicinessCumulativeValue = value;
    }

    public void SetCumulativeStrength(int value)
    {
        strengthCumulativeValue = value;
    }

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

    private void SetTargetSegments()
    {
        temperatureTargetSegment = temperatureSegments[temperatureTargetValue];
        if (temperatureTargetValue == MapValue(temperaturePotentialValue))
            if (MapValue(temperatureTargetValue) == 0)
                temperatureTargetSegment.GetComponent<Image>().sprite = OrderTargetSuccessPolygon;
            else
                temperatureTargetSegment.GetComponent<Image>().sprite = OrderTargetSuccessRectangle;
        else
            temperatureTargetSegment.GetComponent<Image>().sprite = OrderTargetEmptyRectangle;
        temperatureTargetSegment.SetActive(true);

        sweetnessTargetSegment = sweetnessSegments[sweetnessTargetValue];
        if (sweetnessTargetValue == MapValue(sweetnessPotentialValue))
            if (MapValue(sweetnessTargetValue) == 0)
                sweetnessTargetSegment.GetComponent<Image>().sprite = OrderTargetSuccessPolygon;
            else
                sweetnessTargetSegment.GetComponent<Image>().sprite = OrderTargetSuccessRectangle;
        else
            sweetnessTargetSegment.GetComponent<Image>().sprite = OrderTargetEmptyRectangle;
        sweetnessTargetSegment.SetActive(true);

        spicinessTargetSegment = spicinessSegments[spicinessTargetValue];
        if (spicinessTargetValue == MapValue(spicinessPotentialValue))
            if (MapValue(spicinessTargetValue) == 0)
                spicinessTargetSegment.GetComponent<Image>().sprite = OrderTargetSuccessPolygon;
            else
                spicinessTargetSegment.GetComponent<Image>().sprite = OrderTargetSuccessRectangle;
        else
            spicinessTargetSegment.GetComponent<Image>().sprite = OrderTargetEmptyRectangle;
        spicinessTargetSegment.SetActive(true);

        strengthTargetSegment = strengthSegments[strengthTargetValue];
        if (strengthTargetValue == MapValue(strengthPotentialValue))
            if (MapValue(strengthTargetValue) == 0)
                strengthTargetSegment.GetComponent<Image>().sprite = OrderTargetSuccessPolygon;
            else
            strengthTargetSegment.GetComponent<Image>().sprite = OrderTargetSuccessRectangle;
        else
            strengthTargetSegment.GetComponent<Image>().sprite = OrderTargetEmptyRectangle;
        strengthTargetSegment.SetActive(true);


        //difficulty range
        if (GameValueHolder.Instance.difficultySettings.GetDrinkThreshold() == 3)
        {
            if (Math.Abs(temperatureTargetValue - MapValue(temperaturePotentialValue)) <= 1)
            {
                temperatureTargetSegment.GetComponent<Image>().sprite = OrderTargetSuccessRectangle;
                temperatureSegments[temperatureTargetValue - 1].GetComponent<Image>().sprite = OrderTargetSuccessRectangle;
                temperatureSegments[temperatureTargetValue + 1].GetComponent<Image>().sprite = OrderTargetSuccessRectangle;
            }
            else
            {
                temperatureTargetSegment.GetComponent<Image>().sprite = OrderTargetEmptyRectangle;
                temperatureSegments[temperatureTargetValue - 1].GetComponent<Image>().sprite = OrderTargetEmptyRectangle;
                temperatureSegments[temperatureTargetValue + 1].GetComponent<Image>().sprite = OrderTargetEmptyRectangle;
            }

            if (Math.Abs(sweetnessTargetValue - MapValue(sweetnessPotentialValue)) <= 1)
            {
                sweetnessTargetSegment.GetComponent<Image>().sprite = OrderTargetSuccessRectangle;
                sweetnessSegments[sweetnessTargetValue - 1].GetComponent<Image>().sprite = OrderTargetSuccessRectangle;
                sweetnessSegments[sweetnessTargetValue + 1].GetComponent<Image>().sprite = OrderTargetSuccessRectangle;
            }
            else
            {
                sweetnessTargetSegment.GetComponent<Image>().sprite = OrderTargetEmptyRectangle;
                sweetnessSegments[sweetnessTargetValue - 1].GetComponent<Image>().sprite = OrderTargetEmptyRectangle;
                sweetnessSegments[sweetnessTargetValue + 1].GetComponent<Image>().sprite = OrderTargetEmptyRectangle;
            }

            if (Math.Abs(spicinessTargetValue - MapValue(spicinessPotentialValue)) <= 1)
            {
                spicinessTargetSegment.GetComponent<Image>().sprite = OrderTargetSuccessRectangle;
                spicinessSegments[spicinessTargetValue - 1].GetComponent<Image>().sprite = OrderTargetSuccessRectangle;
                spicinessSegments[spicinessTargetValue + 1].GetComponent<Image>().sprite = OrderTargetSuccessRectangle;
            }
            else
            {
                spicinessTargetSegment.GetComponent<Image>().sprite = OrderTargetEmptyRectangle;
                spicinessSegments[spicinessTargetValue - 1].GetComponent<Image>().sprite = OrderTargetEmptyRectangle;
                spicinessSegments[spicinessTargetValue + 1].GetComponent<Image>().sprite = OrderTargetEmptyRectangle;
            }

            if (Math.Abs(strengthTargetValue - MapValue(strengthPotentialValue)) <= 1)
            {
                strengthTargetSegment.GetComponent<Image>().sprite = OrderTargetSuccessRectangle;
                strengthSegments[strengthTargetValue - 1].GetComponent<Image>().sprite = OrderTargetSuccessRectangle;
                strengthSegments[strengthTargetValue + 1].GetComponent<Image>().sprite = OrderTargetSuccessRectangle;
            }
            else
            {
                strengthTargetSegment.GetComponent<Image>().sprite = OrderTargetEmptyRectangle;
                strengthSegments[strengthTargetValue - 1].GetComponent<Image>().sprite = OrderTargetEmptyRectangle;
                strengthSegments[strengthTargetValue + 1].GetComponent<Image>().sprite = OrderTargetEmptyRectangle;
            }

            temperatureSegments[temperatureTargetValue].SetActive(true);
            temperatureSegments[temperatureTargetValue - 1].SetActive(true);
            temperatureSegments[temperatureTargetValue + 1].SetActive(true);

            sweetnessSegments[sweetnessTargetValue].SetActive(true);
            sweetnessSegments[sweetnessTargetValue - 1].SetActive(true);
            sweetnessSegments[sweetnessTargetValue + 1].SetActive(true);

            spicinessSegments[spicinessTargetValue].SetActive(true);
            spicinessSegments[spicinessTargetValue - 1].SetActive(true);
            spicinessSegments[spicinessTargetValue + 1].SetActive(true);

            strengthSegments[strengthTargetValue].SetActive(true);
            strengthSegments[strengthTargetValue - 1].SetActive(true);
            strengthSegments[strengthTargetValue + 1].SetActive(true);

            //temperatureSegments[temperatureTargetValue - 1].SetActive(true);
            //temperatureSegments[temperatureTargetValue + 1].SetActive(true);
            //sweetnessSegments[sweetnessTargetValue - 1].GetComponent<Image>().sprite = OrderTargetEmptyRectangle;
            //sweetnessSegments[sweetnessTargetValue + 1].GetComponent<Image>().sprite = OrderTargetEmptyRectangle;
            //sweetnessSegments[sweetnessTargetValue - 1].SetActive(true);
            //sweetnessSegments[sweetnessTargetValue + 1].SetActive(true);
            //spicinessSegments[spicinessTargetValue - 1].GetComponent<Image>().sprite = OrderTargetEmptyRectangle;
            //spicinessSegments[spicinessTargetValue + 1].GetComponent<Image>().sprite = OrderTargetEmptyRectangle;
            //spicinessSegments[spicinessTargetValue - 1].SetActive(true);
            //spicinessSegments[spicinessTargetValue + 1].SetActive(true);
            //strengthSegments[strengthTargetValue - 1].GetComponent<Image>().sprite = OrderTargetEmptyRectangle;
            //strengthSegments[strengthTargetValue + 1].GetComponent<Image>().sprite = OrderTargetEmptyRectangle;
            //strengthSegments[strengthTargetValue - 1].SetActive(true);
            //strengthSegments[strengthTargetValue + 1].SetActive(true);  
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

    //private void SetPotentialSegment(GameObject ingredientPotentialAttributeSelector, GameObject potentialSegment, int cumulativeValue,  int potentialValue, int targetValue)
    //{
    //    if (cumulativeValue + potentialValue == targetValue)
    //        ingredientPotentialAttributeSelector.GetComponent<Image>().sprite = OrderSelecctorSuccess;
    //    else
    //        ingredientPotentialAttributeSelector.GetComponent<Image>().sprite = OrderSelectorEmpty;
    //    potentialSegment.SetActive(true);
    //    ingredientPotentialAttributeSelector.SetActive(true);
    //    ingredientPotentialAttributeSelector.transform.position = potentialSegment.transform.position;
    //}
    
    private void SetPotentialTemperatureSegment(GameObject ingredientPotentialAttributeSelector, GameObject potentialSegment)
    {
        if (temperatureTargetValue == MapValue(temperaturePotentialValue))
            ingredientPotentialAttributeSelector.GetComponent<Image>().sprite = OrderSelecctorSuccess;
        else
            ingredientPotentialAttributeSelector.GetComponent<Image>().sprite = OrderSelectorEmpty;

        // difficulty range
        if (GameValueHolder.Instance.difficultySettings.GetDrinkThreshold() == 3)
        {
            if (Math.Abs(temperatureTargetValue - MapValue(temperaturePotentialValue)) <= 1)
                ingredientPotentialAttributeSelector.GetComponent<Image>().sprite = OrderSelecctorSuccess;
        }
        potentialSegment.SetActive(true);
        ingredientPotentialAttributeSelector.SetActive(true);
        ingredientPotentialAttributeSelector.transform.position = potentialSegment.transform.position;
    }

    private void SetPotentialSweetnessSegment(GameObject ingredientPotentialAttributeSelector, GameObject potentialSegment)
    {
        if (sweetnessTargetValue == MapValue(sweetnessPotentialValue))
            ingredientPotentialAttributeSelector.GetComponent<Image>().sprite = OrderSelecctorSuccess;
        else
            ingredientPotentialAttributeSelector.GetComponent<Image>().sprite = OrderSelectorEmpty;

        // difficulty range
        if (GameValueHolder.Instance.difficultySettings.GetDrinkThreshold() == 3)
        {
            if (Math.Abs(sweetnessTargetValue - MapValue(sweetnessPotentialValue)) <= 1)
                ingredientPotentialAttributeSelector.GetComponent<Image>().sprite = OrderSelecctorSuccess;
        }

        potentialSegment.SetActive(true);
        ingredientPotentialAttributeSelector.SetActive(true);
        ingredientPotentialAttributeSelector.transform.position = potentialSegment.transform.position;
    }

    private void SetPotentialSpicinessSegment(GameObject ingredientPotentialAttributeSelector, GameObject potentialSegment)
    {
        if (spicinessTargetValue == MapValue(spicinessPotentialValue))
            ingredientPotentialAttributeSelector.GetComponent<Image>().sprite = OrderSelecctorSuccess;
        else
            ingredientPotentialAttributeSelector.GetComponent<Image>().sprite = OrderSelectorEmpty;

        // difficulty range
        if (GameValueHolder.Instance.difficultySettings.GetDrinkThreshold() == 3)
        {
            if (Math.Abs(spicinessTargetValue - MapValue(spicinessPotentialValue)) <= 1)
                ingredientPotentialAttributeSelector.GetComponent<Image>().sprite = OrderSelecctorSuccess;
        }

        potentialSegment.SetActive(true);
        ingredientPotentialAttributeSelector.SetActive(true);
        ingredientPotentialAttributeSelector.transform.position = potentialSegment.transform.position;
    }

    private void SetPotentialStrengthSegment(GameObject ingredientPotentialAttributeSelector, GameObject potentialSegment)
    {
        if (strengthTargetValue == MapValue(strengthPotentialValue))
            ingredientPotentialAttributeSelector.GetComponent<Image>().sprite = OrderSelecctorSuccess;
        else
            ingredientPotentialAttributeSelector.GetComponent<Image>().sprite = OrderSelectorEmpty;

        // difficulty range
        if (GameValueHolder.Instance.difficultySettings.GetDrinkThreshold() == 3)
        {
            if (Math.Abs(strengthTargetValue - MapValue(strengthPotentialValue)) <= 1)
                ingredientPotentialAttributeSelector.GetComponent<Image>().sprite = OrderSelecctorSuccess;
        }

        potentialSegment.SetActive(true);
        ingredientPotentialAttributeSelector.SetActive(true);
        ingredientPotentialAttributeSelector.transform.position = potentialSegment.transform.position;
    }
}
