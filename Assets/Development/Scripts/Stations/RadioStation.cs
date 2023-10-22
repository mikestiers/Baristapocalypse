using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
     

        AudioIndex++;
        AudioIndex %= Audios.Length; // should loop
        MainAudio.clip = Audios[AudioIndex];
        MainAudio.Play();
        
    }

    public override void InteractAlt(PlayerController player)
    {
        AudioIndex--;
        AudioIndex = (AudioIndex + Audios.Length) % Audios.Length; // should loop
        MainAudio.clip = Audios[AudioIndex];
        MainAudio.Play();
    }
   
    
}
