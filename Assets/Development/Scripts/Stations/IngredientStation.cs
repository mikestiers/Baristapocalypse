using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientStation : BaseStation
{
    [SerializeField] private IngredientSO ingredientSO;

    public override void Interact(PlayerStateMachine player)
    {
        if (!HasIngredient())
        {
            Ingredient.SpawnIngredient(ingredientSO, player);
        }
    }
}
