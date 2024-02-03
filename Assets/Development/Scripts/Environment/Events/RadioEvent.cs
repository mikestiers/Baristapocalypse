using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RadioEvent : RandomEventBase
{
    public RadioStation radioStation;

    private void Awake()
    {
       
        radioStation = GetComponent<RadioStation>();
    }
    private void radioBroken() 
    {
        radioStation.EventOn();
    }

    private void RadioFixed() 
    {
        radioStation.EventOff();
    }

}
