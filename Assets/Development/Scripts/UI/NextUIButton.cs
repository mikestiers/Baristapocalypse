using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NextUIButton : MonoBehaviour
{
    [SerializeField] private GameObject nextSelectedButton;
    [SerializeField] private GameObject uiSwitchTargetbutton;

    public void MoveSelectedUiObject()
    {
        EventSystem.current.SetSelectedGameObject(nextSelectedButton);
    }
    
    public void SetLastSelectedObject(GameObject currentGameObject)
    {
        nextSelectedButton = currentGameObject;
        MoveSelectedUiObject();
    }
   
}
