using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityCollisionHandler : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        GravityStorm floatingObjectManager = FindObjectOfType<GravityStorm>();
        if (floatingObjectManager != null)
        {
            floatingObjectManager.HandleCollision(collision.contacts[0].normal);
        }
    }
}
