using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WifiEvent : MonoBehaviour
{
    
    [SerializeField] public WifiStation Wifi;

    private void Start()
    {
        //Wifi.WifiEventIsStarting();
        //GameManager.Instance.OnPlayerDeactivateEvent += GameManager_OnPlayerDeactivateEvent;
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            if(GameManager.Instance.isWifiEvent.Value)
            {
                DeactivateRandomEvent();
            }
        }
    }

    private void GameManager_OnPlayerDeactivateEvent(object sender, EventArgs e)
    {
        DeactivateRandomEvent();
    }

    private void DeactivateRandomEvent()
    {
        Wifi.WifiEventIsDone();
    }
}
