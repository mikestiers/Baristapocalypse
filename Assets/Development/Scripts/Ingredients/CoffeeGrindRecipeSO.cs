using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CoffeeGrindRecipeSO : ScriptableObject
{
    public IngredientSO input;
    public IngredientSO output;
    public int grindMax;
}
