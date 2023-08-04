using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScoreTimerManager : Singleton<ScoreTimerManager>
{
    public float timeRemaining = 240f;
    public int score = 0;
    public UnityEvent LoseEvent = new UnityEvent();
    public UnityEvent WinEvent = new UnityEvent();
    [SerializeField] private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0 && gameManager.gameState == GameState.RUNNING)
        {
            UIManager.Instance.finalScore.text = "Score: " + score.ToString();
            if (score >= 50)
            {
                Win();
            }
            else
            {
                Lose();
            }
            gameManager.gameState = GameState.GAMEOVER;
        }  
    }

    private void Lose()
    {
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.gameover);
        UIManager.Instance.gameOverMenu.SetActive(true);
        UIManager.Instance.gameOverText.text = "You Lose!";
        Time.timeScale = 0f;
    }
    private void Win()
    {
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.victory);
        UIManager.Instance.gameOverMenu.SetActive(true);
        UIManager.Instance.gameOverText.text = "You Win!";
        Time.timeScale = 0f;
    }

  /*  I Quoted this out cuz i moved it to customer reactions in customerbase.cs
    
    public void GetScoreComparison(CoffeeAttributes coffeeAttributes, CoffeeAttributes customerAttributes)
    {
        
        int result = 0;
        if (Mathf.Abs(coffeeAttributes.GetSweetness() - customerAttributes.GetSweetness()) <= 5)
        {
            result += 1;
        }
        if (Mathf.Abs(coffeeAttributes.GetBitterness() - customerAttributes.GetBitterness()) <= 5)
        {
            result += 1;
        }
        if (Mathf.Abs(coffeeAttributes.GetSpiciness() - customerAttributes.GetSpiciness()) <= 5)
        {
            result += 1;
        }
        if (Mathf.Abs(coffeeAttributes.GetTemperature() - customerAttributes.GetTemperature()) <= 5)
        {
            result += 1;
        }
        if (Mathf.Abs(coffeeAttributes.GetStrength() - customerAttributes.GetStrength()) <= 5)
        {
            result += 1;
        }
        score += result;
    }
  */
}
