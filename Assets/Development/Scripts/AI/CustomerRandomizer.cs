using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class CustomerRandomizer : NetworkBehaviour
{
    [SerializeField] public CustomerRaceSO[] Races;
    public List<GameObject> heads = new List<GameObject>();
    public List<GameObject> bodies = new List<GameObject>();
    public List<GameObject> cupHoldPointList = new List<GameObject>();
    [HideInInspector] public GameObject currentCustomerHoldPoint;
    public GameObject currentVFXHoldPoint;
    public GameObject currentHead;
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

        temperatureIngredientList = GameValueHolder.Instance.difficultySettings.temperatureIngredientList;
        sweetnessIngredientList = GameValueHolder.Instance.difficultySettings.sweetnessIngredientList;
        strengthIngredientList = GameValueHolder.Instance.difficultySettings.strengthIngredientList;
        spicinessIngredientList = GameValueHolder.Instance.difficultySettings.spicinessIngredientList;

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



        //int headIndex = Random.Range(0, heads.Count);
        int bodyIndex = Random.Range(0, bodies.Count);
        currentCustomerHoldPoint = cupHoldPointList[bodyIndex];

        currentHead = heads[bodyIndex];
        currentVFXHoldPoint.transform.SetParent(currentHead.transform);
        
        
        StartClientRpc(bodyIndex, accumulatedSpiciness, accumulatedStrength, accumulatedSweetness, accumulatedTemperature);
    }

    [ClientRpc]
    private void StartClientRpc(int bodyIndex, int accumulatedSpiciness, int accumulatedStrength, int accumulatedSweetness, int accumulatedTemperature)
    {
        coffeePreferences = GetComponent<CoffeeAttributes>();

        coffeePreferences.AddSpiciness(accumulatedSpiciness);
        coffeePreferences.AddStrength(accumulatedStrength);
        coffeePreferences.AddSweetness(accumulatedSweetness);
        coffeePreferences.AddTemperature(accumulatedTemperature);

        //heads[headIndex].SetActive(true);
        for(int i=0; i<bodies.Count; i++)
        {
            if (i == bodyIndex) continue;
            bodies[i].SetActive(false);
        }
        
    }
}

