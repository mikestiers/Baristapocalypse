using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IngredientStation : BaseStation
{
    [SerializeField] private IngredientSO ingredientSO;

    public override void Interact(PlayerStateMachine player)
    {
        if (!HasIngredient())
        {
            if (!player.HasIngredient())
            {
                Ingredient.SpawnIngredient(ingredientSO, player);
                                


            }
        }
    }
}
