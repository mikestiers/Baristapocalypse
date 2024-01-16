using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TrashStation : BaseStation
{
    [SerializeField] private ParticleSystem interactParticle;
    private Ingredient ingredient;

    public override void Interact(PlayerController player)
    {
        // Temporary thile hold points are fixed
        if (player.HasIngredient())
        {
            Debug.Log("trashinggggg");
            ingredient = player.GetIngredient();
            InteractServerRpc();
        }

        //if (player.GetNumberOfIngredients() >= 1)
        //{
        //    for (int i = 0; i < player.ingredientHoldPoints.Length; i++)
        //    {
        //        Transform holdPoint = player.ingredientHoldPoints[i];
        //        if (holdPoint.childCount > 0)
        //        {
        //            ingredient = holdPoint.GetComponentInChildren<Ingredient>();
        //            InteractServerRpc();
        //            //Ingredient.DestroyIngredient(ingredient);
        //            SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.interactStation);
        //            //interactParticle.Play();
        //        }
        //    }
        //}
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractServerRpc()
    {
        InteractClientRpc();
    }

    [ClientRpc]
    private void InteractClientRpc()
    {
        Ingredient.DestroyIngredient(ingredient);
    }

}

