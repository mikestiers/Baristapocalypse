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
           
            DeactivateRandomEvent();
            
        }
    }

    private void GameManager_OnPlayerDeactivateEvent(object sender, EventArgs e)
    {
        DeactivateRandomEvent();
    }

    private void DeactivateRandomEvent()
    {

        Wifi.WifiEventIsDone();
      // GameManager.Instance.isEventActive = false;
      // RandomEventBase randomEvent = GameManager.Instance.currentRandomEvent;
      //
      // if (!randomEvent.GetNetworkObject().IsSpawned)
      // {
      //     randomEvent.GetNetworkObject().Spawn();
      // }
      //
      // //  Debug.LogWarning("DeactivateRandomEvent " + randomEvent.name);
      //  randomEvent.SetEventBool(false);
      //  randomEvent.ActivateDeactivateEvent();
    }
}
