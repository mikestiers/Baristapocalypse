using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CustomerManager : Singleton<CustomerManager>
{
    //[SerializeField] public GameObject cashRegister; // where customers line up
    //[SerializeField] public GameObject door;
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
    List<FixedString32Bytes> customerNames = new List<FixedString32Bytes>
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

    private bool IsServing = true;
    private bool isSpawningCustomers = false;

    private void Start()
    {
        List<Vector3> waitingQueuePostionList = new List<Vector3>();
        if (Chairs.Length <= 0) Chairs = GameObject.FindGameObjectsWithTag("Waypoint");

        //where the firstposition is located in scene
        Vector3 firstposition = new Vector3(Counter.position.x, 0, Counter.position.z - 1.5f);
        float positionSize = 2f;
        for (int i = 0; i < numberOfCustomers; i++)
        {
            waitingQueuePostionList.Add(firstposition - new Vector3(0, 0, 1f) * positionSize * i);
        }

        LineQueue = new CustomerLineQueuing(waitingQueuePostionList);
        barFloor = new CustomerBarFloor(Chairs);

        customersLeftinWave = GameValueHolder.Instance.difficultySettings.GetNumberofCustomersInwave();
        WavesLeft = GameValueHolder.Instance.difficultySettings.GetNumberOfWaves();
        Debug.Log($"WavesLeft {WavesLeft}");

        minDelay = GameValueHolder.Instance.difficultySettings.GetMinDelay();
        maxDelay = GameValueHolder.Instance.difficultySettings.GetMaxDelay();
        
        float delay = UnityEngine.Random.Range(minDelay, maxDelay);

        if(initCustomerSpawnDelay < 8f) initCustomerSpawnDelay = 8f;

        if (IsOwner)
        {
            StartCoroutine(NewCustomer(initCustomerSpawnDelay)); // change this to intial delay
        }

        currentServingState = ServingState.CurrentlyServing;
        timer = GameValueHolder.Instance.difficultySettings.GetTimeBetweenWaves();
        Debug.Log($"timer {timer}");
        IsServing = true;
    }

    private void Update()
    {
        if (!IsServer) { return; }

        if (GameManager.Instance.IsGamePlaying())
        {
            //Debug.Log($"currentServingState {currentServingState}");
            switch (currentServingState)
            {
                case ServingState.CurrentlyServing:
                    if (GetCustomerLeftinStore() <= 0 && isSpawningCustomers == true)
                    {
                        isSpawningCustomers = false;
                        currentServingState = ServingState.BreakTime;
                        NextWave();
                    }
                    break;

                case ServingState.BreakTime:
                    timer -= Time.deltaTime;
                    Debug.Log($"timer {timer}");

                    if (timer <= 0)
                    {
                        currentServingState = ServingState.CurrentlyServing;
                    }

                    UpdateTimeUIClientRpc(timer);

                    break;

                case ServingState.ShiftOver:

                    break;
            }
        }
    }

    //maybe randomize time of spawning of customers
    public IEnumerator NewCustomer(float delayS)
    {
        yield return new WaitForSeconds(delayS);

        //while(gameObject is playin) set timer
        if (customerPrefab != null)
        {
            SpawnCustomer();
            StartCoroutine(CustomerEnterStore());
            customersInStore++;
            customersLeftinWave--;
            //UIManager.Instance.customersInStore.text = ("Customers in Store: ") + customersInStore.ToString();
            //UIManager.Instance.customersLeft.text = ("SpawnLeft: " + customersLeftinWave.ToString());

            isSpawningCustomers = true;
        }

        float delay = UnityEngine.Random.Range(minDelay, maxDelay);

        if (customersLeftinWave <= 0)
        {
            UIManager.Instance.SayGameMessage("Last Customer!"); // UI Warning
            yield break;
        }
        else StartCoroutine(NewCustomer(delay));

    }

    // Trigger Time Between waves
    public void NextWave()
    {
        if (GameManager.Instance.IsGamePlaying() == false) return;

        WavesLeft--;
        Debug.Log($"WavesLeft-- {WavesLeft}");
        if (WavesLeft <= 0)
        {
            currentServingState = ServingState.ShiftOver;
        }
        
        if (currentServingState == ServingState.ShiftOver)
        {
            if (GameValueHolder.Instance.difficultySettings.GetShift() == GameValueHolder.Instance.difficultySettings.MaxShift)
            {
                GameValueHolder.Instance.difficultySettings.NextShift(); // Will determine if showing end game screen
            }
            else if (GameValueHolder.Instance.difficultySettings.GetShift() < GameValueHolder.Instance.difficultySettings.MaxShift)
            {
                UIManager.Instance.ShowShiftEvaluation();
                GameValueHolder.Instance.difficultySettings.NextShift();
                WavesLeft = GameValueHolder.Instance.difficultySettings.GetNumberOfWaves();
                currentServingState = ServingState.CurrentlyServing;
                Debug.Log($"WavesLeft Shift End {WavesLeft}");
            }
        }

        if (WavesLeft > 0)
        {
            UIManager.Instance.SayGameMessage("Break Time!");
            timer = GameValueHolder.Instance.difficultySettings.GetTimeBetweenWaves();
            customersLeftinWave = GameValueHolder.Instance.difficultySettings.GetNumberofCustomersInwave();
            StartCoroutine(RestPeriod(timer));
            Debug.Log($"WavesLeft > 0 {WavesLeft}");
        }
    }

    //Timer for Inbetween Waves
    public IEnumerator RestPeriod(float timer)
    {
        yield return new WaitForSeconds(timer);

        //Trigger UI
        UIManager.Instance.SayGameMessage("The Customers are coming!");

        StartCoroutine(NewCustomer(initCustomerSpawnDelay));
    }

    [ClientRpc]
    public void UpdateTimeUIClientRpc(float timer)
    {
        if (timer <= 0)
        {
            UIManager.Instance.ToggleBigTimer(false);
            //end UI & end RestState
        }
        else if (timer > 0 && timer < 5)
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
    }

    public IEnumerator CustomerEnterStore()
    {
        yield return new WaitForSeconds(1f);

        if (TutorialManager.Instance != null && TutorialManager.Instance.tutorialEnabled && !TutorialManager.Instance.firstOrderTaken)
            TutorialManager.Instance.TakeFirstOrder();

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
        SpawnCustomerServerRpc();
    }

    [ServerRpc]
    private void SpawnCustomerServerRpc()
    {
        Transform newCustomerTransform = Instantiate(customerPrefab.transform, barEntrance.transform.position, Quaternion.identity);

        NetworkObject newCustomer = newCustomerTransform.GetComponent<NetworkObject>();  
        CustomerBase newCustomerBase = newCustomer.GetComponent<CustomerBase>();

        customersOutsideList.Add(newCustomerBase);
        customerNumber += 1;

        newCustomerBase.customerNumber.Value = customerNumber;
        if(customerNames.Count >= 1)
        {
            int randomCustomerNameIndex = UnityEngine.Random.Range(0, customerNames.Count);
            newCustomerBase.SetCustomerName(customerNames[randomCustomerNameIndex]);
            customerNames.RemoveAt(randomCustomerNameIndex);
        }

        newCustomer.Spawn(true);
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

