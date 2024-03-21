using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Spill : NetworkBehaviour
{
    private int cleaningProgress = 0; // start clenaing progress for spill
    [SerializeField] private int totalProgress = 4; // amount of timer required to clean spill (temporary)
    [SerializeField] private float slipSpeed = 0.8f;
    private ISpill messObjectParent;
    private SpillSpawnPoint _spillSpawnPoint;
    private IngredientFollowTransform _followTransform;
    [SerializeField] private GameObject controllerPrompt;
    [SerializeField] private GameObject keyboardPrompt;
    [SerializeField] private GameObject imageHolder;
    [SerializeField] private Image imageFill;
    [SerializeField] private GameObject mopErrorHolder;
    private readonly int BP_Barista_SlippingHash = Animator.StringToHash("BP_Barista_Slipping");
    private const float CrossFadeDuration = 0.1f;
    private AudioSource slipSource;

    private void Awake()
    {
        _spillSpawnPoint = GetComponent<SpillSpawnPoint>();
        
    }

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
        controllerPrompt.GetComponentInChildren<Image>().sprite = inputImagesSO.interact;
    }

    public void Interact(PlayerController pickupItem)
    {
        InteractServerRpc(pickupItem.GetNetworkObject());
    }

    [ServerRpc (RequireOwnership = false)]
    public void InteractServerRpc(NetworkObjectReference pickupNetworkObjectReference)
    {
        InteractClientRpc(pickupNetworkObjectReference);
    }

    [ClientRpc]
    private void InteractClientRpc(NetworkObjectReference pickupNetworkObjectReference)
    {

        pickupNetworkObjectReference.TryGet(out NetworkObject playerPickupNetworkObject);
        
        PlayerController player = playerPickupNetworkObject.GetComponent<PlayerController>();
        
        if (player.HasPickup())
        {
            if (player.Pickup.attributes.Contains(Pickup.PickupAttribute.CleansUpSpills)) 
            {
                if (cleaningProgress < totalProgress)
                {
                    // scale down the spill game object or play animation 
                    cleaningProgress++;
                    imageFill.fillAmount += 25f / 100f;
                }
                if (cleaningProgress >= totalProgress)
                {
                    player.OnAnimationSwitch();
                    GameManager.Instance.RemoveSpill();
                    Destroy(gameObject);
                } 
            }
        }
    }

    public static void CreateSpill(MessSO Mess, ISpill messObjectParent)
    {
        BaristapocalypseMultiplayer.Instance.PlayerCreateSpill(Mess, messObjectParent);

        int randomInt = UnityEngine.Random.Range(0, SoundManager.Instance.audioClipRefsSO.spills.Count);
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.spills[randomInt]);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            PlayerController stateMachine = other.gameObject.GetComponent<PlayerController>();
            stateMachine.anim.CrossFadeInFixedTime(BP_Barista_SlippingHash, CrossFadeDuration);
            stateMachine.additionalForce = stateMachine.rb.transform.forward * slipSpeed;
            slipSource = SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.slipOnSpill, true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<PlayerController>()) 
        { 
            PlayerController stateMachine = other.gameObject.GetComponent<PlayerController>();
            Rigidbody rb = stateMachine.rb;
            Vector3 movedirection = rb.transform.forward;

            //rb.AddForce(movedirection * slipSpeed , ForceMode.VelocityChange);
            //stateMachine.ThrowIngredient();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            PlayerController stateMachine = other.gameObject.GetComponent<PlayerController>();
            stateMachine.OnAnimationSwitch();
            slipSource.mute = true;
        }
    }

    public void ShowUi()
    {
        imageHolder.SetActive(true);
        controllerPrompt.SetActive(true);
        //if (Gamepad.current != null)
        //{
        //    controllerPrompt.SetActive(true);
        //    keyboardPrompt.SetActive(false);
        //}
        //else
        //{
        //    keyboardPrompt.SetActive(true);
        //    controllerPrompt.SetActive(false);
        //}
        
        //if(!hasMop) mopErrorHolder.SetActive(true);
    }

    public void HideUi()
    {
        imageHolder.SetActive(false);
        keyboardPrompt.SetActive(false);
        controllerPrompt.SetActive(false);
        //mopErrorHolder.SetActive(false);
        
    }
    
    
}
          
        
   
