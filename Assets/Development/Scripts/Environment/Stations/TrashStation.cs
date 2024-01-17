using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashStation : BaseStation
{
    [SerializeField] private ParticleSystem interactParticle;
    public override void Interact(PlayerController player)
    {
        if (player.GetNumberOfIngredients() >= 1)
        {
            foreach(Ingredient i in player.GetIngredientsList())
            {
                Ingredient.DestroyIngredient(i);
                SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.interactStation);
            }
        }
    }
}

