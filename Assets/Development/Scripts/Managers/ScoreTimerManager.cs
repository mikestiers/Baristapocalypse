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

    public int GetScoreComparison(CoffeeAttributes coffeeAttributes, OrderRequest customerOrder)
    {
        //math.abs(coffeeAttributes.sweetness - customerOrder.sweetness)
        int result = 0;
        if (coffeeAttributes.GetSweetness() == customerOrder.sweetness)
            result += 1;
        if (coffeeAttributes.GetBitterness() == customerOrder.bitterness)
            result += 1;
        return result;
    }
}
