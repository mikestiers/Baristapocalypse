using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashStation : BaseStation
{
    public override void Interact(PlayerStateMachine player)
    {
        if (player.HasIngredient())
        {
            player.GetIngredient().DestroyIngredient();
        }
    }
}
