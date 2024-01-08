using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class BrewingRecipeSO : ScriptableObject
{
    public IngredientSO inputCoffeeGrind;
    public IngredientSO inputMilk;
    public IngredientSO inputSweetener;
    public IngredientSO inputBioMatter;
    public IngredientSO output;
    public float brewingMax;
}
