using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameValueHolder : Singleton<GameValueHolder>
{
    public string DifficultyString;

    [SerializeField] public DifficultySO[] Difficulties; //In Customer Manager for now move to Game Manager

    public DifficultySettings difficultySettings; //will move to GameManager when gamemanager is owki, change references to GameManager aswell

    public DifficultySO currentDifficulty;

    private void Start()
    {
        difficultySettings = new DifficultySettings();
        SetCurrentDifficultyTo("Medium");
    }

    public void SetCurrentDifficultyTo(string difficulty)
    {
        switch (difficulty)
        {
            case "Easy":
                currentDifficulty = Difficulties[0];
                break;

            case "Medium":
                currentDifficulty = Difficulties[1];
                break;

            case "Hard":
                currentDifficulty = Difficulties[2];
                break;

            default:
                currentDifficulty = Difficulties[0];
                break;

        }

        difficultySettings.SetDifficulty(currentDifficulty);
    }

    public void SetPlayerAmount(int playerAmt)
    {
        difficultySettings.SetAmountOfPlayers(playerAmt);
    }

    public void Reset()
    {
        if(difficultySettings != null) difficultySettings.Reset();
    }
}
