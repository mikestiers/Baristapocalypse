using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spill : MessBase
{
    private int cleaningProgress = 0; // start clenaing progress for spill
    private int totalProgress = 4; // amount of timer required to clean spill (temporary)
   

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void Interact(PlayerStateMachine player)
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

        Debug.Log("Cleaning progress" + cleaningProgress);
        

        //Debug.Log("Mess position" + player.GetMessTransform().position);

    }

    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
          Debug.Log("this is the player" + rb);
        if (other.gameObject.CompareTag("Player"))
        {
             Debug.Log("i am working");
        }
    }
    // {
    // Rigidbody rb = collision.collider.GetComponent<Rigidbody>();
    // Debug.Log("this is the player" + rb);
    //if (collision.gameObject.CompareTag("Player")) 
    // {
    //     Debug.Log("i am working");
    // }
    // float slipSpeed = 8;
    // if (rb != null) 
    // {
    //     Vector3 movedirection = rb.transform.forward;
    //     rb.AddForce(movedirection * slipSpeed , ForceMode.VelocityChange);
    // }
    // }
}
