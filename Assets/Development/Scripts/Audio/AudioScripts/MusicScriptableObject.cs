using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class MusicScriptableObject : ScriptableObject
{
    public int numOfStations;

    [Header("Metal Station")]
    public AudioClip[] metalMusics;
    public AudioClip[] metalIntros;
    public AudioClip[] metalSponsor;

    [Header("Perseus Station")]
    public AudioClip[] perseusMusics;
    public AudioClip[] perseusIntros;
    public AudioClip[] perseusSponsor;

    [Header("Synthwave Station")]
    public AudioClip[] synthwaveMusics;
    public AudioClip[] synthwaveIntros;
    public AudioClip[] synthwaveSponsor;

    [Header("Ads")]
    public AudioClip[] ads;
}
