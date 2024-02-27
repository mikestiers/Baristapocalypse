using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class DifficultySettings
{

    private DifficultySO currentDifficulty;

    private float timeBetweenWaves;
    private int numberOfCustomersInWave;
    private int Shift = 1;
    private int numberOfWaves;
    private float minDelay;
    private float maxDelay;
    private int playerCount;
    public int MaxShift;
    private float chanceToMess;
    private float loiterMessEverySec;
    private float chanceToLoiter;
    private float minWaitTime;
    private float maxWaitTime;
    private float minInLineWaitTime;
    private float maxInLineWaitTime;
    private float drinkThreshold;
    private int moneyToPass;
    public IngredientListSO temperatureIngredientList { get; set; }
    public IngredientListSO sweetnessIngredientList { get; set; }
    public IngredientListSO strengthIngredientList { get; set; }
    public IngredientListSO spicinessIngredientList { get; set; }
    public IngredientListSO allIngredientsList { get; set; }

    public DifficultySettings()
    {
        playerCount = 1;
    }

    private void UpdateDifficulty()
    {
        minDelay = currentDifficulty.minCustomerSpawnDelay - Mathf.FloorToInt(playerCount * 6);
        maxDelay = currentDifficulty.maxCustomerSpawnDelay - Mathf.FloorToInt(playerCount * 6);

        timeBetweenWaves = currentDifficulty.timeBetweenWaves;

        numberOfWaves = currentDifficulty.InitialnumberOfWaves;
        numberOfCustomersInWave = currentDifficulty.numberOfCustomersInWave + Mathf.FloorToInt(playerCount * currentDifficulty.rateOfIncreaseBasedOnPlayerCount);
        MaxShift = currentDifficulty.maxShift;

        chanceToMess = currentDifficulty.chanceToMess;

        loiterMessEverySec = currentDifficulty.loiterMessEverySec;

        chanceToLoiter = currentDifficulty.chanceToLoiter;

        minWaitTime = currentDifficulty.minWaitTime;
        maxWaitTime = currentDifficulty.maxWaitTime;

        minInLineWaitTime = currentDifficulty.minInLineWaitTime;
        maxInLineWaitTime = currentDifficulty.maxInLineWaitTime;

        drinkThreshold = currentDifficulty.drinkThreshold;

        temperatureIngredientList = currentDifficulty.temperatureIngredientList;
        sweetnessIngredientList = currentDifficulty.sweetnessIngredientList;
        strengthIngredientList = currentDifficulty.strengthIngredientList;
        spicinessIngredientList = currentDifficulty.spicinessIngredientList;

        allIngredientsList = currentDifficulty.allIngredientsList;

        moneyToPass = currentDifficulty.moneyToPass;

    }

    public void SetDifficulty(DifficultySO choosenDiff)
    {
        currentDifficulty = choosenDiff;
        UpdateDifficulty();
    }

    public void SetAmountOfPlayers(int playeramount)
    {
        playerCount = playeramount;
        UpdateDifficulty();
    }

    public void NextShift()
    {
        Debug.Log("Shift: " + Shift);
        if (Shift == MaxShift)
        {
            //Trigger End Game
            GameManager.Instance.iSEndGame = true;
            UIManager.Instance.shiftEvaluationUI.SetActive(false);

            return;
        }
        else
        {
            Debug.Log("Shift: " + Shift);
            UIManager.Instance.shiftEvaluationUI.SetActive(true);
            Shift++;
        }

        minDelay = currentDifficulty.minCustomerSpawnDelay;
        maxDelay = currentDifficulty.maxCustomerSpawnDelay;

        numberOfWaves = currentDifficulty.InitialnumberOfWaves; // + Mathf.FloorToInt(Shift * currentDifficulty.RateOfIncreaseInNumberOfWaves);
    }

    public int GetShift()
    {
        return Shift;
    }

    //nothing is calling this
    //public void NextWave()
    //{
    //    numberOfWaves--;

    //    //decreasing the amount of delay between customers spawning
    //    minDelay -= currentDifficulty.rateOFDecresedDelayOfCustomerSpawn;
    //    maxDelay -= currentDifficulty.rateOFDecresedDelayOfCustomerSpawn;

    //    //adjusting amount of customers based on wave numbers /// increasing amount of customers every wave
    //    /*
    //    switch (difficultyLevel)
    //    {
    //        case 0:
    //            //easy
    //            numberOfCustomersInWave += (5 + (2 * playerCount));

    //            break; 

    //        case 1:
    //            //medium
    //            numberOfCustomersInWave += (5 + (4 * playerCount));

    //            break;

    //        case 2:
    //            //hard
    //            numberOfCustomersInWave += (5 + (4 * playerCount));

    //            break;

    //    }
    //    */
    //    numberOfCustomersInWave += Mathf.FloorToInt(currentDifficulty.RateOfIncreaseInNumberOfWaves + (playerCount * currentDifficulty.rateOfIncreaseBasedOnPlayerCount));


    //}


    public int GetNumberOfWaves()
    {
        return numberOfWaves;
    }

    public int GetNumberofCustomersInwave()
    {
        return numberOfCustomersInWave;
    }

    public float GetTimeBetweenWaves()
    {
        return timeBetweenWaves;
    }

    public float GetMinDelay()
    {
        return minDelay;
    }

    public float GetMaxDelay()
    {
        return maxDelay;
    }

    public float GetChanceToMess()
    {
        return chanceToMess;
    }

    public float GetLoiterMessEverySec()
    {
        return loiterMessEverySec;
    }

    public float GetChanceToLoiter()
    {
        return chanceToLoiter;
    }

    public float GetMinWaitTime()
    {
        return minWaitTime;
    }

    public float GetMaxWaitTime()
    {
        return maxWaitTime;
    }

    public float GetDrinkThreshold()
    {
        return drinkThreshold;
    }

    public int GetMoneyToPass()
    {
        return moneyToPass;
    }

    public float GetMinInLineWaitTime()
    {
        return minInLineWaitTime;
    }

    public float GetMaxInLineWaitTime()
    {
        return maxInLineWaitTime;
    }
}
