using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class MoneySystem 
{
    private NetworkVariable<int> currentMoney = new NetworkVariable<int>();
    private int moneyNeededToPass;
    public NetworkVariable<int> currentStreakCount = new NetworkVariable<int>();
    private int maxStreakCount; //before activating double tips
    private float baseTipMultiplier;
    private float streakBonus;

    //Dont know where to put this
    private float AverageReviewRatings;
 
    public MoneySystem(int moneyNeededToPass)
    {
        this.moneyNeededToPass = moneyNeededToPass;
        this.currentMoney.Value = 0;
        this.currentStreakCount.Value = 0;
        this.streakBonus = 0.5f;

        //can be exposed in difficultysettings, ask team
        this.maxStreakCount = 5;
        this.baseTipMultiplier = .10f; 

        //check not zero for computation
        if(this.moneyNeededToPass == 0)
        {
            this.moneyNeededToPass = 100;
            Debug.LogWarning(("Money needed to pass is not set in Scriptable object setting value to ") + this.moneyNeededToPass.ToString());
        }
    }

    public void AdjustMoneyByAmount(int MoneyAmount, bool isAdding)
    {
        if(isAdding == true) currentMoney.Value += MoneyAmount;
        else currentMoney.Value -= MoneyAmount;

        if(currentMoney.Value < 0) currentMoney.Value = 0;

        float percentToPass = (float)currentMoney.Value / (float)moneyNeededToPass;

        //UI for Added Money //Set UI MoneyText to currentMoney //SetPercentBar
        UIManager.Instance.UpdateScoreUI(currentMoney.Value, MoneyAmount, isAdding, percentToPass);
    }

    public int ComputeMoney(int reviewscore)
    {
        int baseAmount = 10; //change with amount of ingredients? check with team
        int money = Mathf.CeilToInt(baseAmount * (reviewscore * baseTipMultiplier) + (streakBonus * currentStreakCount.Value));

        return money;
    }

    public bool CheckStreak()
    {
        if (currentStreakCount.Value >= maxStreakCount)
        {
            currentStreakCount.Value = maxStreakCount; 
            return true;
        }
        else return false;
    }

    public int GetCurrentMoney()
    {
        return currentMoney.Value;
    }

    public void IncreaseStreakCount()
    {
        currentStreakCount.Value++;
    }

    public void DecreaseStreakCount()
    {
        currentStreakCount.Value--;
        if (currentStreakCount.Value <= 0) currentStreakCount.Value = 0;
    }

    public void ResetStreak()
    {
        currentStreakCount.Value = 0;
    }

    public void SetAverageReviewRating(float Rating) 
    {
       AverageReviewRatings = Rating;
    }

    public float GetAverageReviewrating()
    {
        return AverageReviewRatings;
    }

    //process review to money
    //streak


    //Compute Money
    //tips = baseamount * (review *.20) + (streakbonus * currentStreak) cuz its extra for cost of drink
    //streak counter when full will double the tips 
  
}
