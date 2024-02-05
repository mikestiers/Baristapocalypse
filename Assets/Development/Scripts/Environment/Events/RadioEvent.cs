using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RadioEvent : RandomEventBase
{
    public RadioStation radioStation;
     // Adjust this value to set the required hold time
   
    private void Start()
    {
       
    }
    private void radioBroken() 
    {
        
        radioStation.EventOn();
        Debug.Log("radio is broken");

    }

   

}
