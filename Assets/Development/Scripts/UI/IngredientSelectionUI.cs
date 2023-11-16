using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngredientSelectionUI : MonoBehaviour
{
    [Header("Drink Stat Sliders")]
    [SerializeField] private Slider tempSlider;
    [SerializeField] private Slider sweetSlider;
    [SerializeField] private Slider spicySlider;
    [SerializeField] private Slider strengthSlider;

    // TODO - Decide if using premapped buttons with listeners for each of the 24 buttons or if using:
    //          string level;
    //          level = EventSystem.current.currentSelectedGameObject.name;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Player Collided With trigger");

            // TODO - Display UI ingredient menu
            // TODO - Play UI ingredient menu grow animation
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Player left trigger collider");

            // TODO - Hide UI ingredient menu
            // TODO - Play UI ingredient shrink animation
        }
    }

    // TODO - OnCursorHover change slider values based on the ingredient
    //          - Take into account currently held ingredients
    //          - Reset sliders after cursor moves off buttons
    
    // TODO - Add Ingredient to inventory after selection

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}
