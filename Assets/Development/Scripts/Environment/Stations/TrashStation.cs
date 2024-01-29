using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TrashStation : BaseStation
{
    [SerializeField] private ParticleSystem interactParticle;
    private Ingredient ingredient;
    private Pickup pickup;

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
                SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.interactStation);
            }
        }

        if (player.HasPickup())
        {
            Debug.Log("Destroying garbage cup");
            pickup = player.GetPickup();
            pickup.GetComponent<IngredientFollowTransform>().SetTargetTransform(pickup.transform);
            pickup.ClearPickupOnParent();

            TrashPickupServerRpc();
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

