using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractStation : BaseStation
{
    public override void Interact(PlayerStateMachine player)
    {
        if (!HasIngredient())
        {
            if (player.HasIngredient())
            {
                player.GetIngredient().SetIngredientParent(this);
            }
        }
        else
        {
            if (!player.HasIngredient())
            {
                GetIngredient().SetIngredientParent(player);
            }
        }
    }
}
