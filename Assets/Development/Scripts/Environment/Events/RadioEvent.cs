using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RadioEvent : RandomEventBase
{
    public RadioStation radioStation;
    public float holdTime = 2f; // Adjust this value to set the required hold time
    private bool isButtonDown = false;
    private float buttonDownTime;

    private void Awake()
    {
       
        radioStation = GetComponent<RadioStation>();
        radioBroken();
    }
    private void radioBroken() 
    {
        radioStation.EventOn();
    }

    private void RadioFixed() 
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isButtonDown = true;
            buttonDownTime = Time.time;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isButtonDown = false;
        }

        if (isButtonDown && Time.time - buttonDownTime >= holdTime)
        {
            // Call the method to turn on your object or perform the desired action
            radioStation.EventOff();
            
        }
            
       
    }

}
