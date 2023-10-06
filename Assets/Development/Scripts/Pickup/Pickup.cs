using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pickup : MonoBehaviour
{
    [Header("Properties")]
    public List<PickupAttribute> attributes = new List<PickupAttribute>();

    [Header("Components")]
    [SerializeField] private CustomerBase customer;
#pragma warning disable CS0108
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private Collider collider;
    [SerializeField] private NavMeshAgent navMeshAgent;
#pragma warning restore CS0108
    [SerializeField] private float throwForceMultiplier = 1;
    public bool IsCustomer => customer != null;

    public Rigidbody GetRigidBody()
    {
        return rigidbody;
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
