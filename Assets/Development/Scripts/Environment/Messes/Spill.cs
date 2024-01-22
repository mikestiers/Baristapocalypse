using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Spill : NetworkBehaviour
{
    private int cleaningProgress = 0; // start clenaing progress for spill
    [SerializeField] private int totalProgress = 4; // amount of timer required to clean spill (temporary)
    [SerializeField] private float slipSpeed = 0.8f;
    private ISpill messObjectParent;
    private SpillSpawnPoint _spillSpawnPoint;
    private IngredientFollowTransform _followTransform; 

    private void Awake()
    {
        _spillSpawnPoint = GetComponent<SpillSpawnPoint>();
        
    }

    public void Interact(PlayerController player)
    {
        if (player.IsHoldingPickup)
        {
            if (player.Pickup.attributes.Contains(Pickup.PickupAttribute.CleansUpSpills))
            {
                if (cleaningProgress < totalProgress)
                {
                    // scale down the spill game object or play animation 
                    cleaningProgress++;
                }
                if (cleaningProgress >= totalProgress)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    public static void PlayerCreateSpill(MessSO Mess, ISpill messObjectParent)
    {
        BaristapocalypseMultiplayer.Instance.PlayerCreateSpill(Mess, messObjectParent);
    }

    public void SetSpillPosition(ISpill messObjectParent)
    {
        SetSpillpositionServerRpc(messObjectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    void SetSpillpositionServerRpc(NetworkObjectReference messObjectParent)
    {
        PlayerCreateSpillClientRpc(messObjectParent);
    }
    [ClientRpc]
    private void PlayerCreateSpillClientRpc(NetworkObjectReference messObjectNetworkReference)
    {
        messObjectNetworkReference.TryGet(out NetworkObject messObjectNetworkObject);
        ISpill messObjectComponet = messObjectNetworkObject.GetComponent<ISpill>();

        messObjectParent = messObjectComponet;

        messObjectComponet.SetSpill(this);
        Debug.Log($"Before SetSpill - Position: {messObjectComponet.GetSpillTransform().position}, Rotation: {messObjectComponet.GetSpillTransform().rotation}");

        _spillSpawnPoint.SetSpawnPointTransform(messObjectComponet.GetSpillTransform());
        Debug.Log($"After SetSpill - Position: {messObjectComponet.GetSpillTransform().position}, Rotation: {messObjectComponet.GetSpillTransform().rotation}");
    }
    
    
    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<PlayerController>()) 
        { 
             PlayerController stateMachine = other.gameObject.GetComponent<PlayerController>();
             Rigidbody rb = stateMachine.rb;
             Vector3 movedirection = rb.transform.forward;
             rb.AddForce(movedirection * slipSpeed , ForceMode.VelocityChange);
            stateMachine.ThrowIngedient();
        }
    }
   
    
  
    
}
          
        
   
