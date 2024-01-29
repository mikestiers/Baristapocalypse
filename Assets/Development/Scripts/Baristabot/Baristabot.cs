using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]

public class Baristabot : MonoBehaviour
{
    NavMeshAgent agent;

    public GameObject target;
    public Transform playerTransform;
    public float distance = 20f;

    public GameObject[] path;
    public int pathIndex = 0;
    public float distToNextNode;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();


        if (path.Length <= 0)
        {
            path = GameObject.FindGameObjectsWithTag("Node");
        }


        if (distToNextNode <= 0)
        {
            distToNextNode = 0.5f;
        }


    }

    void Update()
    {
        if (agent.remainingDistance < distToNextNode)
        {
            pathIndex++;

            pathIndex %= path.Length;

            target = path[pathIndex];
        }

        if (target)
            agent.SetDestination(target.transform.position);
    }
     
    
}