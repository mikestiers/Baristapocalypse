using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TrashStation : BaseStation
{
    [SerializeField] private ParticleSystem interactParticle;
    private Ingredient ingredient;
    private Pickup pickup;
    private string mop = "Mop";

    public override void Interact(PlayerController player)
    {
        if (player.GetNumberOfIngredients() >= 1)
        {
            foreach(Ingredient i in player.GetIngredientsList())
            {
                Debug.Log("trashinggggg");
                player.RemoveIngredientInListByReference(i);
                ingredient = i;
                InteractServerRpc();
                player.OnAnimationSwitch();// setup animation here, OnAnimationSwitch() just reset the animation
                SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.interactStation);
            }
        }

        if (player.HasPickup())
        {
            pickup = player.GetPickup();

            bool hasMop = pickup.pickupSo.objectName == mop;
            if (hasMop)
            {
                return; 
            }

            Debug.Log("Destroying garbage cup");
            pickup.GetComponent<IngredientFollowTransform>().SetTargetTransform(pickup.transform);
            pickup.ClearPickupOnParent();

            TrashPickupServerRpc();
            player.OnAnimationSwitch();// setup animation here, OnAnimationSwitch() just reset the animation
        }
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

    [ServerRpc(RequireOwnership = false)]
    private void TrashPickupServerRpc()
    {
        TrashPickupClientRpc();
    }

    [ClientRpc]
    private void TrashPickupClientRpc()
    {
        Pickup.DestroyPickup(pickup);
    }

}

