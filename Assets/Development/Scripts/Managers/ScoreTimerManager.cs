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

    // Start is called before the first frame update
    void Start()
    {
        LoseEvent?.AddListener(Lose);
        WinEvent?.AddListener(Win);
    }

    // Update is called once per frame
    void Update()
    {
        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0 && GameManager.Instance.gameState == GameState.RUNNING)
        {
            LoseEvent?.Invoke();
            GameManager.Instance.gameState = GameState.LOST;
            Debug.LogError("lose");
        }

        if (score == 100)
        {
            WinEvent?.Invoke();
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

    public void GetScoreComparison(CoffeeAttributes coffeeAttributes, OrderRequest customerOrder)
    {
        
        int result = 0;
        if (Mathf.Abs(coffeeAttributes.GetSweetness() - customerOrder.sweetness) <= 10)
        {
            result += 1;
        }
        if (Mathf.Abs(coffeeAttributes.GetBitterness() - customerOrder.bitterness) <= 10)
        {
            result += 1;
        }
        if (Mathf.Abs(coffeeAttributes.GetSpiciness() - customerOrder.spiciness) <= 10)
        {
            result += 1;
        }
        if (Mathf.Abs(coffeeAttributes.GetTemperature() - customerOrder.temperature) <= 10)
        {
            result += 1;
        }
        if (Mathf.Abs(coffeeAttributes.GetStrength() - customerOrder.strength) <= 10)
        {
            result += 1;
        }
        score += result;
    }
}
