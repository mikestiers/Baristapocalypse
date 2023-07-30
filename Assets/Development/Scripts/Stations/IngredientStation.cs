using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IngredientStation : BaseStation
{
    [SerializeField] private IngredientSO ingredientSO;
    [SerializeField] private ParticleSystem interactParticle;

    public override void Interact(PlayerStateMachine player)
    {
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
                player.IncrementNumberOfIngredients();
                SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.interactStation);
                interactParticle.Play();

            }
        }
    }
}
