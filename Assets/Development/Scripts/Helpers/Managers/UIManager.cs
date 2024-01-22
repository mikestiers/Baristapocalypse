using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;

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

    [Header("Menu")]
    public GameObject mainMenu;
    public GameObject audioSettings;
    public GameObject gameOverMenu;
    public GameObject pauseMenu;
    public GameObject tutorialMenu;
    public GameObject ordersMenu;

    [Header("Text")]
    public Text timer;
    public Text score;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI finalScore;
    public Text volSliderText;

    [Header("Order Stats")]
    public Transform ordersPanel;
    public GameObject ordersUiPrefab;
    private OrderStats orderStats;

    [Header("Customer Review")]
    public GameObject customerReviewPrefab;
    public GameObject starPrefab;
    public GameObject customerReviewPanel;
    public float rPSpeed;
    public float rPArrivalThreshold;
    public float popOutRPTime;
    public float travelDistanceRP;
    private GameObject customerReviewTab;
    private Vector3 originalRPPosition;
    private Vector3 popOutRPPosition;
    private List<GameObject> customerReviewTabs = new List<GameObject>();

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


    [Header("Slider")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider mainVolumeSlider;
    public Slider voiceVolumeSlider;

    public AudioMixer mixer;

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

        originalRPPosition = customerReviewPanel.transform.position;
        popOutRPPosition = new Vector3(customerReviewPanel.transform.position.x - travelDistanceRP, customerReviewPanel.transform.position.y, customerReviewPanel.transform.position.z);
        //if (volSlider)
        //{
        //    volSlider.onValueChanged.AddListener((value) => OnSliderValueChanged(value));
        //    if (volSliderText)
        //        volSliderText.text = volSlider.value.ToString();
        //}
    }
    private void ReturnToGame()
    {
        timer.enabled = true;
        score.enabled = true;
        ordersMenu.SetActive(true);
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    private void RestartGame()
    {
        //Reset score timer
        //ScoreTimerManager.Instance.ResetTimerScore();

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
        SceneManager.LoadScene(mainMenuScene);
        //ScoreTimerManager.Instance.ResetTimerScore();
        timer.enabled = false;
        score.enabled = false;
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
    }

    public void ShowCustomerUiOrder(CustomerBase customer)
    {
        orderStats = Instantiate(ordersUiPrefab, ordersPanel).GetComponent<OrderStats>();
        customerReviewTab = Instantiate(customerReviewPrefab, customerReviewPanel.transform);
        customerReviewTabs.Add(customerReviewTab);
        //ShowCustomerUiOrderClientRpc();
        orderStats.Initialize(customer);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ShowCustomerUiOrderServerRpc()
    {
        ShowCustomerUiOrderClientRpc();
    }

    [ClientRpc]
    public void ShowCustomerUiOrderClientRpc()
    {
        customerReviewTab = Instantiate(customerReviewPrefab, customerReviewPanel.transform);
        customerReviewTabs.Add(customerReviewTab);
    }

    public void ShowCustomerReview(CustomerBase customer)
    {
        // This can be better by moving customer review script to the customer object
        foreach (Transform t in ordersPanel)
        {
            OrderStats o = t.GetComponent<OrderStats>();
            if (o != null)
            {
                if (o.GetOrderOwner() == customer)
                {
                    Debug.Log("Customer Number:"+ customer.customerNumber);
                    CustomerReview customerReview = t.gameObject.GetComponent<CustomerReview>();
                    Text customerReviewText = customerReviewTabs[0].GetComponentInChildren<Text>();
                    customerReview.GenerateReview(customer);
                    customerReviewText.text = customerReview.ReviewText;
                    UpdateStarRating(customerReview.ReviewScore);
                    customerReviewTabs[0].gameObject.SetActive(true);
                    break;
                }
                else
                {
                    Debug.Log($"Customer Order UI not found for {customer.customerNumber}");
                }
            }
        }
        StartCoroutine(MoveRP(popOutRPPosition, originalRPPosition));
    }

    private IEnumerator MoveRP(Vector3 target, Vector3 start)
    {
        float startTime = Time.time;

        // Move towards the target
        while (Time.time - startTime < popOutRPTime)
        {
            float t = (Time.time - startTime) / popOutRPTime*rPSpeed;
            customerReviewPanel.transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        // Wait at the target for specified time
        yield return new WaitForSeconds(popOutRPTime);

        startTime = Time.time;

        // Move back to the initial position
        while (Time.time - startTime < popOutRPTime)
        {
            float t = (Time.time - startTime) / popOutRPTime*rPSpeed;
            customerReviewPanel.transform.position = Vector3.Lerp(target, start, t);
            yield return null;
        }
        Destroy(customerReviewTabs[0]);
        customerReviewTabs.RemoveAt(0);
        Debug.Log("Movement completed!");
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
        Transform starRating = customerReviewTabs[0].GetComponentInChildren<GridLayoutGroup>().gameObject.transform;

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
    public void SetMusicVolume(float value)
    {
        //if (volSliderText)
        //    volSliderText.text = value.ToString();

        mixer.SetFloat("Music", musicSlider.value);

    }

    public void SetSFXVolume(float value)
    {
        mixer.SetFloat("SFX", sfxSlider.value);
    }
    
    public void SetMainVolume(float value)
    {
        mixer.SetFloat("MainVolume", mainVolumeSlider.value);
    }
    
    public void SetVoiceVolume(float value)
    {
        mixer.SetFloat("VoiceLines", voiceVolumeSlider.value);
    }


}
