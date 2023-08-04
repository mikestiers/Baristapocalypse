using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CustomerRaceSO : ScriptableObject
{
    
    public string objectName;
    public string objectTag;

    public int minSweetness;
    public int minBitterness;
    public int minStrength;
    public int minTemperature;
    public int minSpiciness;

    public int maxSweetness;
    public int maxBitterness;
    public int maxStrength;
    public int maxTemperature;
    public int maxSpiciness;

}
