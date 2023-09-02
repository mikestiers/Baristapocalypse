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

        
        // Reset the selectedMess field to null after the interaction is complete.
        player.SetSelectedMess(null); // we may not need this

        Debug.Log("Cleaning progress" + cleaningProgress);
        

        //Debug.Log("Mess position" + player.GetMessTransform().position);

    }

}
