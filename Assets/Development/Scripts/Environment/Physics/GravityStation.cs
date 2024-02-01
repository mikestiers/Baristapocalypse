using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityStation : MonoBehaviour
{
    [SerializeField] public GameObject gravityField;

    private void Start()
    {
        //GameManager.Instance.OnPlayerDeactivateEvent += GameManager_OnPlayerDeactivateEvent;
    }

    // this is temporary, the player will interact with this and hold a button to deactivate the gravity Storm
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            //gravityField.SetActive(true);
            //GameManager_OnPlayerDeactivateEvent(this, EventArgs.Empty);
            DeactivateRandomEvent();

        }
    }

    private void GameManager_OnPlayerDeactivateEvent(object sender, EventArgs e)
    {
        DeactivateRandomEvent();
    }

    private void DeactivateRandomEvent()
    {

        GameManager.Instance.isEventActive = false;
        RandomEventBase randomEvent = GameManager.Instance.currentRandomEvent;

        // Stop physics simulation for each object
        foreach (var obj in randomEvent.gameObject.GetComponent<GravityStorm>().objectsToMove)
        {
            Rigidbody objRigidbody = obj.GetComponent<Rigidbody>();
            if (objRigidbody != null)
            {
                objRigidbody.useGravity = true;
                //objRigidbody.Sleep();
            }
        }
        if (!randomEvent.GetNetworkObject().IsSpawned)
        {
            randomEvent.GetNetworkObject().Spawn(); 
        }
        Debug.LogWarning("DeactivateRandomEvent " + randomEvent.name);
        randomEvent.SetEventBool(false);
        randomEvent.ActivateDeactivateEvent();
    }
}
