using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class IngredientStation : BaseStation
{
    [SerializeField] private IngredientSO ingredientSO;
    [SerializeField] private ParticleSystem interactParticle;

    public override void Interact(PlayerController player)
    {
        if (player.HasPickup())
            return;

        if (!HasIngredient())
        {
            if (!player.HasIngredient())
            {
                // Check if the player has already spawned maxingredients
                if (player.GetNumberOfIngredients() >= player.GetMaxIngredients())
                {
                    Debug.Log("Max ingredients spawned!");
                    return;
                }

                Ingredient.SpawnIngredient(ingredientSO, player);
                SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.interactStation);
                interactParticle.Play();

            }
        }
    }
}
