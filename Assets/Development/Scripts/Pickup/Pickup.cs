using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Pickup : MonoBehaviour
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
