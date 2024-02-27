using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GravityStation : MonoBehaviour
{
    [SerializeField] private GameObject gravityField;
    [SerializeField] private GameObject gravityButton;
    [SerializeField] private Material originalMaterial;

    private Mouse mouse = Mouse.current;


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            InputManager.Instance.InteractEvent += HandleInteract;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            InputManager.Instance.InteractEvent -= HandleInteract;
        }
    }

    private void HandleInteract()
    {
        DeactivateRandomEvent();
    }

    private void DeactivateRandomEvent()
    {

        GameManager.Instance.isEventActive = false;
        GameManager.Instance.isGravityStorm = false;
        RandomEventBase randomEvent = GameManager.Instance.currentRandomEvent;

        GravityStorm gravityStorm = randomEvent.gameObject.GetComponent<GravityStorm>();
        if (gravityStorm != null)
        {
            // Populate objectsToMoveList before conversion
            gravityStorm.objectsToMoveList.Clear(); // Clear the list before populating
            foreach (var obj in gravityStorm.objectsToMove)
            {
                gravityStorm.objectsToMoveList.Add(obj);
            }

            gravityStorm.ConvertListToArray();

            // Stop physics simulation for each object
            foreach (var obj in gravityStorm.objectsToMove)
            {
                Rigidbody objRigidbody = obj.GetComponent<Rigidbody>();
                Collider objCollider = obj.GetComponent<Collider>();

                if (objRigidbody != null)
                {
                    objCollider.isTrigger = false;
                    objRigidbody.useGravity = true;
                    //objRigidbody.Sleep();
                }
            }
        }

        if (!randomEvent.GetNetworkObject().IsSpawned)
        {
            randomEvent.GetNetworkObject().Spawn();
        }

        Debug.LogWarning("DeactivateRandomEvent " + randomEvent.name);
        gravityButton.GetComponent<MeshRenderer>().material = originalMaterial;
        randomEvent.SetEventBool(false);
        randomEvent.ActivateDeactivateEvent();

        //foreach (GameObject bootParticle in PlayerController.Instance.bootsParticles)
        //{
        //    bootParticle.SetActive(GameManager.Instance.isEventActive);
        //}
    }

}


