using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoffeeAttributes : MonoBehaviour
{
    [SerializeField] private int sweetness;
    [SerializeField] private int strength;
    [SerializeField] private int temperature;
    [SerializeField] private int spiciness;
    private bool isMinigamePerfect;

    private void Start()
    {
        isMinigamePerfect = false;
    }

    public int GetTemperature()
    {
        return temperature;
    }
    public int GetSweetness()
    {
        return sweetness;
    }
    public int GetSpiciness()
    {
        return spiciness;
    }
    public int GetStrength()
    {
        return strength;
    }

    public void AddTemperature(int temperature)
    {
        this.temperature += temperature;
    }
    public void AddSweetness(int sweetness)
    {
        this.sweetness += sweetness;
    }
    public void AddSpiciness(int spiciness)
    {
        this.spiciness += spiciness;
    }
    public void AddStrength(int strength)
    {
        this.strength += strength;
    }

    public void SetIsMinigamePerfect(bool minigame)
    {
        isMinigamePerfect = minigame;
    }

    public bool GetIsMinigamePerfect()
    {
        return isMinigamePerfect;
    }
    
}
