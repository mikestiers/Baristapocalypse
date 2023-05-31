using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawnerManager : MonoBehaviour
{
    public static CustomerSpawnerManager singleton;
    public List<Transform> customersSpawn = new List<Transform>();
    public CustomerClass1 customerPrefab;
    
    public CustomerSpawnerManager()
    {
        if (singleton == null)
            singleton = this;
    }

    public void SpawnRandomCustomers(int customerCount)
    {
        for (int i = 0; i < customerCount; i++)
        {
            SpawnCustomer();
        }
    }

    private void SpawnCustomer()
    {
        int customerSpawnIndex = UnityEngine.Random.Range(0, customersSpawn.Count);
        CustomerClass1 customer = Instantiate(customerPrefab.gameObject, customersSpawn[customerSpawnIndex].position, Quaternion.identity).GetComponent<CustomerClass1>();
        customer.orderRequest = new OrderRequest
        {
            bitterness = UnityEngine.Random.Range(0, 10 + 1),
            sweetness = UnityEngine.Random.Range(0, 10 + 1),
            strength = UnityEngine.Random.Range(0, 10 + 1),
            temperature = UnityEngine.Random.Range(0, 10 + 1),
            biomass = UnityEngine.Random.Range(0, 10 + 1)
        };
    }

    private void Start()
    {
        SpawnRandomCustomers(3);
    }
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
