using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class CleanupBot : MonoBehaviour
{

    public UnityEvent<BotState, GameObject> onStateChanged;
    NavMeshAgent agent;

    //Keep reference to player or target if aggro'd.
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject trashStation;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float distance = 30f;
    [SerializeField] private float trashCounter = 0;
    

    public enum BotState
    {
        Roam,
        Cleanup,
        Empty
    }

    [SerializeField] BotState _currentState;

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
    public int pathIndex = 0;
    public float distToNextNode;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        _currentState = BotState.Roam;

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
        target = GameObject.FindGameObjectWithTag("Mess");

        if (target)
        {
            _currentState = BotState.Cleanup;
        }
        else if (trashCounter > 3)
        {
            _currentState = BotState.Empty;
        }
        else
        {
            _currentState = BotState.Roam;
        }

        if (_currentState == BotState.Roam)
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
        else if (_currentState == BotState.Cleanup)
        {
            if (target.gameObject.tag == "Node")
            {
                target = GameObject.FindGameObjectWithTag("Mess");
            }

        }
        else if (_currentState == BotState.Empty)
        {
            target = trashStation;
            trashCounter = 0;
        }
        

        if (target)
            agent.SetDestination(target.transform.position);

        float distToMess = Vector3.Distance(transform.position, playerTransform.transform.position);

        if (distToMess > distance)
        {
            target = path[pathIndex];
            _currentState = BotState.Roam;
        }
        if (distToMess < distance)
        {
            _currentState = BotState.Cleanup;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameObject.FindGameObjectWithTag("Mess").gameObject)

            trashCounter++;
            Destroy(other.gameObject);
        }
    }
