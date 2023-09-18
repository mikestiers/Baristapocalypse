using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoffeeAttributes : MonoBehaviour
{
    [SerializeField] private int sweetness;
    [SerializeField] private int bitterness;
    [SerializeField] private int strength;
    [SerializeField] private int temperature;
    [SerializeField] private int spiciness;
    private bool isMinigamePerfect;

    private void Start()
    {
        isMinigamePerfect = false;
    }

    public int GetSweetness()
    {
        return sweetness;
    }
    public int GetBitterness()
    {
        return bitterness;
    }
    public int GetStrength()
    {
        return strength;
    }
    public int GetTemperature()
    {
        return temperature;
    }
    public int GetSpiciness()
    {
        return spiciness;
    }

    public void AddSweetness(int sweetness)
    {
        this.sweetness += sweetness;
    }
    public void AddBitterness(int bitterness)
    {
        this.bitterness += bitterness;
    }
    public void AddStrength(int strength)
    {
        this.strength += strength;
    }
    public void AddTemperature(int temperature)
    {
        this.temperature += temperature;
    }
    public void AddSpiciness(int spiciness)
    {
        this.spiciness += spiciness;
    }

    public void SetIsMinigamePerfect(bool minigame)
    {
        isMinigamePerfect = minigame;
    }
    
}
