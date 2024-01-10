using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class splineAndPlayerFollower : MonoBehaviour
{
    public SplineFollower splineFollower; // Reference to the SplineFollower component on the camera
    public Transform player; // Reference to the player's transform
    float current;

    private void Start()
    {
        current = (float)splineFollower.GetPercent();
    }
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

            double target = result.percent;

            // Set the follower's position on the spline
            current = (Mathf.MoveTowards(current,(float)target, 5* Time.deltaTime));
            splineFollower.SetPercent(current);
        }
    }
}
