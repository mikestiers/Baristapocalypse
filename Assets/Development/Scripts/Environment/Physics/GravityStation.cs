using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class GravityStation : NetworkBehaviour
{
    [SerializeField] private GameObject gravityField;
    [SerializeField] private GameObject gravityButton;
    [SerializeField] private Material originalMaterial;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            InputManager.Instance.InteractEvent += DeactivateStormEvent;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            InputManager.Instance.InteractEvent -= DeactivateStormEvent;
        }
    }

    private void DeactivateStormEvent()
    {
        DeactivateStormEventServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void DeactivateStormEventServerRpc()
    {
        if (GameManager.Instance.isEventActive.Value && GameManager.Instance.isGravityStorm.Value)
        {
            GameManager.Instance.isEventActive.Value = false;
            GameManager.Instance.isGravityStorm.Value = false;
            DeactivateStormEventClientRpc();
        }
    }

    [ClientRpc]
    private void DeactivateStormEventClientRpc()
    {

        RandomEventBase randomEvent = GameManager.Instance.currentRandomEvent;
        if (randomEvent == null) return;
        GravityStorm gravityStorm = randomEvent.gameObject.GetComponent<GravityStorm>();
        if (gravityStorm != null)
        {
            GameManager.Instance.randomEventEffects.TurnOnOffEventEffect(false);
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

        Debug.LogWarning("DeactivateRandomEvent " + randomEvent.name);
        gravityButton.GetComponent<MeshRenderer>().material = originalMaterial;
        randomEvent.SetEventBool(false);
        randomEvent.ActivateDeactivateEvent();
        // foreach (GameObject bootParticle in PlayerController.Instance.bootsParticles)
        // {
        //     bootParticle.SetActive(GameManager.Instance.isEventActive);
        // }
    }
}

