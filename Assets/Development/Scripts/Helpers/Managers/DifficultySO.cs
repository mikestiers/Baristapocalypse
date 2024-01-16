using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class DifficultySO : ScriptableObject
{

    public string difficultyString;

    [Header("Shift Settings")]
    public float timeBetweenWaves;
    public int InitialnumberOfWaves;
    public float RateOfIncreaseInNumberOfWaves; //0.0f ,weird number for medium, 0.5f
    public float targetBudget;

    [Header("Number of Customers Per Wave")]
    public int numberOfCustomersInWave;
    public float rateOfIncreaseBasedOnPlayerCount;

    [Header("Customer Spawn Rate Values")]
    public float minCustomerSpawnDelay;
    public float maxCustomerSpawnDelay;
    public float rateOFDecresedDelayOfCustomerSpawn; //0.5f original script

    public float chanceToMess;


}
