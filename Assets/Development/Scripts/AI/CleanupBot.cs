using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class CleanupBot : MonoBehaviour
{
    public UnityEvent<BotState, GameObject> onStateChanged;
    private NavMeshAgent agent;

    [SerializeField] private GameObject trashStation;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float distance = 30f;
    [SerializeField] private float distToNextNode = 0.1f;
    [SerializeField] private int trashCounter = 0;

    public enum BotState
    {
        Roam,
        Cleanup,
        Empty
    }

    [SerializeField] private BotState _currentState;

    public BotState currentState
    {
        get { return _currentState; }
        set
        {
            _currentState = value;
            onStateChanged.Invoke(_currentState, gameObject);
        }
    }

    public GameObject[] path;
    private int pathIndex = 0;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        _currentState = BotState.Roam;

        if (path.Length <= 0)
        {
            path = GameObject.FindGameObjectsWithTag("Node");
        }
    }

    private void Update()
    {
        GameObject[] messes = GameObject.FindGameObjectsWithTag("Mess");
        GameObject nearestMess = FindNearestMessOnFloor(messes);

        if (nearestMess != null)
        {
            _currentState = BotState.Cleanup;
            agent.SetDestination(nearestMess.transform.position);
        }
        else if (trashCounter > 3)
        {
            _currentState = BotState.Empty;
            agent.SetDestination(trashStation.transform.position);
        }
        else
        {
            _currentState = BotState.Roam;

            if (agent.remainingDistance < distToNextNode)
            {
                pathIndex++;
                pathIndex %= path.Length;
                agent.SetDestination(path[pathIndex].transform.position);
            }
        }
    }

    private GameObject FindNearestMessOnFloor(GameObject[] messes)
    {
        GameObject nearestMess = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject mess in messes)
        {
            Pickup pickupComponent = mess.GetComponent<Pickup>();
            if (pickupComponent != null && pickupComponent.isOnFloor)
            {
                float distanceToMess = Vector3.Distance(transform.position, mess.transform.position);
                if (distanceToMess < shortestDistance)
                {
                    shortestDistance = distanceToMess;
                    nearestMess = mess;
                }
            }
        }

        return nearestMess;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Pickup>().Getpickup().objectName == "MessCup") 
        {
            GameObject messToDestroy = other.gameObject.GetComponent<Pickup>().gameObject;
            trashCounter++;
            Destroy(messToDestroy);
        }
    }
}
