using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Gun : BaseStation
{
    [SerializeField] private GameObject gunOnPlayer;
    [SerializeField] private GameObject gunInStation;

    public void Start()
    {
        gunOnPlayer.SetActive(false);
        gunInStation.SetActive(true);
    }
    public override void InteractAlt(PlayerStateMachine player)
    {
        gunOnPlayer = GameObject.Find("GunVisuals");

        if (!player.hasGun && player.HasNoIngredients)
        {
            player.gunOnPlayer.SetActive(true);
            gunInStation.SetActive(false);
            player.hasGun = true;
        }
        else if (player.hasGun)
        {
            player.gunOnPlayer.SetActive(false);
            gunInStation.SetActive(true);
            player.hasGun = false;
        }
    }
}
