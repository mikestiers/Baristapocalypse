using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private List<GameObject> containers;
    [SerializeField] private List<Image> starImages;
    [SerializeField] private Sprite starEmpty;
    [SerializeField] private Sprite starFull;

    [Header("Values")]
    [SerializeField] private TextMeshProUGUI customersServedValue;
    [SerializeField] private TextMeshProUGUI customersLeaveValue;
    [SerializeField] private TextMeshProUGUI tipsAcquiredValue;
    [SerializeField] private TextMeshProUGUI tipsNeededValue;
    [SerializeField] private TextMeshProUGUI tipsDifferenceValue;
    [SerializeField] private TextMeshProUGUI TipsDiffernceText;
    [SerializeField] private TextMeshProUGUI winLoseText;

    private void Start()
    {
        GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;

        Hide();
    }

    private void GameManager_OnGameStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsGameOver())
        {
            ((InputSystemUIInputModule)EventSystem.current.currentInputModule).point.action.Enable();
            ((InputSystemUIInputModule)EventSystem.current.currentInputModule).leftClick.action.Enable();
            Show();
            ComputeEndGameStats();
            // Add logic to show in the Game Over UI screen
            // Whatever we are going to show, Money, amount of recepies, etc
        }
        else
            Hide();
    }

    private void ComputeEndGameStats()
    {
        int currentRatings = Mathf.FloorToInt(GameManager.Instance.moneySystem.GetAverageReviewrating());
        bool iSWin = (GameManager.Instance.moneySystem.GetCurrentMoney() >= GameValueHolder.Instance.difficultySettings.GetMoneyToPass());

        customersServedValue.text = CustomerManager.Instance.GetCustomerServed().ToString();
        customersLeaveValue.text = CustomerManager.Instance.GetCustomerLeave().ToString();
        tipsAcquiredValue.text = (("$") + GameManager.Instance.moneySystem.GetCurrentMoney().ToString());
        tipsNeededValue.text = (("$") + GameValueHolder.Instance.difficultySettings.GetMoneyToPass().ToString());

        string difference = iSWin ? "+" : " ";

        tipsDifferenceValue.text = difference + (GameManager.Instance.moneySystem.GetCurrentMoney() - GameValueHolder.Instance.difficultySettings.GetMoneyToPass()).ToString();
        tipsDifferenceValue.color = iSWin ? Color.green : Color.red;

        TipsDiffernceText.text = iSWin ? "Extra Tips" : "Tips Short";

        winLoseText.text = iSWin ? "Coffee Shop Escape!" : "Vacation Brewsponed!";

        UpdateStarRating(currentRatings);
        StartCoroutine(ShowElements());
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator ShowElements()
    {
        for (int i = 0; i < containers.Count; i++)
        {
            StartCoroutine(ActivateMessage(containers[i]));

            yield return new WaitForSeconds(.5f);
        }

    }

    public IEnumerator ActivateMessage(GameObject Container)
    {
        yield return new WaitForSeconds(2f);

        Container.SetActive(true);
    }

    public void UpdateStarRating(int reviewScore)
    {
        // change stars color based on review (5 stars max)
        for (int i = 0; i < starImages.Count; i++)
        {
            if (i >= reviewScore)
            {
                starImages[i].sprite = starEmpty;
            }
            else
            {
                starImages[i].sprite = starFull;
            }
        }
    }
}
