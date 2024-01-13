using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ScoreTimerManager : Singleton<ScoreTimerManager>
{
    public NetworkVariable<float> timeRemainingDefault = new NetworkVariable<float>(20f);
    public NetworkVariable<float> timeRemaining;
    public int score = 0;
    public UnityEvent LoseEvent = new UnityEvent();
    public UnityEvent WinEvent = new UnityEvent();
    public int StreakCount { get; private set; }
    private string activeGameScene = "T5M3_BUILD";

    // Start is called before the first frame update
    void Start()
    {
        timeRemaining = timeRemainingDefault;
    }

    protected override void Awake()
    {
        base.Awake();
        // Initialize streakCount
        StreakCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(SceneManager.GetActiveScene().name == activeGameScene) //
        {
            timeRemaining.Value -= Time.deltaTime;
            if (timeRemaining.Value <= 0 && GameManager.Instance.gameState == GameState.RUNNING)
            {
                timeRemaining = new NetworkVariable<float>(0);
                score *= Mathf.FloorToInt(CustomerReview.GetAverageReviewScore());
                Debug.Log($"score {score}");
                UIManager.Instance.finalScore.text = "Score: " + score.ToString();
                if (score >= 50)
                {
                    Win();
                }
                else
                {
                    Lose();
                }
                GameManager.Instance.gameState = GameState.GAMEOVER;
            }
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
    public void IncrementStreak()
    {
        StreakCount++;
    }

    public void ResetStreak()
    {
        StreakCount = 0;
    }

    public void ResetTimerScore()
    {
        timeRemaining = timeRemainingDefault;
        score = 0;

    }

}
