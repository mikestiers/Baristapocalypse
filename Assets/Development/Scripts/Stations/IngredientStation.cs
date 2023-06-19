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
                Ingredient.SpawnIngredient(ingredientSO, player);
                SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.interactStation);
                interactParticle.Play();

            }
        }
    }
}
