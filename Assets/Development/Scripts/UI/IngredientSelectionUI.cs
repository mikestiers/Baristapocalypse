using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class IngredientSelectionUI : BaseStation
{
    private PlayerController player;

    public Transform orderStatsRoot;
    private bool currentStationInteraction;

    [SerializeField] private GameObject ingredientMenu;
    private string hoverButtonName;
    public GameObject buttonsRoot;
    private Button[] ingredientButtons;
    BrewingStation[] brewingStations;

    [SerializeField] private IngredientSO[] ingredientListSO;

    private IngredientSO currentIngredient;
    private int ingredientListSOIndex;

    private void Start()
    {
        ingredientListSOIndex = 0;
        currentIngredient = ingredientListSO[ingredientListSOIndex];
        ingredientButtons = buttonsRoot.GetComponentsInChildren<Button>();
        brewingStations = Object.FindObjectsOfType<BrewingStation>();
    }

    private void Update()
    {
        if (!currentStationInteraction)
            return;

        Debug.Log(name);
        // Detect the name of the button that the cursor is hovering over
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);

        for (int i = 0; i < raycastResultList.Count; i++)
        {
            if (raycastResultList[i].gameObject.tag == "Ingredient_UI_Button")
            {
                hoverButtonName = raycastResultList[i].gameObject.name;
                Debug.Log("The cursor is currently over: " + hoverButtonName);
            }
        }

        // Detects which button is being hovered over and sets the ingredient index accordingly
        if (hoverButtonName == "Arabica" || hoverButtonName == "CowMilk" || hoverButtonName == "Sugar" || hoverButtonName == "PlutoniumPowder" ||
            EventSystem.current.currentSelectedGameObject.name == "Arabica" || EventSystem.current.currentSelectedGameObject.name == "CowMilk" || EventSystem.current.currentSelectedGameObject.name == "Sugar" || EventSystem.current.currentSelectedGameObject.name == "PlutoniumPowder")
        {
            ingredientListSOIndex = 0;
            currentIngredient = ingredientListSO[ingredientListSOIndex];
            Debug.Log("The current IngredientListSOIndex is: " + ingredientListSOIndex);
            CalculateIngredients(currentIngredient, ingredientListSOIndex);
        }
        else if (hoverButtonName == "CosmicCacao" || hoverButtonName == "Water" || hoverButtonName == "MoonMaple" || hoverButtonName == "ButterBugs" ||
            EventSystem.current.currentSelectedGameObject.name == "CosmicCacao" || EventSystem.current.currentSelectedGameObject.name == "Water" || EventSystem.current.currentSelectedGameObject.name == "MoonMaple" || EventSystem.current.currentSelectedGameObject.name == "ButterBugs")
        {
            ingredientListSOIndex = 1;
            currentIngredient = ingredientListSO[ingredientListSOIndex];
            Debug.Log("The current IngredientListSOIndex is: " + ingredientListSOIndex);
            CalculateIngredients(currentIngredient, ingredientListSOIndex);
        }
        else if (hoverButtonName == "Excelsior" || hoverButtonName == "BlueMilk" || hoverButtonName == "GalaxyGummy" || hoverButtonName == "Brains" ||
            EventSystem.current.currentSelectedGameObject.name == "Excelsior" || EventSystem.current.currentSelectedGameObject.name == "BlueMilk" || EventSystem.current.currentSelectedGameObject.name == "GalaxyGummy" || EventSystem.current.currentSelectedGameObject.name == "Brains")
        {
            ingredientListSOIndex = 2;
            currentIngredient = ingredientListSO[ingredientListSOIndex];
            Debug.Log("The current IngredientListSOIndex is: " + ingredientListSOIndex);
            CalculateIngredients(currentIngredient, ingredientListSOIndex);
        }
        else if (hoverButtonName == "Robusta" || hoverButtonName == "Moonshine" || hoverButtonName == "MochaMiel" || hoverButtonName == "FunkyFungus" ||
            EventSystem.current.currentSelectedGameObject.name == "Robusta" || EventSystem.current.currentSelectedGameObject.name == "Moonshine" || EventSystem.current.currentSelectedGameObject.name == "MochaMiel" || EventSystem.current.currentSelectedGameObject.name == "FunkyFungus")
        {
            ingredientListSOIndex = 3;
            currentIngredient = ingredientListSO[ingredientListSOIndex];
            Debug.Log("The current IngredientListSOIndex is: " + ingredientListSOIndex);
            CalculateIngredients(currentIngredient, ingredientListSOIndex);
        }
        else if (hoverButtonName == "KopiLuwak" || hoverButtonName == "LavaGuava" || hoverButtonName == "Molasses" || hoverButtonName == "JupiterJelly" ||
            EventSystem.current.currentSelectedGameObject.name == "KopiLuwak" || EventSystem.current.currentSelectedGameObject.name == "LavaGuava" || EventSystem.current.currentSelectedGameObject.name == "Molasses" || EventSystem.current.currentSelectedGameObject.name == "JupiterJelly")
        {
            ingredientListSOIndex = 4;
            currentIngredient = ingredientListSO[ingredientListSOIndex];
            Debug.Log("The current IngredientListSOIndex is: " + ingredientListSOIndex);
            CalculateIngredients(currentIngredient, ingredientListSOIndex);
        }
        else if (hoverButtonName == "SlurpJuice" || hoverButtonName == "Gobstopper" || hoverButtonName == "MeatCube" || hoverButtonName == "Undefined" ||
            EventSystem.current.currentSelectedGameObject.name == "SlurpJuice" || EventSystem.current.currentSelectedGameObject.name == "Gobstopper" || EventSystem.current.currentSelectedGameObject.name == "MeatCube" || EventSystem.current.currentSelectedGameObject.name == "Undefined")
        {
            ingredientListSOIndex = 5;
            currentIngredient = ingredientListSO[ingredientListSOIndex];
            Debug.Log("The current IngredientListSOIndex is: " + ingredientListSOIndex);
            CalculateIngredients(currentIngredient, ingredientListSOIndex);
        }
        else
        {
            if (orderStatsRoot != null && orderStatsRoot.childCount > 0 || EventSystem.current.currentSelectedGameObject == null)
            {
                OrderStats orderStats = orderStatsRoot.GetChild(0).GetComponent<OrderStats>();
                orderStats.temperatureSegments.potentialIngredientValue = 0;
                orderStats.sweetnessSegments.potentialIngredientValue = 0;
                orderStats.spicinessSegments.potentialIngredientValue = 0;
                orderStats.strengthSegments.potentialIngredientValue = 0;
            }
        }
    }

    public void AddIngredient()
    {
        // This was the code that spawned the ingredient into the player hands
        //Ingredient.SpawnIngredient(currentIngredient, player);
        //player.GetNumberOfIngredients();
        EventSystem.current.SetSelectedGameObject(null);
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.interactStation);
        player.movementToggle = true;
        brewingStations[0].ingredientSOList.Add(currentIngredient);
        OrderStats orderStats = orderStatsRoot.GetChild(0).GetComponent<OrderStats>();
        orderStats.temperatureSegments.cumulativeIngredientsValue = orderStats.temperatureSegments.potentialIngredientValue;
        orderStats.sweetnessSegments.cumulativeIngredientsValue = orderStats.sweetnessSegments.potentialIngredientValue;
        orderStats.spicinessSegments.cumulativeIngredientsValue = orderStats.spicinessSegments.potentialIngredientValue;
        orderStats.strengthSegments.cumulativeIngredientsValue = orderStats.strengthSegments.potentialIngredientValue;
        //Transform hudOrder = orderStats.transform.Find("HudOrder");
        //hudOrder.transform.Find("Temperature").GetComponent<OrderStatsSegments>().UpdateSegmentColors(orderStats.temperatureSegments.cumulativeIngredientsValue);
        //hudOrder.transform.Find("Sweetness").GetComponent<OrderStatsSegments>().UpdateSegmentColors(orderStats.sweetnessSegments.cumulativeIngredientsValue);
        //hudOrder.transform.Find("Spiciness").GetComponent<OrderStatsSegments>().UpdateSegmentColors(orderStats.spicinessSegments.cumulativeIngredientsValue);
        //hudOrder.transform.Find("Strength").GetComponent<OrderStatsSegments>().UpdateSegmentColors(orderStats.strengthSegments.cumulativeIngredientsValue);
        StartCoroutine(CloseMenu());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            player = other.GetComponent<PlayerController>();
            Debug.Log("Player Collided With trigger");

            //player.movementToggle = false;

            //Display UI ingredient menu
            Show(ingredientMenu);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Player left trigger collider");

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

    private void CalculateIngredients(IngredientSO currentIngredient, int ingredientListSOIndex)
    {
        if (orderStatsRoot != null && orderStatsRoot.childCount > 0)
        {
            OrderStats orderStats = orderStatsRoot.GetChild(0).GetComponent<OrderStats>();
            orderStats.temperatureSegments.potentialIngredientValue = currentIngredient.temperature + orderStats.temperatureSegments.cumulativeIngredientsValue;
            orderStats.sweetnessSegments.potentialIngredientValue = currentIngredient.sweetness + orderStats.sweetnessSegments.cumulativeIngredientsValue;
            orderStats.spicinessSegments.potentialIngredientValue = currentIngredient.spiciness + orderStats.spicinessSegments.cumulativeIngredientsValue;
            orderStats.strengthSegments.potentialIngredientValue = currentIngredient.strength + orderStats.strengthSegments.cumulativeIngredientsValue;
            //Transform hudOrder = orderStats.transform.Find("HudOrder");
            //hudOrder.transform.Find("Temperature").GetComponent<OrderStatsSegments>().UpdateSegmentColors(orderStats.temperatureSegments.potentialIngredientValue);
            //hudOrder.transform.Find("Sweetness").GetComponent<OrderStatsSegments>().UpdateSegmentColors(orderStats.sweetnessSegments.potentialIngredientValue);
            //hudOrder.transform.Find("Spiciness").GetComponent<OrderStatsSegments>().UpdateSegmentColors(orderStats.spicinessSegments.potentialIngredientValue);
            //hudOrder.transform.Find("Strength").GetComponent<OrderStatsSegments>().UpdateSegmentColors(orderStats.strengthSegments.potentialIngredientValue);
            Debug.Log("The current IngredientListSOIndex is: " + ingredientListSOIndex);
            Debug.LogError("this", ingredientListSO[ingredientListSOIndex]);
        }
    }

    IEnumerator CloseMenu()
    {
        ingredientMenu.GetComponent<Animator>().Play("Ingredient_UI_Shrink");
        yield return new WaitForSeconds(0.5f);
        Hide(ingredientMenu);
        OrderStats orderStats = orderStatsRoot.GetChild(0).GetComponent<OrderStats>();
        orderStats.temperatureSegments.potentialIngredientValue = 0;
        orderStats.sweetnessSegments.potentialIngredientValue = 0;
        orderStats.spicinessSegments.potentialIngredientValue = 0;
        orderStats.strengthSegments.potentialIngredientValue = 0;
        currentStationInteraction = false;
        player.movementToggle = true;
    }
}
