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

    public List<IngredientSO> temperature = new List<IngredientSO>();
    public List<IngredientSO> sweetness = new List<IngredientSO>();
    public List<IngredientSO> strength = new List<IngredientSO>();
    public List<IngredientSO> spiciness = new List<IngredientSO>();

    // DELETE AFTER TESTING //
    public IngredientSO sweet;
    public IngredientSO str;
    public IngredientSO temp;
    public IngredientSO spc;
    ///-------------------///

    public CoffeeAttributes coffeePreferences;
    private CustomerRaceSO race;

    // Start is called before the first frame update
    void Start()
    {
        int customerIndex = Random.Range(0, Races.Length);

        race = Races[customerIndex];

        IngredientSO temperatureIngredient = temperature[Random.Range(0, temperature.Count)];
        IngredientSO sweetnessIngredient = sweetness[Random.Range(0, sweetness.Count)];
        IngredientSO strenthIngredient = strength[Random.Range(0, strength.Count)];
        IngredientSO spicinessIngredient = spiciness[Random.Range(0, spiciness.Count)];

        // DELETE AFTER TESTING //
        sweet = sweetnessIngredient;
        str = strenthIngredient;
        temp = temperatureIngredient;
        spc = spicinessIngredient;
        // -----------------------  //

        int accumulatedTemperature = temperatureIngredient.temperature + sweetnessIngredient.temperature + strenthIngredient.temperature + spicinessIngredient.temperature;
        int accumulatedSweetness = temperatureIngredient.sweetness + sweetnessIngredient.sweetness + strenthIngredient.sweetness + spicinessIngredient.sweetness;
        int accumulatedStrength = temperatureIngredient.strength + sweetnessIngredient.strength + strenthIngredient.strength + spicinessIngredient.strength;
        int accumulatedSpiciness = temperatureIngredient.spiciness + sweetnessIngredient.spiciness + strenthIngredient.spiciness + spicinessIngredient.spiciness;



        int headIndex = Random.Range(0, heads.Count);
        int bodyIndex = Random.Range(0, bodies.Count);

        StartClientRpc(headIndex, bodyIndex, accumulatedSpiciness, accumulatedStrength, accumulatedSweetness, accumulatedTemperature);
    }

    [ClientRpc]
    private void StartClientRpc(int headIndex, int bodyIndex, int accumulatedSpiciness, int accumulatedStrength, int accumulatedSweetness, int accumulatedTemperature)
    {
        coffeePreferences = GetComponent<CoffeeAttributes>();

        coffeePreferences.AddSpiciness(accumulatedSpiciness);
        coffeePreferences.AddStrength(accumulatedStrength);
        coffeePreferences.AddSweetness(accumulatedSweetness);
        coffeePreferences.AddTemperature(accumulatedTemperature);

        heads[headIndex].SetActive(true);
        bodies[bodyIndex].SetActive(true);
    }
}

