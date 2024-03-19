using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class CleanupBot : NetworkBehaviour
{
    public UnityEvent<BotState, GameObject> onStateChanged;
    private NavMeshAgent agent;

    [SerializeField] private GameObject[] nodes;
    [SerializeField] private GameObject trashStation;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float distance = 30f;
    [SerializeField] private float distToNextNode = 0.1f;
    [SerializeField] private NetworkVariable<int> trashCounter = new NetworkVariable<int>(0);
    private RoombotConsole console;

    public enum BotState
    {
        Roaming,
        Cleanup,
        Emptying,
        Standby
    }

    [SerializeField] private BotState _currentState;

    public BotState currentState
    {
        get { return _currentState; }
        set
        {
            _currentState = value;
            onStateChanged?.Invoke(_currentState, gameObject);
            if (console != null) console.SetUIBotMode(_currentState.ToString());
        }
    }

    public GameObject[] path;
    private int pathIndex = 0;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        _currentState = BotState.Roaming;

        if (path.Length <= 0)
        {
            path = nodes;
            agent.SetDestination(path[pathIndex].transform.position);
        }
    }

    private void Update()
    {
        switch (_currentState)
        {
            case BotState.Cleanup:
                Cleaning();
                break;

            case BotState.Emptying:
                Emptying();
                break;

            case BotState.Roaming:
                Roaming();
                break;

            case BotState.Standby:
                Standby();
                break;
        }
    }

    private void Roaming()
    {
        RoamingClientRpc();
    }

    [ClientRpc]
    private void RoamingClientRpc()
    {
        GameObject[] messes = GameObject.FindGameObjectsWithTag("Mess");
        GameObject nearestMess = FindNearestMessOnFloor(messes);

        if (nearestMess != null)
        {
            _currentState = BotState.Cleanup;
            agent.SetDestination(nearestMess.transform.position);
        }
        else if (trashCounter.Value > 3)
        {
            _currentState = BotState.Emptying;
            agent.SetDestination(trashStation.transform.position);
        }
        else
        {
            if (agent == null) return;
            if (agent.remainingDistance < distToNextNode)
            {
                if (path != null)
                {
                    pathIndex++;
                    pathIndex %= path.Length;
                    agent.SetDestination(path[pathIndex].transform.position);
                }
            }
        }
    }

    private void Cleaning()
    {
        CleaningClientRpc();
    }

    [ClientRpc]
    private void CleaningClientRpc()
    {
        GameObject[] messes = GameObject.FindGameObjectsWithTag("Mess");
        GameObject nearestMess = FindNearestMessOnFloor(messes);

        if (nearestMess != null)
        {
            agent.SetDestination(nearestMess.transform.position);
        }
        else
        {
            _currentState = BotState.Roaming;
        }
    }

    private void Emptying()
    {
        EmptyingClientRpc();
    }

    [ClientRpc]
    private void EmptyingClientRpc()
    {
        agent.SetDestination(trashStation.transform.position);
        while (agent.pathPending && agent.remainingDistance > distToNextNode) return;

        if (agent.remainingDistance < distToNextNode)
        {
            trashCounter.Value = 0;
            _currentState = BotState.Standby;
        }
    }
    private void Standby()
    {

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
        if (other.gameObject.GetComponent<Pickup>() != null) 
        {
            Pickup _CollidingPickup = other.gameObject.GetComponent<Pickup>();

            if (_CollidingPickup.Getpickup() == null) return;

            if (_CollidingPickup.Getpickup().objectName == "MessCup")
            {
                BaristapocalypseMultiplayer.Instance.DestroyPickup(_CollidingPickup);
                trashCounter.Value++;
            }
        }
       
    }

    public void SetConsole(RoombotConsole console)
    {
        this.console = console;
    }

}
