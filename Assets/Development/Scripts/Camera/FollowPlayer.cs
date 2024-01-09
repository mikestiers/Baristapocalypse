using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] Transform player; // Reference to the player's transform

    void Update()
    {
        LookAtPlayer();
    }

    void LookAtPlayer()
    {
        if (player != null)
        {
            // Calculate the direction from the camera to the player
            Vector3 directionToPlayer = player.position - transform.position;

            // Rotate the camera to look at the player
            transform.rotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);
        }
    }
}
