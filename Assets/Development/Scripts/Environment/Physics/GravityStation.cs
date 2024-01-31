using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityStation : MonoBehaviour
{
    [SerializeField] public GameObject gravityField;

    private void Start()
    {
        GameManager.Instance.OnPlayerDeactivateEvent += GameManager_OnPlayerDeactivateEvent;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            //gravityField.SetActive(true);
            GameManager_OnPlayerDeactivateEvent(this, EventArgs.Empty);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        //if (other.GetComponent<PlayerController>())
        //{
        //    gravityField.SetActive(false);
        //}
    }

    private void GameManager_OnPlayerDeactivateEvent(object sender, EventArgs e)
    {
        DeactivateRandomEvent();
    }

    private void DeactivateRandomEvent()
    {
        RandomEventBase randomEvent = GameManager.Instance.currentRandomEvent;
        randomEvent.SetEventBool(false);
        randomEvent.ActivateDeactivateEvent();
        GameManager.Instance.isEventActive = false;
    }
}
