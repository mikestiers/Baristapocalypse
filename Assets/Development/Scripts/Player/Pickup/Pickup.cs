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
    [field: SerializeField] private PickupSO pickupSo { get; set; }
    [SerializeField] private IngredientFollowTransform FollowTransform;

    public PickupSO Getpickup()
    {
        return pickupSo;
    }
    private void Awake()
    {
        FollowTransform = GetComponent<IngredientFollowTransform>();
    }

    public static void SpawnPickupItem(PickupSO pickupSo, IPickupObjectParent pickupObjectParent)
    {
        BaristapocalypseMultiplayer.Instance.SpawnPickupObject(pickupSo,pickupObjectParent);
    }
    
    // public void SetpickupObjectParent(IPickupObjectParent pickupObjectParent)
    // {
    //     if (this.pickupObjectParent != null)
    //     {
    //         this.pickupObjectParent.ClearPickup();
    //     }
    //
    //     this.pickupObjectParent = pickupObjectParent;
    //
    //     if (pickupObjectParent.HasPickup())
    //     {
    //         Debug.Log("Player already has pickup");
    //     }
    //     
    //     this.pickupObjectParent.SetPickup(this);
    //     
    //     //FollowTransform.transform()
    // }

    public IPickupObjectParent GetpickupObjectParent()
    {
        return pickupObjectParent;
    }

    public PickupSO GetPickupObjectSo()
    {
        return pickupSo;
    }
    public void AddRigidbody()
    {
        transform.AddComponent<Rigidbody>();
    }

    public void RemoveRigidBody()
    {
        if (transform.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            Destroy(rb);
        }
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

    [Serializable]
    public enum PickupAttribute
    {
        CleansUpSpills,
        KillsCustomer,
    }
}
