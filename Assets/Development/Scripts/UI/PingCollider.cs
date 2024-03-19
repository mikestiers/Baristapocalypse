using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingCollider : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
