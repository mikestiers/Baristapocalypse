using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using TMPro;

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

    [Header("Menu")]
    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject gameOverMenu;
    public GameObject pauseMenu;
    public GameObject tutorialMenu;

    [Header("Text")]
    public Text timer;
    public Text score;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI finalScore;

    [Header("Order Stats")]
    public Transform ordersPanel;
    public GameObject ordersUiPrefab;
    private OrderStats orderStats;
    
    [Header("Customer Review")]
    private CustomerReview customerReview;
    public GameObject starPrefab;
    //public Transform starRating;
    //public Sprite filledStarSprite;
    //public Sprite emptyStarSprite;

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

    }
    private void ReturnToGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TestScene");
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
        SceneManager.LoadScene("Main Menu Scene");
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
        settingsMenu.SetActive(true);
    }

    public void ShowCustomerUiOrder(CustomerBase customer)
    {
        orderStats = Instantiate(ordersUiPrefab, ordersPanel).GetComponent<OrderStats>();
        orderStats.Initialize(customer);
    }

    public void ShowCustomerReview(CustomerBase customer)
    {
        if (orderStats != null)
        {
            GameObject customerReviewUIObject = orderStats.GetCustomerReview(customer);
            if (customerReviewUIObject != null)
            {
                customerReview = customerReviewUIObject.GetComponentInParent<CustomerReview>();
                if (customerReview != null)
                {
                    Text customerReviewText = customerReviewUIObject.GetComponent<Text>();
                    customerReview.GenerateReview(customer);
                    customerReviewText.text = customerReview.ReviewText;
                    UpdateStarRating(customerReview.ReviewScore);
                    customerReviewUIObject.SetActive(true);
                }
                else
                {
                    Debug.Log($"CustomerReview component not found on parent for {customer}");
                }
            }
            else
            {
                Debug.LogError($"customerReviewUIObject is null for {customer}");
            }
        }
        else
        {
            Debug.Log($"Order Stats are null for {customer}");
            return;
        }
    }

    public void RemoveCustomerUiOrder(CustomerBase customer)
    {
        // TODO: Add ingredients
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
        timer.text = ScoreTimerManager.Instance.timeRemaining.ToString("n2");
        score.text = ScoreTimerManager.Instance.score.ToString();
    }
}
