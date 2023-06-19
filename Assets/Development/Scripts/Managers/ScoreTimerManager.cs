using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScoreTimerManager : Singleton<ScoreTimerManager>
{
    public float timeRemaining = 1f;
    public int score;
    public UnityEvent LoseEvent = new UnityEvent();
    public UnityEvent WinEvent = new UnityEvent();
    private GameManager gameManager;

    // Start is called before the first frame update
    public AudioClip winsong;
    public AudioClip losesong;
    void Start()
    {
        LoseEvent?.AddListener(Lose);
        WinEvent?.AddListener(Win);
    }

    // Update is called once per frame
    void Update()
    {
        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0 && gameManager.gameState == GameState.RUNNING)
        {
            LoseEvent?.Invoke();
            gameManager.gameState = GameState.LOST;
            AudioManager.Instance.Playoneshot(losesong, false);
            Debug.LogError("lose");
        }

        if (score == 100)
        {
            WinEvent?.Invoke();
            AudioManager.Instance.Playoneshot(winsong, false);
            Debug.LogError("win");
        }
    }
    private void Lose()
    {
        UIManager.Instance.gameOverMenu.SetActive(true);
        Time.timeScale = 0f;
    }
    private void Win()
    {
        UIManager.Instance.gameOverMenu.SetActive(true);
        Time.timeScale = 0f;
    }

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
}
