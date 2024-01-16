using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CustomerManager : Singleton<CustomerManager>
{
    [SerializeField] public DifficultySO[] Difficulties; //In Customer Manager for now move to Game Manager
    [SerializeField] private Transform Counter;
    [SerializeField] private Transform barEntrance;
    [SerializeField] private Transform exit;
    [SerializeField] private float minDelay = 6.0f;
    [SerializeField] private float maxDelay = 10.0f; //difficulty change
    [SerializeField] private int numberOfCustomers = 5;
    [SerializeField] private int customersLeftinWave;
    [SerializeField] private int WavesLeft;
    [SerializeField] private int customersInStore = 0;
    private int customerNumber = 0;
    List<string> customerNames = new List<string>
        {
            "Abby",
            "Abdul",
            "Aidan",
            "Alex G",
            "Alex J",
            "Andrew",
            "Carter",
            "Darian",
            "Deandre",
            "Douglas",
            "Dustin",
            "Gabriel R",
            "Gabriel P",
            "Jeffrey",
            "Juan",
            "Kayden",
            "Lam",
            "Liam",
            "Meg",
            "Michael",
            "Morgan",
            "Nikki",
            "Red",
            "Rod",
            "Turner"
        };


    public NetworkObject customerPrefab;

    public List<CustomerBase> customersOutsideList = new List<CustomerBase>();

    public CustomerLineQueuing LineQueue;
    public CustomerBarFloor barFloor;
    public DifficultySettings difficultySettings; //will move to GameManager when gamemanager is owki, change references to GameManager aswell
    public DifficultySO currentDifficulty;

    private NetworkObject newcustomer;

    //temp stuff -> Ddog will make something more robust
    public GameObject[] Chairs;
    //private int chairNumber = 0;

    // Start is called before the first frame update
    private void Start()
    {
        //this is to check the amount of players
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int numberOfPlayers = (players.Length - Mathf.FloorToInt(players.Length * 0.5f));

        List<Vector3> waitingQueuePostionList = new List<Vector3>();
        if (Chairs.Length <= 0) Chairs = GameObject.FindGameObjectsWithTag("Waypoint");
        //chairNumber = UnityEngine.Random.Range(0, Chairs.Length);

        //where the firstposition is located in scene
        Vector3 firstposition = new Vector3(Counter.position.x, 0, Counter.position.z + 1.5f);
        float positionSize = 2f;
        for (int i = 0; i < numberOfCustomers; i++)
        {
            waitingQueuePostionList.Add(firstposition + new Vector3(0, 0, 1f) * positionSize * i);
        }

        //LineQueue.OnCustomerArrivedAtFrontOfQueue += WaitingQueue_OnCustomerArrivedAtFrontOfQueue; might be used for future code?
        LineQueue = new CustomerLineQueuing(waitingQueuePostionList);
        barFloor = new CustomerBarFloor(Chairs);
        difficultySettings = new DifficultySettings(currentDifficulty , numberOfPlayers); // change constructor to set difficulty when moving to GameManager, made this way just for testing

        customersLeftinWave = difficultySettings.GetNumberofCustomersInwave();
        WavesLeft = difficultySettings.GetNumberOfWaves();

        UIManager.Instance.customersLeft.text = ("SpawnLeft: " + customersLeftinWave.ToString());
        UIManager.Instance.spawnMode.text = "Serving Customers";
        UIManager.Instance.shift.text = ("Shift " + difficultySettings.GetShift().ToString());
        UIManager.Instance.wavesleft.text = ("Waves Left: " + WavesLeft.ToString());

        minDelay = difficultySettings.GetMinDelay();
        maxDelay = difficultySettings.GetMaxDelay();
        difficultySettings.SetAmountOfPlayers(numberOfPlayers); // setdifficulty based on amount of players

        float delay = UnityEngine.Random.Range(minDelay, maxDelay);

        StartCoroutine(NewCustomer(delay));

    }

    //maybe randomize time of spawning of customers
    public IEnumerator NewCustomer(float delayS)
    {
        while (true)
        {
            float delay = UnityEngine.Random.Range(minDelay, maxDelay);
            //yield return new WaitUntil(() -> customers.isServed);
            yield return new WaitForSeconds(delay);
            int randomCustomer = UnityEngine.Random.Range(0, customerNames.Count);
            //while(gameObject is playin) set timer
            if (customerPrefab != null)
            {

                SpawnCustomerClientRpc();
                GiveCustomerNameClientRpc(randomCustomer);
                StartCoroutine(CustomerEnterStore());
                customersInStore++;
                customersLeftinWave--;
                UIManager.Instance.customersInStore.text = ("Customers in Store: ") + customersInStore.ToString();
                UIManager.Instance.customersLeft.text = ("SpawnLeft: " + customersLeftinWave.ToString());

                if (customersLeftinWave <= 0) yield break;
            }
        }
    }

    // Trigger Time Between waves
    public void NextWave()
    {
        WavesLeft--;
        if (WavesLeft <= 0) 
        {
            difficultySettings.NextShift();
            WavesLeft = difficultySettings.GetNumberOfWaves();
            UIManager.Instance.shift.text = ("Shift " + difficultySettings.GetShift().ToString());
        }
        
        customersLeftinWave = difficultySettings.GetNumberofCustomersInwave();
        UIManager.Instance.wavesleft.text = ("Waves Left: " + WavesLeft.ToString());

        //Trigger UI
        UIManager.Instance.spawnMode.text = "Resting";

        StartCoroutine(RestPeriod(difficultySettings.GetTimeBetweenWaves()));

    }

    //Timer for Inbetween Waves
    public IEnumerator RestPeriod(float timer)
    {
        yield return new WaitForSeconds(timer);

        //Trigger UI
        UIManager.Instance.spawnMode.text = "Serving Customers";
        UIManager.Instance.customersLeft.text = ("SpawnLeft: " + customersLeftinWave.ToString());

        float delay = UnityEngine.Random.Range(minDelay, maxDelay);

        StartCoroutine(NewCustomer(delay));
    }


    [ClientRpc]
    public void SpawnCustomerClientRpc()
    {
        SpawnCustomer();
    }

    public IEnumerator CustomerEnterStore()
    {
        yield return new WaitForSeconds(1f);

        if (LineQueue.CanAddCustomer() == true) // add condition for waves? 
        {
            //we can randomize this aswell just set 0 to a random integer from modulo if size of list
            LineQueue.AddCustomer(customersOutsideList[0]);
            customersOutsideList.RemoveAt(0);
        }
        else
        {
            customersOutsideList[0].CustomerLeave();
            customersOutsideList.RemoveAt(0);
        }
    }

    public void Leaveline()
    {
        Debug.Log("hiii");
        CustomerBase customer = LineQueue.GetFirstInQueue();
        barFloor.TrySendToChair(customer);
    }

    private void SpawnCustomer()
    {
        newcustomer = Instantiate(customerPrefab, barEntrance.transform.position, Quaternion.identity);

        // Ensure that the spawned object is spawned on the network
        newcustomer.Spawn();

        customersOutsideList.Add(newcustomer.GetComponent<CustomerBase>());

        customerNumber += 1;

    }

    [ClientRpc]
    public void GiveCustomerNameClientRpc(int randomCustomer)
    {
        // Assign customer number and choose a random name from the list.  If list becomes empty, no names are assigned
        // We have 25 names so far (the names of everyone on the team), but we can add more

        newcustomer.GetComponent<CustomerBase>().customerNumber = customerNumber;
        if (customerNames.Count >= 1)
        {
            //int randomCustomer = UnityEngine.Random.Range(0, customerNames.Count);
            newcustomer.GetComponent<CustomerBase>().SetCustomerName(customerNames[randomCustomer]);
            customerNames.RemoveAt(randomCustomer);
        }
    }

    public int TotalCustomers()
    {
        int totalCustomers = LineQueue.GetLineCount() + barFloor.GetTotalCustomersOnFloor();

        return totalCustomers;
    }

    public int TotalMaxCapacity()
    {
        int maxCapacity = Chairs.Length + numberOfCustomers;

        return maxCapacity;
    }

    /* i didnt delete just incase it can be used for future code

    private void WaitingQueue_OnCustomerArrivedAtFrontOfQueue(object sender, System.EventArgs e)
    {
        StartCoroutine(ProcessNextCustomer());
    }
    private IEnumerator ProcessNextCustomer()
    {
        yield return new WaitForSeconds(1f); // Wait for 1 second

        CustomerBase customer = LineQueue.GetFirstInQueue();
        if (customer != null)
        {
            // Order is taken for the current customer
            customer.OrderTaken();

            // Move the next customer up in the line ---> dont need to move them cuz GetFirstInQueue Already has the relocate function
           /* CustomerBase nextCustomer = LineQueue.GetFirstInQueue();
            if (nextCustomer != null)
            {
                nextCustomer.agent.SetDestination(new Vector3(-2.5f, 0, nextCustomer.transform.position.z));

            } 
        }
    }
    */
    public Transform GetExit()
    {
        return exit;
    }

    public int GetCustomerLeftinStore()
    {
        return customersInStore;
    }

    public void ReduceCustomerInStore()
    {
        customersInStore--;
    }
}
