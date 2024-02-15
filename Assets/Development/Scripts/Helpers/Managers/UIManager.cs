using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Audio;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Collections;
using System;

public class UIManager : Singleton<UIManager>
{
    [Header("Button")]
    public Button toGame;
    public Button toMainFromPause;
    public Button toMainFromGameOver;
    public Button toSettings;
    public Button quit;
    public Button toTutorial;
    public Button toPause;
    public Button closePause;
    public Button restartGame;
    public Button closeAudioSettings;
    public Button closeTutorial;
    public Button tutorialModeOnOff;

    [Header("Menu")]
    public GameObject mainMenu;
    public GameObject audioSettings;
    public GameObject gameOverMenu;
    public GameObject pauseMenu;
    public GameObject tutorialMenu;
    public GameObject ordersMenu;
    public GameObject playerReadyMenu;

    [Header("Text")]
    public Text timer;
    public Text score;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI finalScore;
    public Text volSliderText;
    public TextMeshProUGUI gameTimerText;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI currencyText;
    public TextMeshProUGUI streakText;
    public TextMeshProUGUI gameMessage;

    [Header("Tutorial Image")]
    public Image tutorialImage;

    [Header("GameMessageHolder")]
    public GameObject gameMessageContainer;

    [Header("TimerHolders")]
    public GameObject smallTimer;
    public GameObject bigTimer;

    [Header("MoneyUI")]
    public GameObject moneyUI;

    [Header("ShiftEvaluationUI")]
    public GameObject shiftEvaluationUI;

    [Header("Order Stats")]
    public Transform ordersPanel;
    public GameObject ordersUiPrefab;
    private OrderStats orderStats;
    
    [Header("Customer Review")]
    private CustomerReview customerReview;
    public GameObject starPrefab;
    private List<GameObject> customerReviews;

    [Header("DebugConsole")]
    public GameObject debugConsole;
    public bool debugConsoleActive = false;

    // Scenes
    private string activeGameScene = "T5M3_BUILD";
    private string mainMenuScene = "MainMenuScene"; // using lobby scene in the mean time, need changing to Main menu

    [Header("DifficultyTesting")]
    public Text customersLeft;
    public Text customersInStore;
    public Text spawnMode;
    public Text wavesleft;
    public Text shift;

    [SerializeField] private LevelLoader levelLoader;

    private void Start()
    {
        if (toGame)
            toGame.onClick.AddListener(ReturnToGame);
        if (toSettings)
            toSettings.onClick.AddListener(ShowSettingsMenu);
        if (quit)
            quit.onClick.AddListener(QuitGame);
        if (toMainFromPause)
            toMainFromPause.onClick.AddListener(ShowMainMenu);
        if (toMainFromGameOver)
            toMainFromGameOver.onClick.AddListener(ShowMainMenu);
        if (toTutorial)
            toTutorial.onClick.AddListener(ShowTutorial);
        if (toPause)
            toPause.onClick.AddListener(ShowPause);
        if (closePause)
            closePause.onClick.AddListener(ClosePause);
        if (restartGame)
            restartGame.onClick.AddListener(RestartGame);
        if (closeAudioSettings)
            closeAudioSettings.onClick.AddListener(CloseAudioSettings);
        if (BaristapocalypseMultiplayer.playMultiplayer)
            tutorialModeOnOff.gameObject.SetActive(false);
        else if (tutorialModeOnOff)
            tutorialModeOnOff.onClick.AddListener(ToggleTutorialMode);
        if (closeTutorial)
            closeTutorial.onClick.AddListener(CloseTutorial);

        closeTutorial.GetComponentInChildren<Text>().text = GameManager.Instance.IsGamePlaying() ? "Close" : "Ready";
        tutorialModeOnOff.GetComponentInChildren<Text>().text = TutorialManager.Instance.tutorialEnabled ? "Tutorial Mode: On" : "Tutorial Mode: Off";
    }
    private void OnEnable()
    {
        PlayerController.OnInputChanged += InputUpdated;
    }

    private void OnDisable()
    {
        PlayerController.OnInputChanged -= InputUpdated;
    }

    private void InputUpdated(InputImagesSO inputImagesSO)
    {
        tutorialImage.sprite = inputImagesSO.tutorialImage;
    }

    private void ReturnToGame()
    {
        timer.enabled = true;
        ordersMenu.SetActive(true);
        tutorialMenu.SetActive(false);
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;
        gameOverMenu.SetActive(false);
        //GameManager.Instance.gameState = GameState.RUNNING;
        SceneManager.LoadScene(activeGameScene);
    }

    private void ClosePause()
    {
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.menuClicks);
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }
    private void ShowPause()
    {
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.menuClicks);
        tutorialMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }

    private void ShowTutorial()
    {
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.menuClicks);
        pauseMenu.SetActive(false);
        tutorialMenu.SetActive(true);
    }

    private void ShowMainMenu()
    {
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.menuClicks);
        // Moved Reset game function above scene change due to it not being called if scene swapped beforehand
        //SceneHelper.Instance.ResetGame();        
        SceneManager.LoadScene(mainMenuScene); 
        timer.enabled = false;
        //score.enabled = false;
        ordersMenu.SetActive(false);
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    private void QuitGame()
    {
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.menuClicks);
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void ShowSettingsMenu()
    {
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.menuClicks);
        mainMenu.SetActive(false);
        audioSettings.SetActive(true);
    }

    private void CloseAudioSettings()
    {
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.menuClicks);
        audioSettings.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void RemoveCustomerUiOrder(CustomerBase customer)
    {
        foreach (Transform t in ordersPanel)
        {
            OrderStats o = t.GetComponent<OrderStats>();
            if (o != null)
            {
                if (o.GetOrderOwner() == customer)
                {
                    Destroy(t.gameObject);
                    return;
                }
                else if (o.GetOrderOwner().customerNumber == customer.customerNumber)
                {
                    // This exists because pickups are cloned, so the connection to the order prefab is lost
                    // Just search for the customer based on their number in case owner is lost
                    Destroy(t.gameObject);
                    return;
                }
                else
                {
                    Debug.Log($"Customer Order UI not found for {customer.customerNumber}");
                }
            }
        }
    }

    public void UpdateStarRating(int reviewScore)
    {
        Transform starRating = customerReview.transform.Find("CustomerReview/StarRating");

        // Instantiate stars. 5 stars max
        for (int i = 1; i <= 5; i++)
        {
            if (i <= reviewScore)
            {
                GameObject star = Instantiate(starPrefab, starRating);
                Image starImage = star.GetComponent<Image>();
            }
            else
            {
                GameObject star = Instantiate(starPrefab, starRating);
                Image starImage = star.GetComponent<Image>();
                starImage.color = Color.red;
            }
        }
    }

    private void Update()
    {
       //timer.text = ScoreTimerManager.Instance.timeRemaining.ToString("n2");
       //score.text = ScoreTimerManager.Instance.score.ToString();

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            tutorialMenu.SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            tutorialMenu.SetActive(false);
        }
    }

    public void ToggleBigTimer(bool IsOn)
    {
        if(IsOn) bigTimer.SetActive(true);
        else bigTimer.SetActive(false);
    }

    public void ToggleSmalltimer(bool IsOn)
    {
        if(IsOn) smallTimer.SetActive(true);
        else smallTimer.SetActive(false);
    }

    public void SayGameMessage(string GameMessage)
    {
        gameMessage.text = GameMessage;

        gameMessageContainer.SetActive(true);

        StartCoroutine(DeactivateGameMessage());
    }

    public IEnumerator DeactivateGameMessage() 
    {
        yield return new WaitForSeconds(4f);

        gameMessageContainer.SetActive(false);
    }

    public void UpdateScoreUI(int currentMoney, int adjustedMoney, bool isAdding, float passPercentage)
    {
        moneyUI.GetComponent<ScoreUI>().UpdateMoneyVisuals(currentMoney, adjustedMoney, isAdding, passPercentage);
    }
    
    public void ShowShiftEvaluation()
    {
        shiftEvaluationUI.GetComponent<ShiftEvaluationUI>().Evaluate();
    }

    private void CloseTutorial()
    {
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.menuClicks);
        if (!GameManager.Instance.IsLocalPlayerReady())
        {
            GameManager.Instance.SetLocalPlayerReady();
            closeTutorial.GetComponentInChildren<Text>().text = "Close";
            ReturnToGame();
            return;
        }
        ShowPause();
    }

    private void ToggleTutorialMode()
    {
        tutorialModeOnOff.GetComponentInChildren<Text>().text = TutorialManager.Instance.tutorialEnabled ? "Tutorial Mode: Off" : "Tutorial Mode: On";
        TutorialManager.Instance.tutorialEnabled = !TutorialManager.Instance.tutorialEnabled;
    }

}
