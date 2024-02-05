using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;

public class RadioStation : BaseStation
{
    [SerializeField] private AudioClip[] Audios;
    [SerializeField] private AudioSource MainAudio;
    [SerializeField] private AudioClip ChangeSound;
    [SerializeField] private AudioClip brokenRadio;

    [SerializeField] private ParticleSystem interactParticle; // NOte could be deleted later
    private bool isButtonDown = false;
    private float buttonDownTime;
    public float holdTime = 2f;
    

    private int AudioIndex = 0;

    public override void Interact(PlayerController player)
    {
        ChangeSongDownServerRpc();
    }

    public override void InteractAlt(PlayerController player)
    {
        ChangeSongUpServerRpc();
    }

    [ServerRpc(RequireOwnership = false)] 
    private void ChangeSongDownServerRpc() 
    {
        ChangeSongDownClientRpc();
    }

    [ClientRpc]
    private void ChangeSongDownClientRpc() 
    {
        ChangeSongDown();
    }

    private void ChangeSongDown()
    {
        AudioIndex++;
        AudioIndex %= Audios.Length; // should loop
        MainAudio.clip = Audios[AudioIndex];
        MainAudio.Play();
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeSongUpServerRpc()
    {
        ChangeSongUpClientRpc();
    }

    [ClientRpc]
    private void ChangeSongUpClientRpc()
    {
        ChangeSongUp();
    }


    private void ChangeSongUp() 
    {
        AudioIndex--;
        AudioIndex = (AudioIndex + Audios.Length) % Audios.Length; // should loop
        MainAudio.clip = Audios[AudioIndex];
        MainAudio.Play();
    }

    public void EventOn() 
    {  
        
        MainAudio.clip = brokenRadio;
        MainAudio.Play();
        
    }
       
       
       
       

    public void EventOff() 
    {
        
        MainAudio.clip = Audios[AudioIndex];
        MainAudio.Play();
        DeactivateRandomEvent();
       
    }

    private void RadioFixed()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isButtonDown = true;
            buttonDownTime = Time.time;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isButtonDown = false;
        }

        if (isButtonDown && Time.time - buttonDownTime >= holdTime)
        {
            
           EventOff();

        }


    }
    private void GameManager_OnPlayerDeactivateEvent(object sender, EventArgs e)
    {
        DeactivateRandomEvent();
    }

    private void DeactivateRandomEvent()
    {

        GameManager.Instance.isEventActive = false;
        RandomEventBase randomEvent = GameManager.Instance.currentRandomEvent;

        
        if (!randomEvent.GetNetworkObject().IsSpawned)
        {
            randomEvent.GetNetworkObject().Spawn();
        }
        Debug.LogWarning("DeactivateRandomEvent " + randomEvent.name);
        randomEvent.SetEventBool(false);
        randomEvent.ActivateDeactivateEvent();
    }
}
