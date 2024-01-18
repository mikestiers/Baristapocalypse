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
    [SerializeField] private MessSO spillPrefab;
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

    public static void PlayerCreateSpill(MessSO Mess, IPickupObjectParent messObjectParent)
    {
        BaristapocalypseMultiplayer.Instance.PlayerCreateSpill(Mess, messObjectParent);
    }
    
    
    
    
    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<PlayerController>()) 
        { 
             PlayerController stateMachine = other.gameObject.GetComponent<PlayerController>();
             Rigidbody rb = stateMachine.rb;
             Vector3 movedirection = rb.transform.forward;
             rb.AddForce(movedirection * slipSpeed , ForceMode.VelocityChange);
            stateMachine.ThrowIngredient();
        }
    }

    public MessSO GetSpillSO()
    {
        return spillPrefab;
    }
    
  
    
}
          
        
   
