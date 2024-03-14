using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.UI;

public class TrashStation : BaseStation
{
    [SerializeField] private ParticleSystem interactParticle;
    [SerializeField] private GameObject interactImage;
    [SerializeField] private PlayableDirector playableDirector;
    [SerializeField] private PlayableAsset trashOpenAnimation;
    [SerializeField] private PlayableAsset trashCloseAnimation;
    private Ingredient trashIngredient;
    private Pickup pickup;
    private PlayerController player;
    private string mop = "Mop";

    private void OnEnable()
    {
        InputManager.OnInputChanged += InputUpdated;
    }

    private void OnDisable()
    {
        InputManager.OnInputChanged -= InputUpdated;
    }

    private void InputUpdated(InputImagesSO inputImagesSO)
    {
        interactImage.GetComponentInChildren<Image>().sprite = inputImagesSO.interact;
    }

    public override void Interact(PlayerController player)
    {
        if (player.GetNumberOfIngredients() >= 1)
        {
            foreach(Ingredient i in player.GetIngredientsList())
            {
                player.RemoveIngredientInListByReference(i);
                trashIngredient = i;
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
        Ingredient.DestroyIngredient(trashIngredient);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            player = other.GetComponent<PlayerController>();

            if (player.IsLocalPlayer)
            {
                interactImage.SetActive(true);
                playableDirector.Play(trashOpenAnimation);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            player = other.GetComponent<PlayerController>();

            if (player.IsLocalPlayer)
            {
                interactImage.SetActive(false);
                playableDirector.Play(trashCloseAnimation);
            }
        }
    }

}

