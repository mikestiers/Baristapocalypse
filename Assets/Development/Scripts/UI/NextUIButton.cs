using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NextUIButton : MonoBehaviour
{
    [SerializeField] private GameObject nextSelectedButton;
    [SerializeField] private GameObject uiSwitchTargetbutton;
    [SerializeField] private GameObject uiToActive;
    [SerializeField] private GameObject uiToDeactive;

    public void MoveSelectedUiObject()
    {
        EventSystem.current.SetSelectedGameObject(nextSelectedButton);
    }
    
    public void SetLastSelectedObject(GameObject currentGameObject)
    {
        nextSelectedButton = currentGameObject;
        MoveSelectedUiObject();
    }

    public void DeactiveCurrentUi(GameObject currentUI)
    {
        uiToDeactive = currentUI;
        uiToDeactive.SetActive(false);
    }

    public void ActiveNextUI(GameObject NextUI)
    {
        uiToActive = NextUI;
        uiToActive.SetActive(true);
    }
    
}
