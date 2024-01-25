using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CustomerRandomizer : NetworkBehaviour
{
    [SerializeField] public CustomerRaceSO[] Races;
    public List<GameObject> heads = new List<GameObject>();
    public List<GameObject> bodies = new List<GameObject>();
    public GridLayoutGroup cheatIconsLayoutGroup;

    //public List<IngredientSO> temperature = new List<IngredientSO>();
    //public List<IngredientSO> sweetness = new List<IngredientSO>();
    //public List<IngredientSO> strength = new List<IngredientSO>();
    //public List<IngredientSO> spiciness = new List<IngredientSO>();
    
    public IngredientListSO temperatureIngredientList;
    public IngredientListSO sweetnessIngredientList;
    public IngredientListSO strengthIngredientList;
    public IngredientListSO spicinessIngredientList;

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

        temperatureIngredientList = GameManager.Instance.difficultySettings.temperatureIngredientList;
        sweetnessIngredientList = GameManager.Instance.difficultySettings.sweetnessIngredientList;
        strengthIngredientList = GameManager.Instance.difficultySettings.strengthIngredientList;
        spicinessIngredientList = GameManager.Instance.difficultySettings.spicinessIngredientList;

        IngredientSO temperatureIngredient = temperatureIngredientList.ingredientSOList[Random.Range(0, temperatureIngredientList.ingredientSOList.Count)];
        IngredientSO sweetnessIngredient = sweetnessIngredientList.ingredientSOList[Random.Range(0, sweetnessIngredientList.ingredientSOList.Count)];
        IngredientSO spicinessIngredient = spicinessIngredientList.ingredientSOList[Random.Range(0, spicinessIngredientList.ingredientSOList.Count)];
        IngredientSO strenthIngredient = strengthIngredientList.ingredientSOList[Random.Range(0, strengthIngredientList.ingredientSOList.Count)];

        //IngredientSO temperatureIngredient = temperature[Random.Range(0, temperature.Count)];
        //IngredientSO sweetnessIngredient = sweetness[Random.Range(0, sweetness.Count)];
        //IngredientSO strenthIngredient = strength[Random.Range(0, strength.Count)];
        //IngredientSO spicinessIngredient = spiciness[Random.Range(0, spiciness.Count)];

        //temperatureIngredient.icon.transform.SetParent(cheatIconsLayoutGroup.transform);
        //sweetnessIngredient.icon.transform.SetParent(cheatIconsLayoutGroup.transform);
        //strenthIngredient.icon.transform.SetParent(cheatIconsLayoutGroup.transform);
        //spicinessIngredient.icon.transform.SetParent(cheatIconsLayoutGroup.transform);
        
        Debug.Log($"ISO Temperature {this.name}: {temperatureIngredient.name}");
        Debug.Log($"ISO Sweetness {this.name}: {sweetnessIngredient.name}");
        Debug.Log($"ISO Strength {this.name}: {strenthIngredient.name}");
        Debug.Log($"ISO Spiciness {this.name}: {spicinessIngredient.name}");

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

