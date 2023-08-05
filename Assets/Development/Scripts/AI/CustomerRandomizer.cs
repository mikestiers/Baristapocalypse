using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerRandomizer : MonoBehaviour
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
        race= Races[customerIndex];
        
        coffeePreferences = GetComponent<CoffeeAttributes>();

        coffeePreferences.AddBitterness(UnityEngine.Random.Range(race.minBitterness, race.maxBitterness + 1));
        coffeePreferences.AddSpiciness(UnityEngine.Random.Range(race.minSpiciness, race.maxSpiciness + 1));
        coffeePreferences.AddStrength(UnityEngine.Random.Range(race.minStrength, race.maxStrength +1));
        coffeePreferences.AddSweetness(UnityEngine.Random.Range(race.minSweetness, race.maxSweetness + 1));
        coffeePreferences.AddTemperature(UnityEngine.Random.Range(race.minTemperature, race.maxTemperature + 1));



        int headIndex = Random.Range(0, heads.Count);
        int bodyIndex = Random.Range(0, bodies.Count);
        heads[headIndex].SetActive(true);
        bodies[bodyIndex].SetActive(true);
     
    }
}

