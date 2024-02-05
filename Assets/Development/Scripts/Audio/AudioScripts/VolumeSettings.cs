using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider SFXSlider;


    private void Start()
    {
        SetMusicVolume();
    }

    public void SetMusicVolume()
    {
        float Musvolume = musicSlider.value;
        myMixer.SetFloat("Music", Mathf.Log10(Musvolume) * 20);
    }

    public void SetSFXVolume()
    {
        float SFXvolume = SFXSlider.value;
        myMixer.SetFloat("SFX", Mathf.Log10(SFXvolume) * 20);
    }
}
