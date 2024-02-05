using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] public AudioMixer mixer;
    [SerializeField] public Slider musicSlider;
    [SerializeField] public Slider SFXSlider;


    private void Start()
    {
        SetMusicVolume();
    }

    public void SetMusicVolume()
    {
        float Musvolume = musicSlider.value;
        mixer.SetFloat("Music", Mathf.Log10(Musvolume) * 20);
    }

    public void SetSFXVolume()
    {
        float SFXvolume = SFXSlider.value;
        mixer.SetFloat("SFX", Mathf.Log10(SFXvolume) * 20);
    }
}
