using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class IngredientSelectionUI : MonoBehaviour
{
    [SerializeField] private GameObject ingredientMenu;
    private string hoverButtonName;

    [Header("Drink Stat Sliders")]
    [SerializeField] private Slider tempSlider;
    [SerializeField] private Slider sweetSlider;
    [SerializeField] private Slider spicySlider;
    [SerializeField] private Slider strengthSlider;

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

        // TODO - On cursor hover change slider values based on the ingredient
        //          - Take into account currently held ingredients
        //          - Reset sliders after cursor moves off buttons
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
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
