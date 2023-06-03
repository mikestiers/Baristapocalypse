using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class IngredientSO : ScriptableObject
{
    public GameObject prefab;
    public string objectName;
    public string objectTag;
    public int sweetness;
    public int bitterness;
    public int strength;
    public int hotness;
    public int spiciness;
}
