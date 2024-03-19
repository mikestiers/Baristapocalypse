using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RadioStation : BaseStation
{
    public MusicScriptableObject musicRefsSO;
    public AudioClip currentSong;
    [SerializeField] private AudioClip[] Stations;
    [SerializeField] private AudioSource MainAudio;
    [SerializeField] private AudioClip ChangeSound;
    [SerializeField] private AudioClip BrokenSound;
    [SerializeField] private ParticleSystem interactParticle; // Note could be deleted later
    [SerializeField] private GameObject eventLight;
    private bool guaranteeSong = false;
    private bool ignoreRandom = false;
    private bool guaranteeAd = false;
    [SerializeField] private NetworkVariable<bool> eventIsOn = new NetworkVariable<bool>(false);
    private int AudioIndex = 0;

    public override void Interact(PlayerController player)
    {
        if (eventIsOn.Value == false) ChangeSongDownServerRpc();
        else if (eventIsOn.Value == true) { MoveSlider(-1); }
    }

    public override void InteractAlt(PlayerController player)
    {
        if (eventIsOn.Value == false) ChangeSongUpServerRpc(); 
        else if (eventIsOn.Value == true) { MoveSlider(1); }
    }

    [ServerRpc(RequireOwnership = false)] 
    private void ChangeSongDownServerRpc() 
    {
        ChangeSongDownClientRpc();
    }

    [ClientRpc]
    private void ChangeSongDownClientRpc() 
    {
        ChangeSongDown();
    }

    private void ChangeSongDown()
    {
        AudioIndex++;
        AudioIndex %= musicRefsSO.numOfStations;
        Debug.Log(AudioIndex);
        if (AudioIndex == 0)
        {
            currentSong = musicRefsSO.metalMusics[Random.Range(0, musicRefsSO.metalMusics.Length)];
            MainAudio.clip = currentSong;
            float randomTime = Random.Range(0f, currentSong.length);
            MainAudio.time = randomTime;
            MainAudio.Play();
        }
        else if (AudioIndex == 1)
        {
            currentSong = musicRefsSO.perseusMusics[Random.Range(0, musicRefsSO.perseusMusics.Length)];
            MainAudio.clip = currentSong;
            float randomTime = Random.Range(0f, currentSong.length);
            MainAudio.time = randomTime;
            MainAudio.Play();
        }
        else
        {
            currentSong = musicRefsSO.synthwaveMusics[Random.Range(0, musicRefsSO.synthwaveMusics.Length)];
            MainAudio.clip = currentSong;
            MainAudio.Play();
        }

    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeSongUpServerRpc()
    {
        ChangeSongUpClientRpc();
    }

    [ClientRpc]
    private void ChangeSongUpClientRpc()
    {
        ChangeSongUp();
    }

    private void ChangeSongUp()
    {
        AudioIndex--;
        AudioIndex %= musicRefsSO.numOfStations;
        if (AudioIndex == 0)
        {
            currentSong = musicRefsSO.metalMusics[Random.Range(0, musicRefsSO.metalMusics.Length)];
            MainAudio.clip = currentSong;
            float randomTime = Random.Range(0f, currentSong.length);
            MainAudio.time = randomTime;
            MainAudio.Play();
        }
        else if (AudioIndex == 1)
        {
            currentSong = musicRefsSO.perseusMusics[Random.Range(0, musicRefsSO.perseusMusics.Length)];
            MainAudio.clip = currentSong;
            float randomTime = Random.Range(0f, currentSong.length);
            MainAudio.time = randomTime;
            MainAudio.Play();
        }
        else
        {
            currentSong = musicRefsSO.synthwaveMusics[Random.Range(0, musicRefsSO.synthwaveMusics.Length)];
            MainAudio.clip = currentSong;
            float randomTime = Random.Range(0f, currentSong.length);
            MainAudio.time = randomTime;
            MainAudio.Play();
        }
    }

    public void EventOn()
    {
        EventOnServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void EventOnServerRpc()
    {
        eventIsOn.Value = true;
        ChangeRandomTextColorPair();
        EventOnClientRpc();
    }

    [ClientRpc]
    private void EventOnClientRpc()
    {
        screenEffect.ToggleRadioEffect(true);
        MainAudio.clip = BrokenSound;
        MainAudio.Play();
        eventLight.SetActive(true);
        Ui.gameObject.SetActive(true);
    }

    

    public void EventOff()
    {
        EventOffServerRpc();
    }


    [ServerRpc(RequireOwnership = false)]
    private void EventOffServerRpc()
    {
        GameManager.Instance.isEventActive.Value = false;
        eventIsOn.Value = false;
        EventOffClientRpc();
    }

    [ClientRpc]
    private void EventOffClientRpc()
    {
        textPairs[currentRandomIndex].text1.color = Color.white;
        textPairs[currentRandomIndex].text2.color = Color.white;
        screenEffect.ToggleRadioEffect(false);
        eventLight.SetActive(false);
        GameManager.Instance.isEventActive.Value = false;
        MainAudio.clip = Stations[AudioIndex];
        MainAudio.Play();
        Ui.gameObject.SetActive(false);
    }

    

    [SerializeField] private Slider slider;
    [SerializeField] private float moveSpeed = 1f; // Speed at which the slider moves
    [SerializeField] private BoxCollider box;
    [SerializeField] private Image rotatingImage;
    [SerializeField] private float minRotationAngle = -47.6f; // Minimum rotation angle
    [SerializeField] private float maxRotationAngle = 229.1f; // Maximum rotation angle
    [SerializeField] private GameObject Ui;

    private PlayerController player;

    private float currentGoal;
    private int currentRandomIndex;

    [System.Serializable]
    public class TextPair
    {
        public TextMeshProUGUI text1;
        public TextMeshProUGUI text2;
        public float goalValue; // Goal value associated with this pair

        // Constructor to initialize the pair with specific values
        public TextPair(TextMeshProUGUI t1, TextMeshProUGUI t2, float goal)
        {
            text1 = t1;
            text2 = t2;
            goalValue = goal;
        }
    }

    public List<TextPair> textPairs = new List<TextPair>();

    private void Start()
    {
        // Set up slider
        slider.minValue = 0f;
        slider.maxValue = 25f;
        slider.wholeNumbers = true;
        slider.value = 0f; // Start at 0

        ChangeSongUpServerRpc();
    }

    private void ChangeRandomTextColorPair()
    {
        ChangeRandomTextColorPairServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeRandomTextColorPairServerRpc()
    {
        if (textPairs.Count > 0)
        {
            // Choose a random index within the list
            int randomNum = RandomNumber();
            ChangeRandomTextColorPairClientRpc(randomNum);
        }
        else
        {
            Debug.LogError("Text Pair List is empty!");
        }
    }

    [ClientRpc]
    private void ChangeRandomTextColorPairClientRpc(int index)
    {
        // Change the text color of both TextMeshProUGUI objects in the randomly selected pair to red
        currentRandomIndex = index;
        textPairs[index].text1.color = Color.red;
        textPairs[index].text2.color = Color.red;
        currentGoal = textPairs[index].goalValue;
    }


    private int RandomNumber() 
    {
        int randomIndex = Random.Range(0, textPairs.Count);
        return randomIndex;
    }



    private void MoveSlider(int direction)
    {
        slider.value = Mathf.Clamp(slider.value + direction, slider.minValue, slider.maxValue);

        float rotationAngle = Mathf.Lerp(minRotationAngle, maxRotationAngle, slider.normalizedValue);
        rotatingImage.transform.rotation = Quaternion.Euler(0f, 0f, rotationAngle);

        // Check if the slider value matches any of the goal values
        if (slider.value == currentGoal)
        {
            EventOff();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            player = other.GetComponent<PlayerController>();

            //Display UI ingredient menu
            if (player.IsLocalPlayer)
            {
                if(eventIsOn.Value == true) 
                {
                    GetComponentInParent<CameraStation1>().SwitchCameraOn(); 
                }
            }
        }
    }
                

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" )
        {
            player = other.GetComponent<PlayerController>();

            if (player.IsLocalPlayer )
            {
                GetComponentInParent<CameraStation1>().SwitchCameraOff();
            }
        }
    }

    private void Update()
    {
        if (currentSong != null)
        {
            if (!MainAudio.isPlaying)
            {
                OnAudioFinished();
            }
        }
    }

    void OnAudioFinished()
    {
        float randomChance = Random.Range(0, 4);

        //Random chance for a ad at song end
        if(randomChance == 0 && ignoreRandom != true)
        {
            if (AudioIndex == 0)
            {
                currentSong = musicRefsSO.metalSponsor[Random.Range(0, musicRefsSO.metalSponsor.Length)];
                MainAudio.clip = currentSong;
                MainAudio.Play();
            }
            else if (AudioIndex == 1)
            {
                currentSong = musicRefsSO.perseusSponsor[Random.Range(0, musicRefsSO.perseusSponsor.Length)];
                MainAudio.clip = currentSong;
                MainAudio.Play();
            }
            else
            {
                //currently doesnt have a ad read
            }
            ignoreRandom = true;
            guaranteeAd = true;
        }

        else if (ignoreRandom == true && guaranteeAd == true)
        { 
            currentSong = musicRefsSO.ads[Random.Range(0, musicRefsSO.ads.Length)];
            MainAudio.clip = currentSong;
            MainAudio.Play();
            guaranteeAd = false;
        }

        //guarenteed song after a ad plays
        else if (guaranteeSong == true)
        {
            if (AudioIndex == 0)
            {
                currentSong = musicRefsSO.metalMusics[Random.Range(0, musicRefsSO.metalMusics.Length)];
                MainAudio.clip = currentSong;
                MainAudio.Play();
            }
            else if (AudioIndex == 1)
            {
                currentSong = musicRefsSO.perseusMusics[Random.Range(0, musicRefsSO.perseusMusics.Length)];
                MainAudio.clip = currentSong;
                MainAudio.Play();
            }
            else
            {
                currentSong = musicRefsSO.synthwaveMusics[Random.Range(0, musicRefsSO.synthwaveMusics.Length)];
                MainAudio.clip = currentSong;
                MainAudio.Play();
            }
            guaranteeSong = false;
            ignoreRandom = false;
        }

        //intros for the next song
        else
        {
            if (AudioIndex == 0)
            {
                currentSong = musicRefsSO.metalIntros[Random.Range(0, musicRefsSO.metalIntros.Length)];
                MainAudio.clip = currentSong;
                MainAudio.Play();
            }
            else if (AudioIndex == 1)
            {
                currentSong = musicRefsSO.perseusIntros[Random.Range(0, musicRefsSO.perseusIntros.Length)];
                MainAudio.clip = currentSong;
                MainAudio.Play();
            }
            else
            {
                currentSong = musicRefsSO.synthwaveIntros[Random.Range(0, musicRefsSO.synthwaveIntros.Length)];
                MainAudio.clip = currentSong;
                MainAudio.Play();
            }
            guaranteeSong = true;
        }
    }
}
                
              
