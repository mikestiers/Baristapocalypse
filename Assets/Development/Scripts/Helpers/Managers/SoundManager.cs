using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : Singleton<SoundManager>
{
    public AudioClipRefsSO audioClipRefsSO;
    List<AudioSource> currentAudioSources = new List<AudioSource>();

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
                continue;
            }
            source.PlayOneShot(clip);
            return;
        }

        AudioSource temp = gameObject.AddComponent<AudioSource>();
        currentAudioSources.Add(temp);
        temp.PlayOneShot(clip);
    }
}
