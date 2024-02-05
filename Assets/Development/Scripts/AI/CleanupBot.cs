using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class CleanupBot : MonoBehaviour
{
    public UnityEvent<CleanupBotState, GameObject> onStateChanged;
    NavMeshAgent agent;

    public GameObject target;
    public GameObject messTransform;
    public float distance = 20f;

    public enum CleanupBotState
    {
        Roam,
        Cleanup
    }

    [SerializeField] CleanupBotState _currentState;

    public CleanupBotState currentState
    {
        get { return _currentState; }
        set
        {
            _currentState = value;
            onStateChanged.Invoke(_currentState, gameObject);
        }
    }


    public GameObject[] path;
    public int pathIndex = 0;
    public float distToNextNode;

    // Start is called before the first frame update
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

    // Update is called once per frame
    void Update()
    {
        if (_currentState == CleanupBotState.Roam)
        {

            if (target)
                Debug.DrawLine(transform.position, target.transform.position, Color.blue);

            if (agent.remainingDistance < distToNextNode)
            {
                pathIndex++;

                pathIndex %= path.Length;

                target = path[pathIndex];
            }


        }
        if (_currentState == CleanupBotState.Cleanup)
        {
            if (target.gameObject.tag == "Node")
            {
                
            }

        }
        if (GameObject.FindGameObjectWithTag("Mess") == null)
        {
            currentState = CleanupBotState.Roam;
        }
        else
        {
            currentState = CleanupBotState.Cleanup;
        }

        if (_currentState == CleanupBotState.Cleanup)
        {
            target = GameObject.FindGameObjectWithTag("Mess");

            if (target)
                agent.SetDestination(target.transform.position);

        }

        if (target)
            agent.SetDestination(target.transform.position);

        float distToPlayer = Vector3.Distance(transform.position, messTransform.transform.position);

    }
    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject == GameObject.FindGameObjectWithTag("Mess"))
        {
            //destroy mess
            Destroy(other.gameObject);
        }
    }
}


