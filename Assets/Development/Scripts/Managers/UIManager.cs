using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class UIManager : Singleton<UIManager>
{
    [Header("Button")]
    public Button toGame;
    public Button toMain;
    public Button toSettings;
    public Button quit;
    public Button toTutorial;
    public Button toPause;

    [Header("Menu")]
    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject gameOverMenu;
    public GameObject pauseMenu;
    public GameObject tutorialMenu;

    [Header("Text")]
    public Text timer;
    public Text score;

    void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (toGame)
            toGame.onClick.AddListener(StartGame);
        if (toSettings)
            toSettings.onClick.AddListener(ShowSettingsMenu);
        if (quit)
            quit.onClick.AddListener(QuitGame);
        if (toMain)
            toMain.onClick.AddListener(ShowMainMenu);
        if (toTutorial)
            toTutorial.onClick.AddListener(ShowTutorial);
        if (toPause)
            toPause.onClick.AddListener(ShowPause);

    }

    void ShowPause()
    {
        tutorialMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }

    void ShowTutorial()
    {
        pauseMenu.SetActive(false);
        tutorialMenu.SetActive(true);
    }

    void ShowMainMenu()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0))
        {
            settingsMenu.SetActive(false);
            mainMenu.SetActive(true);
        }
        else 
        {
            SceneManager.LoadScene(0);
            pauseMenu.SetActive(false);
        }
    }

    void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Aplication.Quit();
        #endif
    }

    void ShowSettingsMenu()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        timer.text = ScoreTimerManager.Instance.timeRemaining.ToString("n2");
        score.text = ScoreTimerManager.Instance.score.ToString();
    }
}
