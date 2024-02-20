using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityCollisionHandler : MonoBehaviour
{
    private GravityStorm gravityStorm;

    private void Start()
    {
        // Find and store the GravityStorm script reference
        gravityStorm = FindObjectOfType<GravityStorm>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (gravityStorm != null)
        {
            //Debug.Log("cup collisioooon ");
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();

            if (rb != null)
            {
                gravityStorm.HandleCollision(rb, collision.contacts[0].normal);

            }
        }
    }
}

