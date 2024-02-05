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
        EventOff();
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
        GameManager.Instance.isEventActive = false;
        MainAudio.clip = Audios[AudioIndex];
        MainAudio.Play();
       
       
    }

  
   
}
