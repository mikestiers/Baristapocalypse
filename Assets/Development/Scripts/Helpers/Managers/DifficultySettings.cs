using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class DifficultySettings
{
    private float timeBetweenWaves = 30.0f;
    private int numberOfCustomersInWave;
    private int difficultyLevel = 1;
    private string difficultyString = "Medium";
    private int Shift = 1;
    private int numberOfWaves;
    private float minDelay;
    private float maxDelay;
    private int playerCount;

    public DifficultySettings(int difficultyLevelChosen, int InitplayerCount)
    {
        difficultyLevel = difficultyLevelChosen;
        Debug.Log(InitplayerCount);

        switch (difficultyLevel)
        {
            case 0:
                difficultyString = "Easy";
                numberOfCustomersInWave = 15 + Mathf.FloorToInt(InitplayerCount * 1.5f);
                minDelay = 8.0f;
                maxDelay = 15.0f;
                numberOfWaves = 3;

                break;

            case 1:
                difficultyString = "Medium";
                numberOfCustomersInWave = 5 + (InitplayerCount * 2); // set to 5 just for testing purposes
               
                minDelay = 6.0f;
                maxDelay = 10.0f;
                numberOfWaves = 3;

                break;

            case 2:
                numberOfCustomersInWave = 10 + (InitplayerCount * 3);
                difficultyString = "Hard";
                
                minDelay = 6.0f;
                maxDelay = 8.0f;
                numberOfWaves = 4;
                break;

        }


    }

    public void NextShift()
    {
        Shift++;

        switch(difficultyLevel)
        {
            case 0:
                //easy
                minDelay = 8.0f;
                maxDelay = 15.0f;

                numberOfWaves = 3;

                break;

            case 1:
                //medium
                minDelay = 6.0f;
                maxDelay = 10.0f;

                switch (Shift) 
                {
                    case 2: case 3: case 4:case 5:
                        numberOfWaves = 3;
                        break;
                    case 6: case 7: case 8:
                        numberOfWaves = 4;
                        break;
                    case 9: case 10:
                        numberOfWaves = 5;
                        break;

                }

                break;

            case 2:
                //hard
                minDelay = 6.0f;
                maxDelay = 8.0f;

                numberOfWaves = Mathf.FloorToInt(4 + Shift * 0.5f);

                break;

        }


    }

    public int GetShift()
    {
        return Shift;
    }

    public void NextWave()
    {
        numberOfWaves--;

        //decreasing the amount of delay between customers spawning
        minDelay -= 0.5f;
        maxDelay -= 0.5f;

        //adjusting amount of customers based on wave numbers /// increasing amount of customers every wave
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
}
