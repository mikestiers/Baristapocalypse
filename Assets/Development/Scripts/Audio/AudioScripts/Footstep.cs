using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footstep : MonoBehaviour
{
    public AudioClip footstep;

    public void stepSound()
    {
        SoundManager.Instance.PlayOneShot(footstep);
    }
}
