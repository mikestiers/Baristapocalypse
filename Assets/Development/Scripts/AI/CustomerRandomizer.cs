using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CustomerRandomizer : NetworkBehaviour
{
    [SerializeField] public CustomerRaceSO[] Races;
    public List<GameObject> heads = new List<GameObject>();
    public List<GameObject> bodies = new List<GameObject>();
    public CoffeeAttributes coffeePreferences;
    private CustomerRaceSO race;

    // Start is called before the first frame update
    void Start()
    {
        int customerIndex = Random.Range(0, Races.Length);

        race = Races[customerIndex];

        int randomBitterness = Random.Range(race.minBitterness, race.maxBitterness + 1);
        int randomSpiciness = Random.Range(race.minSpiciness, race.maxSpiciness + 1);
        int randomStrength = Random.Range(race.minStrength, race.maxStrength + 1);
        int randomSweetness = Random.Range(race.minSweetness, race.maxSweetness + 1);
        int randomTemperature = Random.Range(race.minTemperature, race.maxTemperature + 1);
        int headIndex = Random.Range(0, heads.Count);
        int bodyIndex = Random.Range(0, bodies.Count);

        StartClientRpc(headIndex, bodyIndex, randomBitterness, randomSpiciness, randomStrength, randomSweetness, randomTemperature);

    }

    [ClientRpc]
    private void StartClientRpc(int headIndex, int bodyIndex, int randomBitterness, int randomSpiciness, int randomStrength, int randomSweetness, int randomTemperature)
    {
        coffeePreferences = GetComponent<CoffeeAttributes>();

        coffeePreferences.AddBitterness(randomBitterness);
        coffeePreferences.AddSpiciness(randomSpiciness);
        coffeePreferences.AddStrength(randomStrength);
        coffeePreferences.AddSweetness(randomSweetness);
        coffeePreferences.AddTemperature(randomTemperature);

        heads[headIndex].SetActive(true);
        bodies[bodyIndex].SetActive(true);
    }
}

