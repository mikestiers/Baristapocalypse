using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Linq;

public class IngredientSelectionUI : BaseStation
{
    private PlayerController player;

    public Transform orderStatsRoot;
    private bool currentStationInteraction;

    [SerializeField] private GameObject ingredientMenu;
    public GameObject buttonsRoot;
    [SerializeField] private Button[] ingredientButtons;
    //BrewingStation[] brewingStations;
    public IngredientStationType ingredientStationType;
    public IngredientListSO ingredientList;
    private int ingredientListIndex;
    private IngredientSO currentIngredient;

    private void Start()
    {
        ingredientListIndex = 0;
        
        ingredientButtons = buttonsRoot.GetComponentsInChildren<Button>();
        //brewingStations = UnityEngine.Object.FindObjectsOfType<BrewingStation>();
    }

    private void Update()
    {
        if (GameManager.Instance.difficultySettings == null)
            return;

        switch (ingredientStationType)
        {
            case IngredientStationType.Temperature:
                ingredientList = GameManager.Instance.difficultySettings.temperatureIngredientList;
                break;
            case IngredientStationType.Sweetness:
                ingredientList = GameManager.Instance.difficultySettings.sweetnessIngredientList;
                break;
            case IngredientStationType.Strength:
                ingredientList = GameManager.Instance.difficultySettings.strengthIngredientList;
                break;
            case IngredientStationType.Spiciness:
                ingredientList = GameManager.Instance.difficultySettings.spicinessIngredientList;
                break;
            default:
                Debug.LogError("Ingredient Station Type not set");
                break;
        }

        if (!currentStationInteraction)
            return;

        // Detect the name of the button that the cursor is hovering over
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);

        GameObject selectedObj = EventSystem.current.currentSelectedGameObject;
        if (selectedObj != null)
        {
            for (int i = 0; i < ingredientButtons.Length; i++)
            {
                if (selectedObj == ingredientButtons[i].gameObject)
                {
                    ingredientListIndex = i;
                    currentIngredient = ingredientList.ingredientSOList[ingredientListIndex];
                    CalculateIngredients(currentIngredient);
                    break;
                }
            }
        }
    }

    public void AddIngredient()
    {
        EventSystem.current.SetSelectedGameObject(null);
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.interactStation);
        player.movementToggle = true;
        OrderManager.Instance.brewingStations[player.currentBrewingStation].ingredientSOList.Add(currentIngredient);
        OrderManager.Instance.orderStats[player.currentBrewingStation].temperatureSegments.cumulativeIngredientsValue = OrderManager.Instance.orderStats[player.currentBrewingStation].temperatureSegments.potentialIngredientValue;
        OrderManager.Instance.orderStats[player.currentBrewingStation].sweetnessSegments.cumulativeIngredientsValue = OrderManager.Instance.orderStats[player.currentBrewingStation].sweetnessSegments.potentialIngredientValue;
        OrderManager.Instance.orderStats[player.currentBrewingStation].spicinessSegments.cumulativeIngredientsValue = OrderManager.Instance.orderStats[player.currentBrewingStation].spicinessSegments.potentialIngredientValue;
        OrderManager.Instance.orderStats[player.currentBrewingStation].strengthSegments.cumulativeIngredientsValue = OrderManager.Instance.orderStats[player.currentBrewingStation].strengthSegments.potentialIngredientValue;
        OrderManager.Instance.orderStats[player.currentBrewingStation].SetSweetness();
        OrderManager.Instance.orderStats[player.currentBrewingStation].SetTemperature();
        OrderManager.Instance.orderStats[player.currentBrewingStation].SetSpiciness();
        OrderManager.Instance.orderStats[player.currentBrewingStation].SetStrength();
        StartCoroutine(CloseMenu());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            player = other.GetComponent<PlayerController>();
            //player.movementToggle = false;

            //Display UI ingredient menu
            Show(ingredientMenu);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            player.movementToggle = true;

            // Hide UI ingredient menu
            EventSystem.current.SetSelectedGameObject(null);
            StartCoroutine(CloseMenu());
        }
    }

    private void Hide(GameObject obj)
    {
        obj.SetActive(false);
    }
    public void SetDefaultSelected(GameObject defaultSelected)
    {
        EventSystem.current.SetSelectedGameObject(defaultSelected);
    }

    private void Show(GameObject obj)
    {
        currentStationInteraction = true;
        obj.SetActive(true);
        EventSystem.current.firstSelectedGameObject = ingredientButtons[0].gameObject;
        SetDefaultSelected(ingredientButtons[0].gameObject);
    }

    private void CalculateIngredients(IngredientSO currentIngredient)
    {
        if (orderStatsRoot != null && orderStatsRoot.childCount > 0)
        {
            OrderManager.Instance.orderStats[player.currentBrewingStation].temperatureSegments.potentialIngredientValue = currentIngredient.temperature + OrderManager.Instance.orderStats[player.currentBrewingStation].temperatureSegments.cumulativeIngredientsValue;
            OrderManager.Instance.orderStats[player.currentBrewingStation].sweetnessSegments.potentialIngredientValue = currentIngredient.sweetness + OrderManager.Instance.orderStats[player.currentBrewingStation].sweetnessSegments.cumulativeIngredientsValue;
            OrderManager.Instance.orderStats[player.currentBrewingStation].spicinessSegments.potentialIngredientValue = currentIngredient.spiciness + OrderManager.Instance.orderStats[player.currentBrewingStation].spicinessSegments.cumulativeIngredientsValue;
            OrderManager.Instance.orderStats[player.currentBrewingStation].strengthSegments.potentialIngredientValue = currentIngredient.strength + OrderManager.Instance.orderStats[player.currentBrewingStation].strengthSegments.cumulativeIngredientsValue;
            OrderManager.Instance.orderStats[player.currentBrewingStation].SetPotentialSweetness();
            OrderManager.Instance.orderStats[player.currentBrewingStation].SetPotentialTemperature();
            OrderManager.Instance.orderStats[player.currentBrewingStation].SetPotentialSpiciness();
            OrderManager.Instance.orderStats[player.currentBrewingStation].SetPotentialStrength();
        }
    }

    IEnumerator CloseMenu()
    {
        ingredientMenu.GetComponent<Animator>().Play("Ingredient_UI_Shrink");
        yield return new WaitForSeconds(0.5f);
        Hide(ingredientMenu);
        OrderManager.Instance.orderStats[player.currentBrewingStation].temperatureSegments.potentialIngredientValue = 0;
        OrderManager.Instance.orderStats[player.currentBrewingStation].sweetnessSegments.potentialIngredientValue = 0;
        OrderManager.Instance.orderStats[player.currentBrewingStation].spicinessSegments.potentialIngredientValue = 0;
        OrderManager.Instance.orderStats[player.currentBrewingStation].strengthSegments.potentialIngredientValue = 0;
        currentStationInteraction = false;
        player.movementToggle = true;
    }
}

public enum IngredientStationType
{
    Temperature,
    Sweetness,
    Spiciness,
    Strength
}