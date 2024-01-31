using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class MoneySystem 
{
    private int currentMoney;
    private int moneyNeededToPass;
    private int currentStreakCount;
    private int maxStreakCount; //before activating double tips
    private float baseTipMultiplier;

    //Dont know where to put this
    private float AverageReviewRatings;
 
    public MoneySystem(int moneyNeededToPass)
    {
        this.moneyNeededToPass = moneyNeededToPass;
        this.currentMoney = 0;
        this.currentStreakCount = 0;

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
        if(isAdding == true) currentMoney += MoneyAmount;
        else currentMoney -= MoneyAmount;

        if(currentMoney < 0) currentMoney = 0;

        float percentToPass = (float)currentMoney / (float)moneyNeededToPass;

        //UI for Added Money //Set UI MoneyText to currentMoney //SetPercentBar
        UIManager.Instance.UpdateScoreUI(currentMoney, MoneyAmount, isAdding, percentToPass);
    }

    public int ComputeMoney(int reviewscore)
    {
        int baseAmount = 10; //change with amount of ingredients? check with team
        int money = Mathf.CeilToInt(baseAmount * (reviewscore * baseTipMultiplier));

        if (CheckStreak() == true) money *= 2;

        return money;
    }

    public bool CheckStreak()
    {
        if (currentStreakCount >= maxStreakCount)
        {
            currentStreakCount = maxStreakCount; 
            return true;
        }
        else return false;
    }

    public int GetCurrentMoney()
    {
        return currentMoney;
    }

    public void IncreaseStreakCount()
    {
        currentStreakCount++;
    }

    public void DecreaseStreakCount()
    {
        currentStreakCount--;
        if (currentStreakCount <= 0) currentStreakCount = 0;
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
    //tips = baseamount * (review *.20) cuz its extra for cost of drink
    //streak counter when full will double the tips 
  
}
