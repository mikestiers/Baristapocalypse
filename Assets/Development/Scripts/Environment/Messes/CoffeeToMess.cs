using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Turns coffee up into mess cup if it hits the floor
public class CoffeeToMess : MonoBehaviour
{
    [SerializeField] private GameObject messCup;
    [SerializeField] private LayerMask groundLayer;

    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            // Add animation to turn into mess cup
            TurnCoffeeToMess();
            gameObject.GetComponent<NetworkObject>().Despawn();
            Destroy(gameObject);
        }
    }

    private void TurnCoffeeToMess()
    {
        if (messCup)
        {
            GameObject newMessCup = Instantiate(messCup, transform.position, transform.rotation);
            NetworkObject newMessCupNetworkObject = newMessCup.GetComponent<NetworkObject>();

            if (newMessCupNetworkObject)
            {
                newMessCupNetworkObject.Spawn();
            }
            else
            {
                Debug.Log("Mess Cup prefab does not have NetworkObject component.");
            }
        }
    }
}

