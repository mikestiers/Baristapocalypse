using System;
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
    [SerializeField] public GameObject NextBrewingStation;
    [SerializeField] public GameObject PreviousBrewingStation;
    [SerializeField] public List<PlayerController> currentPlayers;
    [SerializeField] public Image resetMachineImage;

    [SerializeField] public bool orderInProgress { get; set; }

    [Header("Customer Review")]
    [SerializeField] private GameObject customerReview;
    [SerializeField] private CustomerBase orderOwner;

    //[SerializeField] private IngredientListSO temperatureIngredientList;
    //[SerializeField] private IngredientListSO sweetnessIngredientList;
    //[SerializeField] private IngredientListSO strengthIngredientList;
    //[SerializeField] private IngredientListSO spicinessIngredientList;

    //private void Start()
    //{
    //    temperatureIngredientList = GameManager.Instance.difficultySettings.temperatureIngredientList;
    //    sweetnessIngredientList = GameManager.Instance.difficultySettings.sweetnessIngredientList;
    //    strengthIngredientList = GameManager.Instance.difficultySettings.strengthIngredientList;
    //    spicinessIngredientList = GameManager.Instance.difficultySettings.spicinessIngredientList;
    //}
    private void Update()
    {
        if (orderInProgress)
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
    public void OrderInProgress(bool isInProgress)
    {
        Image[] images = GetComponentsInChildren<Image>(); // not including 'true' parameter because it includes inactive objects and the segments are not active by default
        foreach (Image image in images)
        {
            Color imageColor = image.GetComponent<Image>().color;
            if (!isInProgress)
            {
                imageColor.a = 0.2f;
                selectedByPlayerImage.SetActive(false);


                customerInfoRoot.SetActive(false);
                temperatureSegments.targetAttributeValue = 0;
                sweetnessSegments.targetAttributeValue = 0;
                spicinessSegments.targetAttributeValue = 0;
                strengthSegments.targetAttributeValue = 0;
                temperatureSegments.cumulativeIngredientsValue = 0;
                sweetnessSegments.cumulativeIngredientsValue = 0;
                spicinessSegments.cumulativeIngredientsValue = 0;
                strengthSegments.cumulativeIngredientsValue = 0;
                orderOwner = null;


            }
            else if (isInProgress)
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

        orderInProgress = isInProgress;
    }

    private void UpdateTimer()
    {
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

    public void SetTemperature()
    {
        temperatureSegments.UpdateSegments(temperatureSegments.cumulativeIngredientsValue);
    }

    public void SetSweetness()
    {
        sweetnessSegments.UpdateSegments(sweetnessSegments.cumulativeIngredientsValue);
    }

    public void SetSpiciness()
    {
        spicinessSegments.UpdateSegments(spicinessSegments.cumulativeIngredientsValue);
    }

    public void SetStrength()
    {
        strengthSegments.UpdateSegments(strengthSegments.cumulativeIngredientsValue);
    }

    public void SetPotentialTemperature()
    {
        temperatureSegments.UpdateSegments(temperatureSegments.potentialIngredientValue);
    }

    public void SetPotentialSweetness()
    {
        sweetnessSegments.UpdateSegments(sweetnessSegments.potentialIngredientValue);
    }

    public void SetPotentialSpiciness()
    {
        spicinessSegments.UpdateSegments(spicinessSegments.potentialIngredientValue);
    }

    public void SetPotentialStrength()
    {
        strengthSegments.UpdateSegments(strengthSegments.potentialIngredientValue);
    }
}
