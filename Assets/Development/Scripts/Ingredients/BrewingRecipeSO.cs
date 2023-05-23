using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class BrewingRecipeSO : ScriptableObject
{
    public IngredientSO input;
    public IngredientSO output;
    public float brewingMax;
}
