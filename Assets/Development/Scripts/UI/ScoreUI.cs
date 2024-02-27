using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    [Header("Text")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI streakText;
    public TextMeshProUGUI moneyAdded;
    
    [Header("MoneyBar")]
    public Image moneyBar;

    [Header("Container")]
    public GameObject moneyAddedContainer;

    public void UpdateMoneyVisuals(int currentMoney, int addedMoney, bool isAdding, float passPercentage) 
    {
        scoreText.text = ("$ ") + currentMoney.ToString();

        if (isAdding == true) SayAddedMoneyMessage(("+") + addedMoney.ToString());
        else SayAddedMoneyMessage(("-") + addedMoney.ToString());

        SetMoneyBarPercentage(passPercentage);
    }

    public void UpdateStreak()
    {
        streakText.gameObject.SetActive(true);
        streakText.text = ("Perfect Drink x") + GameManager.Instance.moneySystem.currentStreakCount.Value;
    }

    public void DeactivateStreak()
    {
        streakText.gameObject.SetActive(false);
    }

    public void SetMoneyBarPercentage(float percentFill)
    {
        moneyBar.fillAmount = percentFill;
    }

    public void SayAddedMoneyMessage(string GameMessage)
    {
        moneyAdded.text = GameMessage;

        moneyAddedContainer.SetActive(true);

        StartCoroutine(DeactivateAddedMoneyMessage());
    }

    public IEnumerator DeactivateAddedMoneyMessage()
    {
        yield return new WaitForSeconds(4f);

        moneyAddedContainer.SetActive(false);
    }
}
