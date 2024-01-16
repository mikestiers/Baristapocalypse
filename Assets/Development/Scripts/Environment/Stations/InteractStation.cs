using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractStation : BaseStation
{
    [SerializeField] private ParticleSystem interactParticle;
    public override void Interact(PlayerController player)
    {
        if (!HasIngredient())
        {
            if (player.GetNumberOfIngredients() >= player.GetMaxIngredients())
            {
                return; 
            }
            foreach (Transform holdPoint in player.ingredientHoldPoints)
            {
                Ingredient ingredient = holdPoint.GetComponentInChildren<Ingredient>();
                if (ingredient != null)
                {
                    player.GetIngredient().SetIngredientParent(this);
                    SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.interactStation);
                    interactParticle.Play();
                    break;
                }
            }
        } 
        else
        {
            if (!player.HasIngredient())
            {
                GetIngredient().SetIngredientParent(player);
                SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.interactStation);
                interactParticle.Play();
            }
        }
    }
}
