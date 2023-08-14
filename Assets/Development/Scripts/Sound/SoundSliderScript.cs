using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

[RequireComponent(typeof(Slider))]

public class SoundSliderScript : MonoBehaviour
{
    public AudioMixer MasterVolume;

    [SerializeField]
    private string volumeName;


    Slider slider
    {
        get
        {
            return GetComponent<Slider>();
        }
    }



    private void Start()
    {
        UpdateValueOnChange(slider.value);
    }

    public void UpdateValueOnChange(float value)
    {
        MasterVolume.SetFloat(volumeName, Mathf.Log(value) * 20f);
    }
}
