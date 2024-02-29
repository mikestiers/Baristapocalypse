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
    [SerializeField] private AudioClip BrokenSound;
    [SerializeField] private ParticleSystem interactParticle; // NOte could be deleted later
    [SerializeField] private GameObject eventLight;
    private int AudioIndex = 0;

    public override void Interact(PlayerController player)
    {
        ChangeSongDownServerRpc();
        EventOffServerRpc();
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

    [ClientRpc]
    public void EventOnClientRpc()
    {
        EventOn();
    }

    public void EventOn() 
    {
        MainAudio.clip = BrokenSound;
        MainAudio.Play();
        eventLight.SetActive(true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void EventOffServerRpc()
    {
        EventOffClientRpc();
    }

    [ClientRpc]
    public void EventOffClientRpc()
    {
        EventOff();
    }

    public void EventOff() 
    {
        eventLight.SetActive(false);
        GameManager.Instance.isEventActive.Value = false;
        MainAudio.clip = Audios[AudioIndex];
        MainAudio.Play();
    }
}
