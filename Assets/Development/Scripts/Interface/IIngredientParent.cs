using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IIngredientParent
{
    public Transform GetIngredientTransform();
    public void SetIngredient(Ingredient ingredient);
    public Ingredient GetIngredient();
    public void ClearIngredient();
    public bool HasIngredient();
}
