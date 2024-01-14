using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ScoreTimerManager : Singleton<ScoreTimerManager>
{
    //public float timeRemainingDefault = 300f;
    //public float timeRemaining;
    //public int score = 0;
    //public UnityEvent LoseEvent = new UnityEvent();
    //public UnityEvent WinEvent = new UnityEvent();
    //public int StreakCount { get; private set; }
    //private string activeGameScene = "T5M3_BUILD";

    //// Start is called before the first frame update
    //void Start()
    //{
    //    timeRemaining = timeRemainingDefault;
    //}

    //protected override void Awake()
    //{
    //    base.Awake();
    //    // Initialize streakCount
    //    StreakCount = 0;
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    if(SceneManager.GetActiveScene().name == activeGameScene) //
    //    {
    //        timeRemaining -= Time.deltaTime;
    //        if (timeRemaining <= 0 && GameManager.Instance.gameState == GameState.RUNNING)
    //        {
    //            timeRemaining = 0;
    //            score *= Mathf.FloorToInt(CustomerReview.GetAverageReviewScore());
    //            Debug.Log($"score {score}");
    //            UIManager.Instance.finalScore.text = "Score: " + score.ToString();
    //            if (score >= 50)
    //            {
    //                WinServerRpc();
    //            }
    //            else
    //            {
    //                LoseServerRpc();
    //            }
                
    //            GameManager.Instance.gameState = GameState.GAMEOVER;
    //        }
    //    }
        
    //}

    //[ServerRpc(RequireOwnership = false)]
    //public void LoseServerRpc()
    //{
    //    LoseClientRpc();
    //}


    //[ClientRpc]
    //public void LoseClientRpc()
    //{
    //    Lose();
    //}

    //public void Lose()
    //{
    //    SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.gameover);
    //    UIManager.Instance.gameOverMenu.SetActive(true);
    //    UIManager.Instance.gameOverText.text = "You Lose!";
    //    Time.timeScale = 0f;
    //}

    //[ServerRpc(RequireOwnership = false)]
    //public void WinServerRpc()
    //{
    //    WinClientRpc();
    //}

    //[ClientRpc]
    //public void WinClientRpc()
    //{
    //    Win();
    //}

    //public void Win()
    //{
    //    SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.victory);
    //    UIManager.Instance.gameOverMenu.SetActive(true);
    //    UIManager.Instance.gameOverText.text = "You Win!";
    //    Time.timeScale = 0f;
    //}

    //public void IncrementStreak()
    //{
    //    StreakCount++;
    //}

    //public void ResetStreak()
    //{
    //    StreakCount = 0;
    //}

    //public void ResetTimerScore()
    //{
    //    timeRemaining = timeRemainingDefault;
    //    score = 0;
    //}

}
