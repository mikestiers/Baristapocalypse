using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
      
            Debug.Log("chocando");
            PhysicsManager floatingObjectManager = FindObjectOfType<PhysicsManager>();
            if (floatingObjectManager != null)
            {
                floatingObjectManager.HandleCollision(collision.contacts[0].normal);
            }
      
    }
}
