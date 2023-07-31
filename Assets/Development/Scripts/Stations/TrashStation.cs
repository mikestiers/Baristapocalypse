using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashStation : BaseStation
{
    [SerializeField] private ParticleSystem interactParticle;
    public override void Interact(PlayerStateMachine player)
    {
        if (player.GetNumberOfIngredients() >= 1)
        {
            foreach (Transform holdPoint in player.ingredientHoldPoints)
            {
                Ingredient ingredient = holdPoint.GetComponentInChildren<Ingredient>();
                ingredient.DestroyIngredient();
                ingredient.SetIngredientParent(player);
                player.GetIngredient().DestroyIngredient();
                SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.interactStation);
                //interactParticle.Play();
            }
        }
    }
}
