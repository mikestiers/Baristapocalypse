using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spill : MessBase
{
    private int cleaningProgress = 0; // start clenaing progress for spill
    [SerializeField] private int totalProgress = 4; // amount of timer required to clean spill (temporary)
    [SerializeField] private float slipSpeed = 0.8f;

    public override void Interact(PlayerController player)
    {
        if (player.hasMop == true ) 
        { 
            if (cleaningProgress < totalProgress )
            {
            // scale down the spill game object or play animation 
                cleaningProgress++;
            }
            if (cleaningProgress == totalProgress)
            {
                Destroy(player.GetMess().gameObject);
                cleaningProgress = 0;
            }   
        }
        else 
        {
            Debug.Log("cant clean");
            return;
        }

        // Reset the selectedMess field to null after the interaction is complete.
        player.SetSelectedMess(null); // we may not need this

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
          
        
   
