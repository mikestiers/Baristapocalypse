using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashStation : BaseStation
{
    [SerializeField] private ParticleSystem interactParticle;
    public override void Interact(PlayerStateMachine player)
    {
        if (player.HasIngredient())
        {
            player.GetIngredient().DestroyIngredient();
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.interactStation);
            interactParticle.Play();
        }
    }
}
