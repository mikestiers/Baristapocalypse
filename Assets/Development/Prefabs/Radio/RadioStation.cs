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
    [SerializeField] private AudioClip[] Audios;
    [SerializeField] private AudioSource MainAudio;
    [SerializeField] private AudioClip ChangeSound;
    [SerializeField] private AudioClip BrokenSound;
    [SerializeField] private ParticleSystem interactParticle; // NOte could be deleted later
    [SerializeField] private GameObject eventLight;
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
        AudioIndex %= Audios.Length; // should loop
        MainAudio.clip = Audios[AudioIndex];
        MainAudio.Play();
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
        AudioIndex = (AudioIndex + Audios.Length) % Audios.Length; // should loop
        MainAudio.clip = Audios[AudioIndex];
        MainAudio.Play();
        
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
        screenEffect.ToggleRadioEffect(eventIsOn.Value);
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
        screenEffect.ToggleRadioEffect(eventIsOn.Value);
        eventLight.SetActive(false);
        MainAudio.clip = Audios[AudioIndex];
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
}
                
              
