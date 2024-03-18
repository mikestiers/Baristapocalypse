using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class AudioClipRefsSO : ScriptableObject
{
    public AudioClip menuClicks;
    [Header("EventSounds")]
    public AudioClip tutorialPopOut;
    public AudioClip wifiOff;
    public AudioClip wifiOn;
    public AudioClip radioStationChange;
    [Header("WaveEndingSounds/UI")]
    public AudioClip yorpReview;
    public List<AudioClip> starlevel;
    public AudioClip tipAdded;
    public AudioClip goodGameover;
    public AudioClip badGameover;
    public AudioClip fiveSecCountdown;
    [Header("PlayerSounds")]
    public AudioClip dash;
    public AudioClip jump;
    public AudioClip[] playerFootsteps;
    public AudioClip mopping;
    public AudioClip slipOnSpill;
    public List<AudioClip> spills;
    [Header("MultiplayerSounds")]
    public AudioClip playerJoins;
    public AudioClip playerLeaves;
    public AudioClip playerColorSelect;
    [Header("Machine sounds")]
    public AudioClip interactStation;
    public AudioClip liquidMachine;
    public AudioClip sweetnerMachine;
    public AudioClip bioMatterMachine;
    public AudioClip beanMachine;
    public AudioClip drinkReady;
    public AudioClip drinkBrewing;
    public AudioClip drinkMinigameHit;
    public AudioClip drinkMinigameMiss;
    public AudioClip radioMalfunction;
    [Header("Pickups")]
    public AudioClip pickUp;
    public AudioClip throwIngredient;
    public AudioClip cupDropped;
    public AudioClip garbageChute;
    [Header("Doors")]
    public AudioClip doorOpen;
    public AudioClip doorClose;
    [Header("Customer Interactions")]
    public AudioClip[] customerFootsteps;
    public AudioClip takeOrder;
    public AudioClip interactCustomer;
    public AudioClip failedInteration;
    public AudioClip customerThrown;
    public List<AudioClip> customerPickedUpList;
    public List<AudioClip> customerSipsList;
    [Header("VolumeCheckSounds")]
    public AudioClip voiceCheck;

}
