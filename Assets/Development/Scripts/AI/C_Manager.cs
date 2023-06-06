using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Manager : MonoBehaviour
{
    public Transform Counter;
    public Transform barEntrance;
    public float delay = 10.0f;
    public CustomerBase customerPrefab;

    private List<CustomerBase> customersOutsideList = new List<CustomerBase>();

    private C_LineQueuing LineQueue;

    //temp stuff -> Ddog will make something more robust
    public GameObject[] Chairs;
    public int chairNumber;



    // Start is called before the first frame update
    private void Start()
    {
        
        List<Vector3> waitingQueuePostionList = new List<Vector3>();
        if (Chairs.Length <= 0) Chairs = GameObject.FindGameObjectsWithTag("Waypoint");


        //where the firstposition is located in scene
        Vector3 firstposition = new Vector3(Counter.position.x, 0, Counter.position.z);
        float positionSize = 2f;
        for (int i = 0; i < 5; i++)
        {
            waitingQueuePostionList.Add(firstposition + new Vector3(0, 0 , 1f) * positionSize * i);
        }
        //LineQueue.OnCustomerArrivedAtFrontOfQueue += WaitingQueue_OnCustomerArrivedAtFrontOfQueue; might be used for future code?
        LineQueue = new C_LineQueuing(waitingQueuePostionList);


        StartCoroutine(NewCustomer(delay));
      

    }

    private void Update()
    {
      


    }

    //maybe randomize time of spawning of customers
    public IEnumerator NewCustomer(float delayS)
    {
        while(true) 
        {
            //yield return new WaitUntil(() -> customers.isServed);
            yield return new WaitForSeconds(delayS);

            //while(gameObject is playin) set timer
            if(customerPrefab != null)
            {

                SpawnCustomer();

                StartCoroutine(CustomerEnterStore());
            }              

        }

    }

    public IEnumerator CustomerEnterStore()
    {
        yield return new WaitForSeconds(2f);

        if (LineQueue.CanAddCustomer() == true) 
        {
            //we can randomize this aswell just set 0 to a random integer from modulo if size of list
            LineQueue.AddCustomer(customersOutsideList[0]);
            customersOutsideList.RemoveAt(0);
        }


        //i just added a delay for when theyre at the counter/ we cam add an event that could trigger them leaving the line and waiting for order
        StartCoroutine(Leaveline()); 
    }

    public IEnumerator Leaveline()
    {
        yield return new WaitForSeconds(25f);

        CustomerBase customer = LineQueue.GetFirstInQueue();



        customer.Walkto(Chairs[chairNumber].transform.position);
        customer.currentState = CustomerBase.CustomerState.Insit;
        chairNumber++;
        chairNumber %= Chairs.Length;

    }

    private void SpawnCustomer()
    {
        CustomerBase Newcustomer = Instantiate(customerPrefab, barEntrance.transform.position, Quaternion.identity);
        Newcustomer.orderRequest = new OrderRequest
        {
            bitterness = UnityEngine.Random.Range(0, 10 + 1),
            sweetness = UnityEngine.Random.Range(0, 10 + 1),
            strength = UnityEngine.Random.Range(0, 10 + 1),
            temperature = UnityEngine.Random.Range(0, 10 + 1),
            biomass = UnityEngine.Random.Range(0, 10 + 1)
        };

        customersOutsideList.Add(Newcustomer);
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
}

[Serializable]
public struct OrderRequest
{
    public int bitterness;
    public int sweetness;
    public int strength;
    public int temperature;
    public int biomass;

}