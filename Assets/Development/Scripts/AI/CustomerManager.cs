using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomerManager : Singleton<CustomerManager>
{
    [SerializeField] private Transform Counter;
    [SerializeField] private Transform barEntrance;
    [SerializeField] private Transform exit;
    [SerializeField] private float delay = 8.0f;
    [SerializeField] private int numberOfCustomers = 5;
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
    public CustomerBase customerPrefab;

    public List<CustomerBase> customersOutsideList = new List<CustomerBase>();

    private CustomerLineQueuing LineQueue;
    private CustomerBarFloor barFloor;

    //temp stuff -> Ddog will make something more robust
    public GameObject[] Chairs;
    //private int chairNumber = 0;

    // Start is called before the first frame update
    private void Start()
    {
        List<Vector3> waitingQueuePostionList = new List<Vector3>();
        if (Chairs.Length <= 0) Chairs = GameObject.FindGameObjectsWithTag("Waypoint");
        //chairNumber = UnityEngine.Random.Range(0, Chairs.Length);

        //where the firstposition is located in scene
        Vector3 firstposition = new Vector3(Counter.position.x, 0, Counter.position.z);
        float positionSize = 2f;
        for (int i = 0; i < numberOfCustomers; i++)
        {
            waitingQueuePostionList.Add(firstposition + new Vector3(0, 0, 1f) * positionSize * i);
        }

        //LineQueue.OnCustomerArrivedAtFrontOfQueue += WaitingQueue_OnCustomerArrivedAtFrontOfQueue; might be used for future code?
        LineQueue = new CustomerLineQueuing(waitingQueuePostionList);
        barFloor = new CustomerBarFloor(Chairs);

        StartCoroutine(NewCustomer(delay));
    }

    //maybe randomize time of spawning of customers
    public IEnumerator NewCustomer(float delayS)
    {
        while (true)
        {
            //yield return new WaitUntil(() -> customers.isServed);
            yield return new WaitForSeconds(delayS);

            //while(gameObject is playin) set timer
            if (customerPrefab != null)
            {
                SpawnCustomer();

                StartCoroutine(CustomerEnterStore());
            }
        }
    }

    public IEnumerator CustomerEnterStore()
    {
        yield return new WaitForSeconds(1f);

        if (LineQueue.CanAddCustomer() == true)
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
        CustomerBase customer = LineQueue.GetFirstInQueue();
        barFloor.TrySendToChair(customer);
    }

    private void SpawnCustomer()
    {
        CustomerBase newcustomer = Instantiate(customerPrefab, barEntrance.transform.position, Quaternion.identity);
        customersOutsideList.Add(newcustomer);
        customerNumber += 1;

        // Assign customer number and choose a random name from the list.  If list becomes empty, no names are assigned
        // We have 25 names so far (the names of everyone on the team), but we can add more
        newcustomer.customerNumber = customerNumber;
        if (customerNames.Count >= 1)
        {
            int randomCustomer = UnityEngine.Random.Range(0, customerNames.Count);
            newcustomer.SetCustomerName(customerNames[randomCustomer]);
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
}
