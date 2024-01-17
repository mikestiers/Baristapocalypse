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
    private int MaxShift = 5; //do determine end of game
    private float chanceToMess;
    private float loiterMessEverySec;
    private float chanceToLoiter; 

    public DifficultySettings(DifficultySO chosenDifficulty, int InitplayerCount)
    {

        currentDifficulty = chosenDifficulty;

        minDelay = chosenDifficulty.minCustomerSpawnDelay;
        maxDelay = chosenDifficulty.maxCustomerSpawnDelay;

        timeBetweenWaves += chosenDifficulty.timeBetweenWaves;

        numberOfWaves = chosenDifficulty.InitialnumberOfWaves;
        numberOfCustomersInWave = chosenDifficulty.numberOfCustomersInWave + Mathf.FloorToInt(InitplayerCount * chosenDifficulty.rateOfIncreaseBasedOnPlayerCount);

        playerCount = InitplayerCount;

        chanceToMess = chosenDifficulty.chanceToMess;

        loiterMessEverySec = chosenDifficulty.loiterMessEverySec;

        chanceToLoiter = chosenDifficulty.chanceToLoiter;

        /*
        switch (currentDifficulty.difficultyString)
        {
            case "Easy":
                numberOfCustomersInWave = chosenDifficulty.InitialnumberOfWaves + Mathf.FloorToInt(InitplayerCount * 1.5f);
                minDelay = 8.0f;
                maxDelay = 15.0f;
                numberOfWaves = 3;

                break;

            case "Medium":
                numberOfCustomersInWave = chosenDifficulty.InitialnumberOfWaves + (InitplayerCount * 2);
               
                minDelay = 6.0f;
                maxDelay = 10.0f;
                numberOfWaves = 3;

                break;

            case "Hard":
                numberOfCustomersInWave = chosenDifficulty.InitialnumberOfWaves + (InitplayerCount * 3);
                
                minDelay = 6.0f;
                maxDelay = 8.0f;
                numberOfWaves = 4;
                break;

        }
        */

    }

    public void NextShift()
    {
        Shift++;

        if(Shift <= MaxShift) 
        {
            //Trigger End Game
            return;
        }


        minDelay = currentDifficulty.minCustomerSpawnDelay;
        maxDelay = currentDifficulty.maxCustomerSpawnDelay;

        numberOfWaves = currentDifficulty.InitialnumberOfWaves + Mathf.FloorToInt(Shift * currentDifficulty.RateOfIncreaseInNumberOfWaves);


    }

    public int GetShift()
    {
        return Shift;
    }

    public void NextWave()
    {
        numberOfWaves--;

        //decreasing the amount of delay between customers spawning
        minDelay -= currentDifficulty.rateOFDecresedDelayOfCustomerSpawn;
        maxDelay -= currentDifficulty.rateOFDecresedDelayOfCustomerSpawn;

        //adjusting amount of customers based on wave numbers /// increasing amount of customers every wave
        /*
        switch (difficultyLevel)
        {
            case 0:
                //easy
                numberOfCustomersInWave += (5 + (2 * playerCount));

                break; 

            case 1:
                //medium
                numberOfCustomersInWave += (5 + (4 * playerCount));

                break;

            case 2:
                //hard
                numberOfCustomersInWave += (5 + (4 * playerCount));

                break;

        }
        */
        numberOfCustomersInWave += Mathf.FloorToInt(currentDifficulty.RateOfIncreaseInNumberOfWaves + (playerCount * currentDifficulty.rateOfIncreaseBasedOnPlayerCount));


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

    public void SetAmountOfPlayers(int playeramount)
    {
       playerCount = playeramount;
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
}
