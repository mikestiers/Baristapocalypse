using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatsEvent : MonoBehaviour
{
   // [SerializeField] public RatStation Rats;

    private void Start()
    {
        
        //GameManager.Instance.OnPlayerDeactivateEvent += GameManager_OnPlayerDeactivateEvent;
    }


    private void AllRatsAreDead()
    {
        if (!CompareTag("Rats"))
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
