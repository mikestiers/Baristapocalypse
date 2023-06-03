using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoffeeAttributes : MonoBehaviour
{
    [SerializeField] private int sweetness;
    [SerializeField] private int bitterness;
    [SerializeField] private int strength;
    [SerializeField] private int hotness;
    [SerializeField] private int spiciness;

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
    public void AddHotness(int hotness)
    {
        this.hotness += hotness;
    }
    public void AddSpiciness(int spiciness)
    {
        this.spiciness += spiciness;
    }
    
}
