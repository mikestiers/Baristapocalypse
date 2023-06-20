using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractStation : BaseStation
{
    [SerializeField] private ParticleSystem interactParticle;
    public override void Interact(PlayerStateMachine  player)
    {
        if (!HasIngredient())
        {
            if (player.HasIngredient())
            {
                player.GetIngredient().SetIngredientParent(this);
                SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.interactStation);
                interactParticle.Play();
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
