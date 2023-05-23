using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashStation : BaseStation
{
    public override void Interact(PlayerController player)
    {
        if (player.HasIngredient())
        {
            player.GetIngredient().DestroyIngredient();
        }
    }
}
