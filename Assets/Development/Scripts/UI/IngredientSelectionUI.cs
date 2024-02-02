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
    BrewingStation[] brewingStations;
    public IngredientStationType ingredientStationType;
    public IngredientListSO ingredientList;
    private int ingredientListIndex;
    private IngredientSO currentIngredient;

    private void Start()
    {
        ingredientListIndex = 0;
        
        ingredientButtons = buttonsRoot.GetComponentsInChildren<Button>();
        brewingStations = UnityEngine.Object.FindObjectsOfType<BrewingStation>();
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
        if (brewingStations[player.currentBrewingStation].TryAddIngredient(currentIngredient))
        {
            brewingStations[player.currentBrewingStation].AddIngredientToListSO(BaristapocalypseMultiplayer.Instance.GetIngredientSOIndex(currentIngredient));
            OrderStats orderStats = orderStatsRoot.GetChild(player.currentBrewingStation).GetComponent<OrderStats>();
            orderStats.temperatureSegments.cumulativeIngredientsValue = orderStats.temperatureSegments.potentialIngredientValue;
            orderStats.sweetnessSegments.cumulativeIngredientsValue = orderStats.sweetnessSegments.potentialIngredientValue;
            orderStats.spicinessSegments.cumulativeIngredientsValue = orderStats.spicinessSegments.potentialIngredientValue;
            orderStats.strengthSegments.cumulativeIngredientsValue = orderStats.strengthSegments.potentialIngredientValue;
            orderStats.SetSweetness();
            orderStats.SetTemperature();
            orderStats.SetSpiciness();
            orderStats.SetStrength();
        }
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
            OrderStats orderStats = orderStatsRoot.GetChild(player.currentBrewingStation).GetComponent<OrderStats>();
            orderStats.temperatureSegments.potentialIngredientValue = currentIngredient.temperature + orderStats.temperatureSegments.cumulativeIngredientsValue;
            orderStats.sweetnessSegments.potentialIngredientValue = currentIngredient.sweetness + orderStats.sweetnessSegments.cumulativeIngredientsValue;
            orderStats.spicinessSegments.potentialIngredientValue = currentIngredient.spiciness + orderStats.spicinessSegments.cumulativeIngredientsValue;
            orderStats.strengthSegments.potentialIngredientValue = currentIngredient.strength + orderStats.strengthSegments.cumulativeIngredientsValue;
            orderStats.SetPotentialSweetness();
            orderStats.SetPotentialTemperature();
            orderStats.SetPotentialSpiciness();
            orderStats.SetPotentialStrength();
        }
    }

    IEnumerator CloseMenu()
    {
        ingredientMenu.GetComponent<Animator>().Play("Ingredient_UI_Shrink");
        yield return new WaitForSeconds(0.5f);
        Hide(ingredientMenu);
        OrderStats orderStats = orderStatsRoot.GetChild(player.currentBrewingStation).GetComponent<OrderStats>();
        orderStats.temperatureSegments.potentialIngredientValue = 0;
        orderStats.sweetnessSegments.potentialIngredientValue = 0;
        orderStats.spicinessSegments.potentialIngredientValue = 0;
        orderStats.strengthSegments.potentialIngredientValue = 0;
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