using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Audio;


public class UIManager : Singleton<UIManager>
{
    public AudioMixer AudioMixer;
    [Header("Button")]
    public Button toGame;
    public Button toMain;
    public Button toSettings;
    public Button quit;

    [Header("Menu")]
    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject gameOverMenu;
    public GameObject pauseMenu;

    [Header("Text")]
    public Text timer;
    public Text score;


    [Header("Menu Music")]
    [SerializeField] private AudioClip menuSong1;
    [SerializeField] private AudioClip pauseMusic;
    [Header("Background music")]
    [SerializeField] private AudioClip BGMSong1;
    [SerializeField] private AudioClip BGMSong2;

    [Header("Slider")]
    public Slider volSlider;
    public Text volSliderText;
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

        if (volSlider)
        {
            volSlider.onValueChanged.AddListener(onValueChange);
        }
    }

    void ShowMainMenu()
    {
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
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

        if (volSlider && volSliderText)
        {
            float value;
            AudioMixer.GetFloat("MasterVol", out value);
            volSlider.value = value + 80;
            volSliderText.text = (value + 80).ToString();
        }
    }
    void onValueChange(float value)
    {
        if (volSliderText)
        {
            volSliderText.text = value.ToString();
            AudioMixer.SetFloat("MasterVol", value - 80);
        }

    }


    // Update is called once per frame
    void Update()
    {
        timer.text = ScoreTimerManager.Instance.timeRemaining.ToString("n2");
        score.text = ScoreTimerManager.Instance.score.ToString();
    }
}
