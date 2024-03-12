using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Pickup : NetworkBehaviour
{
    [Header("Properties")]
    public List<PickupAttribute> attributes = new List<PickupAttribute>();
    public Vector3 holdingPosition;
    public Vector3 holdingRotation;

    [Header("Components")]
    [SerializeField] private CustomerBase customer;
#pragma warning disable CS0108
    [SerializeField] private Collider collider;
#pragma warning restore CS0108
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private float throwForceMultiplier = 1;
    public bool IsCustomer => customer != null;
    private IPickupObjectParent pickupObjectParent;
    [field: SerializeField] public PickupSO pickupSo { get;  private set; }
    [SerializeField] private IngredientFollowTransform followTransform;
    [SerializeField] private LayerMask groundLayer;
    public bool isOnFloor = false;

    public PickupSO Getpickup()
    {
        return pickupSo;
    }
    private void Awake()
    {
        followTransform = GetComponent<IngredientFollowTransform>();
    }

    public static void SpawnPickupItem(PickupSO pickupSO, Base pickupObjectParent)
    {
        BaristapocalypseMultiplayer.Instance.SpawnPickupObject(pickupSO, pickupObjectParent);
    }
    
    public void SetPickupObjectParent(IPickupObjectParent pickupObjectParent)
    {
        SetPickupObjectServerRpc(pickupObjectParent.GetNetworkObject());
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetPickupObjectServerRpc(NetworkObjectReference pickupObjectNetworkReference)
    {
        SetPickupObjectClientRpc(pickupObjectNetworkReference);
    }
    [ClientRpc]
    private void SetPickupObjectClientRpc(NetworkObjectReference pickupObjectNetworkReference)
    {
        pickupObjectNetworkReference.TryGet(out NetworkObject pickupNetworkParentObject);
        IPickupObjectParent pickupObjectParent = pickupNetworkParentObject.GetComponent<IPickupObjectParent>();

        if (this.pickupObjectParent != null)
        {
            this.pickupObjectParent.ClearPickup();
        }
    
        this.pickupObjectParent = pickupObjectParent;
    
        if (pickupObjectParent.HasPickup())
        {
            Debug.Log("Player already has pickup");
        }
        
        pickupObjectParent.SetPickup(this);

        followTransform.SetTargetTransform(pickupObjectParent.GetPickupTransform());
    }

    public void DisablePickupColliders(Pickup pickup)
    {
        DisablePickupObjectCollidersServerRpc(pickup.GetNetworkObject());
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void DisablePickupObjectCollidersServerRpc(NetworkObjectReference pickupNetworkObjectReference)
    {
        DisablePickupObjectCollidersClientRpc(pickupNetworkObjectReference);
    }
    
    [ClientRpc]
    private void DisablePickupObjectCollidersClientRpc(NetworkObjectReference pickupNetworkObjectReference)
    {
        pickupNetworkObjectReference.TryGet(out NetworkObject pickupNetworkObject);
        Pickup pickupObject = pickupNetworkObject.GetComponent<Pickup>();
        
        //pickupObject.RemoveRigidBody();
        pickupObject.GetCollider().enabled = false;
    }

    public void EnablePickupColliders(Pickup pickup)
    {
        EnablePickupObjectCollisionServerRpc(pickup.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void EnablePickupObjectCollisionServerRpc(NetworkObjectReference pickupNetworkObjectReference)
    {
        EnablePickupObjectCollidersClientRpc(pickupNetworkObjectReference);
    }

    [ClientRpc]
    private void EnablePickupObjectCollidersClientRpc(NetworkObjectReference pickupNetworkObjectReference)
    {
        pickupNetworkObjectReference.TryGet(out NetworkObject pickupNetworkObject);
        Pickup pickup = pickupNetworkObject.GetComponent<Pickup>();

        pickup.GetCollider().enabled = true;
    }


    public PickupSO GetPickupObjectSo()
    {
        return pickupSo;
    }

    public NavMeshAgent GetNavMeshAgent()
    {
        return navMeshAgent;
    }

    public float GetThrowForceMultiplier()
    {
        return throwForceMultiplier;
    }

    public Collider GetCollider()
    {
        return collider;
    }

    public CustomerBase GetCustomer()
    {
        return customer;
    }

    public static void DestroyPickup(Pickup pickup)
    {
        BaristapocalypseMultiplayer.Instance.DestroyPickup(pickup);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void ClearPickupOnParent()
    {
        pickupObjectParent.ClearPickup();
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
    [Serializable]
    public enum PickupAttribute
    {
        CleansUpSpills,
        KillsCustomer,
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isOnFloor = true;
            Debug.LogWarning("Mess on cup " + isOnFloor);
        }
    }
}
