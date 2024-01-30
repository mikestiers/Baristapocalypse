using System;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class OnScreenKeyboardInputkeys : MonoBehaviour
{
  [SerializeField] private string Inputletter;
  [SerializeField] private TMP_InputField currentInputField;
  [SerializeField] private TMP_Dropdown _dropdown;
  [Header("OnscreenKeyboards")]
  [SerializeField] private GameObject LowerCaseKeys;
  [SerializeField] private GameObject UpperCaseKeys;
  [SerializeField] private GameObject uppercaplockTartgetbutton;
  [SerializeField] private GameObject lowercaplockTartgetbutton;
  [SerializeField] private GameObject numPadKeys;
  [Header("Target buttons")]
  [SerializeField] private GameObject KeyboardButton;
  [SerializeField] private GameObject numPadTargetButton;
  [SerializeField] private GameObject uiSwitchTargetbutton;
  [SerializeField] private GameObject lastSelectedButton;
 
  
  // force the selected button to a key on the keyboard
   // everything is in scene called by each button on keyboard
  public void InputKey(string key)
  {
     if (currentInputField != null)
     {
        currentInputField.text += key;
     }
      //Debug.LogError(key);
   }
  public void RemoveInputkey(string key)
  {
     if (currentInputField != null)
     {
        if (currentInputField.text.Length > 0)
        {
           currentInputField.text = currentInputField.text.Substring(0, currentInputField.text.Length - 1);
        }
     }
  }
  
   public void ActivateCapsOnShiftPressed()
   {
      // Toggle between lowercase and uppercase keyboard
         if (LowerCaseKeys.activeSelf)
         {
            LowerCaseKeys.SetActive(false);
            UpperCaseKeys.SetActive(true);
            EventSystem.current.SetSelectedGameObject(uppercaplockTartgetbutton);
         }
         else if (UpperCaseKeys.activeSelf)
         {
            LowerCaseKeys.SetActive(true);
            UpperCaseKeys.SetActive(false);
            EventSystem.current.SetSelectedGameObject(lowercaplockTartgetbutton);
         }
   }

   public void DeactiveOnScreenKeyboard()
   {
      LowerCaseKeys.SetActive(false);
      UpperCaseKeys.SetActive(false); 
      numPadKeys.SetActive(false);
      ReturnToLastSelectedObject();
   }
   public void SetCurrentInputField(TMP_InputField newInputField)
   {
      // Set the current input field
      currentInputField = newInputField;
      LowerCaseKeys.SetActive(true);
      EventSystem.current.SetSelectedGameObject(KeyboardButton);
   }

   public void SetNumpadInputField(TMP_InputField newinputField)
   {
      currentInputField = newinputField;
      numPadKeys.SetActive(true);
      EventSystem.current.SetSelectedGameObject(numPadTargetButton);
   }

   public void MoveSelectedUiObject()
   {
      EventSystem.current.SetSelectedGameObject(uiSwitchTargetbutton);
   }

   public void SetLastSelectedObject(GameObject currentGameObject)
   {
      lastSelectedButton = currentGameObject;
   }
   public void ReturnToLastSelectedObject()
   {
      EventSystem.current.SetSelectedGameObject(lastSelectedButton);
   }


}
