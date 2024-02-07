using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NextUIButton : MonoBehaviour
{
    [SerializeField] private GameObject nextSelectedButton;
    [SerializeField] private GameObject uiSwitchTargetbutton;
    [SerializeField] private GameObject currentUIElement;
    [SerializeField] private GameObject nextUIElement;

    public void MoveSelectedUiObject()
    {
        EventSystem.current.SetSelectedGameObject(nextSelectedButton);
    }
    
    public void SetLastSelectedObject(GameObject currentGameObject)
    {
        nextSelectedButton = currentGameObject;
        MoveSelectedUiObject();
    }

    public void DeActiveUIElement(GameObject currentActiveUIElement)
    {
        currentUIElement = currentActiveUIElement;
        currentUIElement.SetActive(false);
        
    }

    public void ActiveNextUIElement(GameObject newUIElement)
    {
        nextUIElement = newUIElement;
        nextUIElement.SetActive(true);
    }
}
