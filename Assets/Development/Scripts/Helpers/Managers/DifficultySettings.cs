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

    public DifficultySettings(int difficultyLevelChosen)
    {
        difficultyLevel = difficultyLevelChosen;

        switch (difficultyLevel)
        {
            case 0:
                numberOfCustomersInWave = 15;
                difficultyString = "Easy";
                numberOfWaves = 3;
                minDelay = 8.0f;
                maxDelay = 15.0f;
                break;

            case 1:
                numberOfCustomersInWave = 5;
                difficultyString = "Medium";
                minDelay = 6.0f;
                maxDelay = 10.0f;
                switch(Shift)
                {
                    case 1: case 2: case 3: case 4:  case 5:
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
                numberOfCustomersInWave = 10;
                difficultyString = "Hard";
                numberOfWaves = Mathf.FloorToInt(4 + Shift * 0.5f);
                minDelay = 6.0f;
                maxDelay = 8.0f;
                break;

        }

    }

    public void NextShift()
    {
        Shift++;
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
}
