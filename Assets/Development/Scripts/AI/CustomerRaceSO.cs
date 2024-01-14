using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CustomerRaceSO : ScriptableObject
{
    
    public string objectName;
    public string objectTag;

    public int minTemperature;
    public int minSweetness;
    public int minSpiciness;
    public int minStrength;

    public int maxTemperature;
    public int maxSweetness;
    public int maxSpiciness;
    public int maxStrength;
}
