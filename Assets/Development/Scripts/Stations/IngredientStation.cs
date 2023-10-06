using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class IngredientStation : BaseStation
{
    [SerializeField] private IngredientSO ingredientSO;
    [SerializeField] private ParticleSystem interactParticle;

    public override void Interact(PlayerStateMachine player)
    {
        if (player.IsHoldingPickup)
            return;

        if (!HasIngredient())
        {
            if (!player.HasIngredient())
            {
                // Check if the player has already spawned 4 ingredients
                if (player.GetNumberOfIngredients() >= 4)
                {
                    Debug.Log("Max ingredients spawned!");
                    return;
                }

                Ingredient.SpawnIngredient(ingredientSO, player);
                player.GetNumberOfIngredients();
                SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.interactStation);
                interactParticle.Play();

            }
        }
    }
}
