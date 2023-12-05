using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class IngredientSelectionUI : BaseStation
{
    private PlayerController player;

    [SerializeField] private GameObject ingredientMenu;
    private string hoverButtonName;

    [SerializeField] private IngredientSO[] ingredientListSO;

    private IngredientSO currentIngredient;
    private int ingredientListSOIndex;

    [Header("Drink Stat Sliders")]
    [SerializeField] private Slider tempSlider;
    [SerializeField] private Slider sweetSlider;
    [SerializeField] private Slider spicySlider;
    [SerializeField] private Slider strengthSlider;

    private void Start()
    {
        ingredientListSOIndex = 0;
        currentIngredient = ingredientListSO[ingredientListSOIndex];

        //player = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
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
        if (hoverButtonName == "Arabica" || hoverButtonName == "CowMilk" || hoverButtonName == "Sugar" || hoverButtonName == "PlutoniumPowder")
        {
            ingredientListSOIndex = 0;
            currentIngredient = ingredientListSO[ingredientListSOIndex];
            Debug.Log("The current IngredientListSOIndex is: " + ingredientListSOIndex);
        }
        if (hoverButtonName == "CosmicCacao" || hoverButtonName == "Water" || hoverButtonName == "MoonMaple" || hoverButtonName == "ButterBugs")
        {
            ingredientListSOIndex = 1;
            currentIngredient = ingredientListSO[ingredientListSOIndex];
            Debug.Log("The current IngredientListSOIndex is: " + ingredientListSOIndex);
        }
        if (hoverButtonName == "Excelsior" || hoverButtonName == "BlueMilk" || hoverButtonName == "GalaxyGummy" || hoverButtonName == "Brains")
        {
            ingredientListSOIndex = 2;
            currentIngredient = ingredientListSO[ingredientListSOIndex];
            Debug.Log("The current IngredientListSOIndex is: " + ingredientListSOIndex);
        }
        if (hoverButtonName == "Robusta" || hoverButtonName == "Moonshine" || hoverButtonName == "MochaMiel" || hoverButtonName == "FunkyFungus")
        {
            ingredientListSOIndex = 3;
            currentIngredient = ingredientListSO[ingredientListSOIndex];
            Debug.Log("The current IngredientListSOIndex is: " + ingredientListSOIndex);
        }
        if (hoverButtonName == "KopiLuwak" || hoverButtonName == "LavaGuava" || hoverButtonName == "Molasses" || hoverButtonName == "JupiterJelly")
        {
            ingredientListSOIndex = 4;
            currentIngredient = ingredientListSO[ingredientListSOIndex];
            Debug.Log("The current IngredientListSOIndex is: " + ingredientListSOIndex);
        }
        if (hoverButtonName == "Undefined" || hoverButtonName == "Gobstopper" || hoverButtonName == "MeatCube")
        {
            ingredientListSOIndex = 5;
            currentIngredient = ingredientListSO[ingredientListSOIndex];
            Debug.Log("The current IngredientListSOIndex is: " + ingredientListSOIndex);
        }

        // TODO - On cursor hover change slider values based on the ingredient
        //          - Take into account currently held ingredients
        //          - Reset sliders after cursor moves off buttons
    }

    public void AddIngredient()
    {
        Ingredient.SpawnIngredient(currentIngredient, player);
        player.GetNumberOfIngredients();
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.interactStation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            player = other.GetComponent<PlayerController>();
            Debug.Log("Player Collided With trigger");

            //Display UI ingredient menu
            Show(ingredientMenu);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Player left trigger collider");

            // Hide UI ingredient menu
            StartCoroutine(CloseMenu());
        }
    }

    // TODO - Add Ingredient to inventory after selection
    // TODO - Adjust sliders to represent currently held ingredients
    public void AddBean()
    {
        string buttonName;
        buttonName = EventSystem.current.currentSelectedGameObject.name;
    }

    public void AddSweetener()
    {
        string buttonName;
        buttonName = EventSystem.current.currentSelectedGameObject.name;
    }

    public void AddLiquid()
    {
        string buttonName;
        buttonName = EventSystem.current.currentSelectedGameObject.name;
    }

    public void AddBiomatter()
    {
        string buttonName;
        buttonName = EventSystem.current.currentSelectedGameObject.name;
    }

    private void Hide(GameObject obj)
    {
        obj.SetActive(false);
    }

    private void Show(GameObject obj)
    {
        obj.SetActive(true);
    }

    IEnumerator CloseMenu()
    {
        ingredientMenu.GetComponent<Animator>().Play("Ingredient_UI_Shrink");
        yield return new WaitForSeconds(0.5f);
        Hide(ingredientMenu);
    }
}
