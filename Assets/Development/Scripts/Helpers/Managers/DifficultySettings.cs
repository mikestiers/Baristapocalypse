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
    private float minDrinkingDurationTime;
    private float maxDrinkingDurationTime;
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

        minDrinkingDurationTime = currentDifficulty.minDrinkingDurationTime;
        maxDrinkingDurationTime = currentDifficulty.maxDrinkingDurationTime;

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

        numberOfWaves = currentDifficulty.InitialnumberOfWaves + Mathf.FloorToInt(Shift * currentDifficulty.RateOfIncreaseInNumberOfWaves);
    }

    public int GetShift()
    {
        return Shift;
    }

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

    public float GetMinDrinkingDurationTime()
    {
        return minDrinkingDurationTime;
    }

    public float GetMaxDrinkingDurationTime()
    {
        return maxDrinkingDurationTime;
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
