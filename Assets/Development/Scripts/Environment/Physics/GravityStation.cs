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
            InputManager.Instance.InteractEvent += DeactivateStormEventServerRpc;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            InputManager.Instance.InteractEvent -= DeactivateStormEventServerRpc;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void DeactivateStormEventServerRpc()
    {
        DeactivateStormEventClientRpc();
    }

    [ClientRpc]
    private void DeactivateStormEventClientRpc()
    {
        DeactivateStormEvent();
    }

    private void DeactivateStormEvent()
    {

        GameManager.Instance.isEventActive.Value = false;
        GameManager.Instance.isGravityStorm.Value = false;

        RandomEventBase randomEvent = GameManager.Instance.currentRandomEvent;
        if (randomEvent == null) return;
        GravityStorm gravityStorm = randomEvent.gameObject.GetComponent<GravityStorm>();
        if (gravityStorm != null)
        {
            GameManager.Instance.randomEventEffects.TurnOnOffEventEffectServerRpc(false);
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

