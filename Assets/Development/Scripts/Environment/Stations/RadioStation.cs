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
    [SerializeField] private ParticleSystem interactParticle; // NOte could be deleted later
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
}
