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

    public DifficultySettings(int difficultyLevelChosen)
    {
        difficultyLevel = difficultyLevelChosen;

        switch (difficultyLevel)
        {
            case 0:
                numberOfCustomersInWave = 15;
                difficultyString = "Easy";
                numberOfWaves = 3;
                break;

            case 1:
                numberOfCustomersInWave = 10;
                difficultyString = "Medium";
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
                break;

        }

    }

    public void nextshift()
    {
        Shift++;
    }



}
