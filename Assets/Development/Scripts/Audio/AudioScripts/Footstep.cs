using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footstep : MonoBehaviour
{
    public AudioSource customerFootstepAudioSource;
    [HideInInspector]
    public float volume = 1;

    private void Update()
    {
        if (customerFootstepAudioSource != null)
            customerFootstepAudioSource.volume = volume;  
    }

    public void stepSound()
    {
        int temp = Random.Range(0, SoundManager.Instance.audioClipRefsSO.playerFootsteps.Length);
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.playerFootsteps[temp], 0.3f);
    }
    
    public void customerStepSound()
    {
        int temp = Random.Range(0, SoundManager.Instance.audioClipRefsSO.customerFootsteps.Length);
        customerFootstepAudioSource.clip = SoundManager.Instance.audioClipRefsSO.customerFootsteps[temp];
        customerFootstepAudioSource.Play();
    }
}
