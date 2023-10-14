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
            for (int i = 0; i < player.ingredientHoldPoints.Length; i++)
            {
                Transform holdPoint = player.ingredientHoldPoints[i];
                if (holdPoint.childCount > 0)
                {
                    Ingredient ingredient = holdPoint.GetComponentInChildren<Ingredient>();
                    ingredient.DestroyIngredient();
                    SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.interactStation);
                    //interactParticle.Play();
                }
            }
        }
    }
}

