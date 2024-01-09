using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class splineAndPlayerFollower : MonoBehaviour
{
    public SplineFollower splineFollower; // Reference to the SplineFollower component on the camera
    public Transform player; // Reference to the player's transform

    void LateUpdate()
    {

        UpdateFollowerPosition();
    }



    void UpdateFollowerPosition()
    {
        if (player != null && splineFollower != null)
        {
            // Project the player's position onto the spline
            SplineSample result = splineFollower.spline.Project(player.position);

            // Set the follower's position on the spline
            splineFollower.SetPercent(result.percent);
        }
    }
}
