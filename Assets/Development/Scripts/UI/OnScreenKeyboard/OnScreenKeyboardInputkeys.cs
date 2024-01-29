using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class OnScreenKeyboardInputkeys : MonoBehaviour
{
  [SerializeField] private string Inputletter;
  [SerializeField] private TMP_InputField currentInputField;
  [SerializeField] private GameObject LowerCaseKeys;
  [SerializeField] private GameObject UpperCaseKeys;

  [SerializeField] private GameObject Targetbutton; // force the selected button to a key on the keyboard
   // everything is in scene called by each button on keyboard
  public void InputKey(string key)
  {
     if (currentInputField != null)
     {
        currentInputField.text += key;
     }
     Debug.LogError(key);
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
         }
         else if (UpperCaseKeys.activeSelf)
         {
            LowerCaseKeys.SetActive(true);
            UpperCaseKeys.SetActive(false);
         }
   }

   public void DeactiveOnScreenKeyboard()
   {
      LowerCaseKeys.SetActive(false);
      UpperCaseKeys.SetActive(false);
      EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
   }
   public void SetCurrentInputField(TMP_InputField newInputField)
   {
      // Set the current input field
      currentInputField = newInputField;
      LowerCaseKeys.SetActive(true);
       EventSystem.current.SetSelectedGameObject(Targetbutton);
   }
}
