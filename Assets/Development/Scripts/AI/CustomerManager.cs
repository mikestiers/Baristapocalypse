using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CustomerManager : Singleton<CustomerManager>
{
    [SerializeField] private Transform Counter;
    [SerializeField] private Transform barEntrance;
    [SerializeField] private Transform exit;
    [SerializeField] private float minDelay = 6.0f;
    [SerializeField] private float maxDelay = 10.0f; //difficulty change
    [SerializeField] private int numberOfCustomers = 5;
    [SerializeField] private int customersLeftinWave;
    [SerializeField] private int WavesLeft;
    [SerializeField] private int customersInStore = 0;
    [SerializeField] private float initCustomerSpawnDelay;
    private float timer;
    private ServingState currentServingState;
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

    public enum ServingState
    {
        CurrentlyServing,
        BreakTime,
        ShiftOver
    }


    public NetworkObject customerPrefab;

    public List<CustomerBase> customersOutsideList = new List<CustomerBase>();

    public CustomerLineQueuing LineQueue;
    public CustomerBarFloor barFloor;
    public DifficultySO currentDifficulty;

    private NetworkObject newcustomer;

    //temp stuff -> Ddog will make something more robust
    public GameObject[] Chairs;
    //private int chairNumber = 0;

    //Shift Evaluation values
    private int customerServed = 0;
    private int customerLeave = 0;

    private void Start()
    {

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

        customersLeftinWave = GameManager.Instance.difficultySettings.GetNumberofCustomersInwave();
        WavesLeft = GameManager.Instance.difficultySettings.GetNumberOfWaves();

        UIManager.Instance.customersLeft.text = ("SpawnLeft: " + customersLeftinWave.ToString());
        UIManager.Instance.spawnMode.text = "Serving Customers";
        UIManager.Instance.shift.text = ("Shift " + GameManager.Instance.difficultySettings.GetShift().ToString());
        UIManager.Instance.wavesleft.text = ("Waves Left: " + WavesLeft.ToString());

        minDelay = GameManager.Instance.difficultySettings.GetMinDelay();
        maxDelay = GameManager.Instance.difficultySettings.GetMaxDelay();
        
        float delay = UnityEngine.Random.Range(minDelay, maxDelay);

        if(initCustomerSpawnDelay < 8f) initCustomerSpawnDelay = 8f;


        StartCoroutine(NewCustomer(initCustomerSpawnDelay)); // change this to intial delay

        currentServingState = ServingState.CurrentlyServing;
    }

    private void Update()
    {
        switch (currentServingState)
        {
            case ServingState.CurrentlyServing:

                break;

            case ServingState.BreakTime:
               
                timer -= Time.deltaTime;

                if(timer <= 0)
                {
                    UIManager.Instance.ToggleBigTimer(false);
                    //end UI & end RestState
                    //StartCoroutine(NewCustomer(initCustomerSpawnDelay));
                    currentServingState= ServingState.CurrentlyServing;
                }
                else if(timer > 0 && timer < 5)
                {
                    //use big timer & hide small timer
                    UIManager.Instance.countdownText.text = Math.Ceiling(timer).ToString();
                    UIManager.Instance.ToggleBigTimer(true);
                    UIManager.Instance.ToggleSmalltimer(false);
                }
                else
                {
                    //update small timer
                    UIManager.Instance.gameTimerText.text = Math.Ceiling(timer).ToString();
                    UIManager.Instance.ToggleSmalltimer(true);
                }

                break;

            case ServingState.ShiftOver:

                break;
        }
    }

    //maybe randomize time of spawning of customers
    public IEnumerator NewCustomer(float delayS)
    {
        yield return new WaitForSeconds(delayS);

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

                if (customersLeftinWave <= 0)
                {
                    UIManager.Instance.SayGameMessage("Last Customer!"); // UI Warning
                    yield break;
                }
            }
        }
    }

    // Trigger Time Between waves
    public void NextWave()
    {
        UIManager.Instance.SayGameMessage("Break Time!");

        timer = GameManager.Instance.difficultySettings.GetTimeBetweenWaves();
        currentServingState = ServingState.BreakTime;
        WavesLeft--;
        if (WavesLeft <= 0) 
        {
            currentServingState = ServingState.ShiftOver;
            GameManager.Instance.difficultySettings.NextShift();
            WavesLeft = GameManager.Instance.difficultySettings.GetNumberOfWaves();
            UIManager.Instance.shift.text = ("Shift " + GameManager.Instance.difficultySettings.GetShift().ToString());
            //Shift Evaluation
            UIManager.Instance.ShowShiftEvaluation();
        }
        
        customersLeftinWave = GameManager.Instance.difficultySettings.GetNumberofCustomersInwave();

        UIManager.Instance.wavesleft.text = ("Waves Left: " + WavesLeft.ToString());
        //UIManager.Instance.customersLeft.text = ("SpawnLeft: " + customersLeftinWave.ToString());

        //Trigger UI
        //UIManager.Instance.spawnMode.text = "Resting";

        StartCoroutine(RestPeriod(GameManager.Instance.difficultySettings.GetTimeBetweenWaves()));

    }

    //Timer for Inbetween Waves
    public IEnumerator RestPeriod(float timer)
    {
        yield return new WaitForSeconds(timer);

        //Trigger UI
        //UIManager.Instance.spawnMode.text = "Serving Customers";
        //UIManager.Instance.customersLeft.text = ("SpawnLeft: " + customersLeftinWave.ToString());
        UIManager.Instance.SayGameMessage("The Customers are coming!");

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

        if (LineQueue.CanAddCustomer() == true)
        {
            LineQueue.AddCustomer(customersOutsideList[0]);
            customersOutsideList.RemoveAt(0);
        }
        else
        {
            customersOutsideList[0].CustomerLeave();
            customersOutsideList.RemoveAt(0);
            customerLeaveIncrease();
        }
    }

    public void Leaveline()
    {
        CustomerBase customer = LineQueue.GetFirstInQueue();
        barFloor.TrySendToChair(customer);
        customer.inLine = false;
        customer.frontofLine = false;
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

    public void customerServedIncrease()
    {
        customerServed++;
    }

    public void customerLeaveIncrease()
    {
        customerLeave++;
    }

    public int GetCustomerServed()
    {
        return customerServed;
    }

    public int GetCustomerLeave()
    {
        return customerLeave;
    }
}