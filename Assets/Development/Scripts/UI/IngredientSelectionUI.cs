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
    [SerializeField] private GameObject[] selectedBGImages;
    public IngredientStationType ingredientStationType;
    public IngredientListSO ingredientList;
    private int ingredientListIndex;
    private IngredientSO currentIngredient;
    private bool canSelectIngredient = false;
    public bool isInUse = false;

    private void Start()
    {
        ingredientListIndex = 0;
        
        ingredientButtons = buttonsRoot.GetComponentsInChildren<Button>();

        switch (ingredientStationType)
        {
            case IngredientStationType.Temperature:
                ingredientList = GameValueHolder.Instance.difficultySettings.temperatureIngredientList;
                break;
            case IngredientStationType.Sweetness:
                ingredientList = GameValueHolder.Instance.difficultySettings.sweetnessIngredientList;
                break;
            case IngredientStationType.Strength:
                ingredientList = GameValueHolder.Instance.difficultySettings.strengthIngredientList;
                break;
            case IngredientStationType.Spiciness:
                ingredientList = GameValueHolder.Instance.difficultySettings.spicinessIngredientList;
                break;
            default:
                Debug.LogError("Ingredient Station Type not set");
                break;
        }

        RebuildButtonUI();

    }

    private void Update()
    {
        // This should not be in Update() but difficultysettings are not available when the game starts for some reason

       

        if (!currentStationInteraction)
            return;

        if (canSelectIngredient)
        {
            // Detect the name of the button that the cursor is hovering over
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;

            List<RaycastResult> raycastResultList = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResultList);

            GameObject hoveredObj = raycastResultList.Count > 0 ? raycastResultList[0].gameObject : null;
            GameObject selectedObj = EventSystem.current.currentSelectedGameObject;

            // Playing with controller or mouse?
            GameObject targetObj = selectedObj != null ? selectedObj : hoveredObj;

            if (targetObj != null)
            {
                for (int i = 0; i < ingredientButtons.Length; i++)
                {
                    foreach (GameObject backGroundobj in selectedBGImages)
                    {
                        backGroundobj.SetActive(false);
                    }

                    if (targetObj == ingredientButtons[i].gameObject)
                    {
                        selectedBGImages[i].SetActive(true);
                        ingredientListIndex = i;
                        currentIngredient = ingredientList.ingredientSOList[ingredientListIndex];
                        CalculateIngredients(currentIngredient);

                        break;
                    }
                }
            }
        }
    }

    private void RebuildButtonUI()
    {
        for (int i = 0; i < ingredientButtons.Length; i++)
        {
            if (i >= ingredientList.ingredientSOList.Count)
            {
                ingredientButtons[i].GetComponent<Image>().sprite = null;
            }
            else
            {
                ingredientButtons[i].GetComponent<Image>().sprite = ingredientList.ingredientSOList[i].icon;
                ingredientButtons[i].name = ingredientList.ingredientSOList[i].name;
            }
        }
    }

    public void AddIngredient()
    {
        EventSystem.current.SetSelectedGameObject(null);
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.interactStation);
        player.movementToggle = true;
        if (OrderManager.Instance.brewingStations[player.currentBrewingStation].TryAddIngredient(currentIngredient))
        {
            OrderManager.Instance.brewingStations[player.currentBrewingStation].AddIngredientToListSO(BaristapocalypseMultiplayer.Instance.GetIngredientSOIndex(currentIngredient));
            OrderManager.Instance.orderStats[player.currentBrewingStation].SetCumulativeToPotential();

            if (TutorialManager.Instance != null && TutorialManager.Instance.tutorialEnabled && !TutorialManager.Instance.firstIngredientSelected)
                TutorialManager.Instance.MadeFirstIngredientSelection();

            else if (TutorialManager.Instance != null && TutorialManager.Instance.tutorialEnabled && !TutorialManager.Instance.secondIngredientSelected)
                TutorialManager.Instance.MadeSecondIngredientSelection();

            else if (TutorialManager.Instance != null && TutorialManager.Instance.tutorialEnabled && !TutorialManager.Instance.thirdIngredientSelected)
                TutorialManager.Instance.MadeThirdIngredientSelection();

            else if (TutorialManager.Instance != null && TutorialManager.Instance.tutorialEnabled && !TutorialManager.Instance.fourthIngredientSelected)
                TutorialManager.Instance.MadeFourthIngredientSelection();
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
            if (player.IsLocalPlayer)
            {
                Show(ingredientMenu);
                isInUse = true;
            }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            player = other.GetComponent<PlayerController>();
            player.movementToggle = true;

            if (player.IsLocalPlayer)
            {
                // Hide UI ingredient menu
                EventSystem.current.SetSelectedGameObject(null);
                StartCoroutine(CloseMenu());
                isInUse = false;
            }
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
        canSelectIngredient = true;
        currentStationInteraction = true;
        obj.SetActive(true);

        // Select first igredient if not playing with mouse
        if (!Cursor.visible)
        {
            EventSystem.current.firstSelectedGameObject = ingredientButtons[0].gameObject;
            SetDefaultSelected(ingredientButtons[0].gameObject);
        }
        else if (Cursor.visible) 
        {
            // Fix bug where BG select image does not trigger at the start of the game
        }

    }

    private void CalculateIngredients(IngredientSO currentIngredient)
    {
        OrderManager.Instance.orderStats[player.currentBrewingStation].HoverIngredient(currentIngredient);
    }

    IEnumerator CloseMenu()
    {
        canSelectIngredient = false;
        ingredientMenu.GetComponent<Animator>().Play("Ingredient_UI_Shrink");
        yield return new WaitForSeconds(0.5f);
        Hide(ingredientMenu);
        OrderManager.Instance.orderStats[player.currentBrewingStation].SetPotentialToCumulative();
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