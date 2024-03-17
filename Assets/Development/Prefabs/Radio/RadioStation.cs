using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RadioStation : BaseStation
{
    [SerializeField] private AudioClip[] Audios;
    [SerializeField] private AudioSource MainAudio;
    [SerializeField] private AudioClip ChangeSound;
    [SerializeField] private AudioClip BrokenSound;
    [SerializeField] private ParticleSystem interactParticle; // NOte could be deleted later
    [SerializeField] private GameObject eventLight;
    bool eventIsOn = false;
    private int AudioIndex = 0;

    public override void Interact(PlayerController player)
    {
        if (eventIsOn == false) ChangeSongDownServerRpc();
        else if (eventIsOn == true) { MoveSlider(-1); }
    }

    public override void InteractAlt(PlayerController player)
    {
        if (eventIsOn == false) ChangeSongUpServerRpc(); 
        else if (eventIsOn == true) { MoveSlider(1); }
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
        {
            AudioIndex++;
            AudioIndex %= Audios.Length; // should loop
            MainAudio.clip = Audios[AudioIndex];
            float randomTime = Random.Range(0f, Audios[AudioIndex].length);
            MainAudio.time = randomTime;
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
   
        {
            AudioIndex--;
            AudioIndex = (AudioIndex + Audios.Length) % Audios.Length; // should loop
            MainAudio.clip = Audios[AudioIndex];
            float randomTime = Random.Range(0f, Audios[AudioIndex].length);
            MainAudio.time = randomTime;
            MainAudio.Play();
        }
        
    }

    [ClientRpc]
    public void EventOnClientRpc()
    {
        EventOn();
    }

    public void EventOn() 
    {
        screenEffect.ToggleRadioEffect(eventIsOn);
        eventIsOn = true;
        MainAudio.clip = BrokenSound;
        MainAudio.Play();
        eventLight.SetActive(true);
        ChangeRandomTextColorPair();
        Ui.gameObject.SetActive(true);
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void EventOffServerRpc()
    {
        EventOffClientRpc();
    }

    [ClientRpc]
    public void EventOffClientRpc()
    {
        EventOff();
    }

    public void EventOff() 
    {
        eventIsOn = false;
        screenEffect.ToggleRadioEffect(eventIsOn);
        eventLight.SetActive(false);
        GameManager.Instance.isEventActive.Value = false;
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

    void Start()
    {
        // Set up slider
        slider.minValue = 0f;
        slider.maxValue = 25f;
        slider.wholeNumbers = true;
        slider.value = 0f; // Start at 0

        ChangeSongUpServerRpc();
    }

  

    void ChangeRandomTextColorPair()
    {

        if (textPairs.Count > 0)
        {
            // Choose a random index within the list
           currentRandomIndex = RandomNumber(); Debug.LogWarning("current random Index" + currentRandomIndex);

            // Change the text color of both TextMeshProUGUI objects in the randomly selected pair to red
            textPairs[currentRandomIndex].text1.color = Color.red;
            textPairs[currentRandomIndex].text2.color = Color.red;
            currentGoal = textPairs[currentRandomIndex].goalValue;
        }
        else
        {
            Debug.LogError("Text Pair List is empty!");
        }
    }

    private int RandomNumber() 
    {
        int randomIndex = Random.Range(0, textPairs.Count);
        return randomIndex;
    }

    void MoveSlider(int direction)
    {
        slider.value = Mathf.Clamp(slider.value + direction, slider.minValue, slider.maxValue);

        float rotationAngle = Mathf.Lerp(minRotationAngle, maxRotationAngle, slider.normalizedValue);
        rotatingImage.transform.rotation = Quaternion.Euler(0f, 0f, rotationAngle);

        // Check if the slider value matches any of the goal values
        
        {
            if (slider.value == currentGoal)
            {
                Debug.LogWarning("has entered white");// Turn text color white for the pair
                textPairs[currentRandomIndex].text1.color = Color.white;
                textPairs[currentRandomIndex].text2.color = Color.white;

                EventOffServerRpc();
            }
        }
    }
}
