using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Mop : BaseStation
{

    [SerializeField] private GameObject mopOnPlayer;
    [SerializeField] private GameObject mopInStation;

    public void Start()
    {
        mopOnPlayer.SetActive(false);
        mopInStation.SetActive(true);
    }
    public override void InteractAlt(PlayerStateMachine player) 
      {
        if (!player.hasMop && player.GetNumberOfIngredients() == 0) 
        {
            mopOnPlayer.SetActive(true);
            mopInStation.SetActive(false);
            player.hasMop = true;
        }
        else if (player.hasMop) 
        {
            mopOnPlayer.SetActive(false);
            mopInStation.SetActive(true);
            player.hasMop = false;
        }
         
       
     }
   

}
