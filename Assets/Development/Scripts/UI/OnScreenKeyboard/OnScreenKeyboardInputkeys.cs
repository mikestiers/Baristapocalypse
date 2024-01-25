using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
public class OnScreenKeyboardInputkeys : MonoBehaviour
{
  [SerializeField] private string Inputletter;
  [SerializeField] private TMP_InputField currentInputField;
  [SerializeField] private GameObject LowerCaseKeys;
  [SerializeField] private GameObject UpperCaseKeys;

   public void Update()
   {
      if (EventSystem.current.currentSelectedGameObject != null &&
          EventSystem.current.currentSelectedGameObject.GetComponent<InputField>() != null)
      {
         // Update the currentInputField reference
            currentInputField = EventSystem.current.currentSelectedGameObject.GetComponent<InputField>(); 
      }
   }

   public void InputKey(string key)
   {
      Inputletter = key;
      
      Debug.LogError(key);
   }

   public void ActvateCapsOnShiftPressed()
   {
      if (LowerCaseKeys.active)
      {
         LowerCaseKeys.SetActive(false);
         UpperCaseKeys.SetActive(true);
      }
      else if(UpperCaseKeys.active)
      {
         LowerCaseKeys.SetActive(true);
         UpperCaseKeys.SetActive(false);
      }
   }
 
}
