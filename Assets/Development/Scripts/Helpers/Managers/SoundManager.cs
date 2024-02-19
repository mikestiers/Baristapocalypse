using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : Singleton<SoundManager>
{
    public AudioClipRefsSO audioClipRefsSO;
    List<AudioSource> currentAudioSources = new List<AudioSource>();

    public AudioMixerGroup sFXMixerGroup;


    // Start is called before the first frame update
    void Start()
    {
        currentAudioSources.Add(gameObject.GetComponent<AudioSource>());
    }

    public void PlayOneShot(AudioClip clip)
    {
        foreach (AudioSource source in currentAudioSources)
        {
            if (source.isPlaying)
            {
                break;
            }
            source.PlayOneShot(clip);
            return;
        }

        AudioSource temp = gameObject.AddComponent<AudioSource>();
        temp.outputAudioMixerGroup = sFXMixerGroup;
        currentAudioSources.Add(temp);
        temp.PlayOneShot(clip);
    }
}
