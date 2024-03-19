using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Turns coffee up into mess cup if it hits the floor
public class CoffeeToMess : NetworkBehaviour
{
    [SerializeField] private Pickup messCup;
    [SerializeField] private GameObject messCupPrefab;
    [SerializeField] private LayerMask groundLayer;

    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            // Add animation to turn into mess cup
            TurnCoffeeToMess();
            BaristapocalypseMultiplayer.Instance.DestroyIngredient(gameObject.GetComponent<Ingredient>());
        }
    }

    private void TurnCoffeeToMess()
    {
        if (messCup)
        {
            SpawnMessCupServerRpc();
        }
    }

    [ServerRpc]
    private void SpawnMessCupServerRpc()
    {
        Transform messCupObjectTransform = Instantiate(messCupPrefab.transform);
        NetworkObject messCupObjectNetworkObject = messCupObjectTransform.GetComponent<NetworkObject>();
        messCupObjectNetworkObject.Spawn(true);
        messCupObjectNetworkObject.transform.position = gameObject.transform.position;
    }
}

