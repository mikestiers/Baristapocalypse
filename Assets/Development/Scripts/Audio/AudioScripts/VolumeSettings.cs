using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] public AudioMixer mixer;
    [SerializeField] public Slider masterSlider;
    [SerializeField] public Slider musicSlider;
    [SerializeField] public Slider sFXSlider;
    [SerializeField] public Slider voicesSlider;

    public AudioSource musicSource;
    public AudioSource sFXSource;

    private const string MusicLevelKey = "MusicVolume";
    private const string SFXLevelKey = "SFXVolume";
    private const string VoicesLevelKey = "VoicesVolume";
    private const string MasterLevelKey = "MasterVolume";

    private void Start()
    {
        // Initialize the slider value
        masterSlider.value = GetMasterLevel();
        musicSlider.value = GetMusicLevel();
        sFXSlider.value = GetSFXLevel();
        voicesSlider.value = GetVoicesLevel();

        // Add listener for the slider value change
        masterSlider.onValueChanged.AddListener(SetMasterLevel);
        musicSlider.onValueChanged.AddListener(SetMusicLevel);
        sFXSlider.onValueChanged.AddListener(SetSFXLevel);
        voicesSlider.onValueChanged.AddListener(SetVoicesLevel);

    }

    private void SetMasterLevel(float masterLevel)
    {
        PlayerPrefs.SetFloat(MasterLevelKey, masterLevel);
        mixer.SetFloat(MasterLevelKey, MathF.Log10(masterLevel) * 20);
    }

    public float GetMasterLevel()
    {
        float masterLevel = PlayerPrefs.GetFloat(MasterLevelKey, 0.5f);
        mixer.SetFloat(MasterLevelKey, MathF.Log10(masterLevel) * 20);
        return PlayerPrefs.GetFloat(MasterLevelKey, 1.0f);
    }

    private void SetMusicLevel(float musicLevel)
    {
        PlayerPrefs.SetFloat(MusicLevelKey, musicLevel);
        mixer.SetFloat(MusicLevelKey, MathF.Log10(musicLevel) * 20);
    }

    public float GetMusicLevel()
    {
        float musicLevel = PlayerPrefs.GetFloat(MusicLevelKey, 0.5f);
        mixer.SetFloat(MusicLevelKey, MathF.Log10(musicLevel) * 20);
        return PlayerPrefs.GetFloat(MusicLevelKey, 1.0f);
    }

    private void SetSFXLevel(float sFXLevel)
    {
        PlayerPrefs.SetFloat(SFXLevelKey, sFXLevel);
        mixer.SetFloat(SFXLevelKey, MathF.Log10(sFXLevel) * 20);
    }
    public float GetSFXLevel()
    {
        float sFXLevel = PlayerPrefs.GetFloat(SFXLevelKey, 0.5f);
        mixer.SetFloat(SFXLevelKey, MathF.Log10(sFXLevel) * 20);
        return PlayerPrefs.GetFloat(SFXLevelKey, 1.0f);
    }

    private void SetVoicesLevel(float voicesLevel)
    {
        PlayerPrefs.SetFloat(VoicesLevelKey, voicesLevel);
        mixer.SetFloat(VoicesLevelKey, MathF.Log10(voicesLevel) * 20);
    }

    public float GetVoicesLevel()
    {
        float voicesLevel = PlayerPrefs.GetFloat(VoicesLevelKey, 0.5f);
        mixer.SetFloat(VoicesLevelKey, MathF.Log10(voicesLevel) * 20);
        return PlayerPrefs.GetFloat(VoicesLevelKey, 1.0f);
    }
}
