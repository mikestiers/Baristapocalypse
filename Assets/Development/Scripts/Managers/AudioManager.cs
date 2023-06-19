using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{List<AudioSource> currentAudioSoures = new List<AudioSource>();
    // Start is called before the first frame update
    public AudioMixerGroup Master;
    public AudioMixerGroup sfxGroup;
    public AudioMixerGroup musicGroup;

    void Start()
    {

        currentAudioSoures.Add(gameObject.GetComponent<AudioSource>());
    }   
   public void Playoneshot(AudioClip clip, bool isMusic)
        { 
        foreach (AudioSource source in currentAudioSoures)
        {
            if (source.isPlaying)
                continue;

            source.PlayOneShot(clip);
            source.outputAudioMixerGroup = isMusic ? musicGroup : sfxGroup;
            return;
        }

        AudioSource temp = gameObject.AddComponent<AudioSource>();
        currentAudioSoures.Add(temp);
        temp.PlayOneShot(clip);
        temp.outputAudioMixerGroup = isMusic ? musicGroup : sfxGroup;
    }
   
}
