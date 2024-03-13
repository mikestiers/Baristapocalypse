using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UI;

public class RoombotConsole : BaseStation
{
    [SerializeField] private CleanupBot bot;
    [SerializeField] private TextMeshProUGUI uiBotMode;
    [SerializeField] private GameObject interactImage;
    private PlayerController player;
    private void OnEnable()
    {
        InputManager.OnInputChanged += InputUpdated;
    }

    private void OnDisable() 
    {
        InputManager.OnInputChanged -= InputUpdated;
    }

    // Start is called before the first frame update
    public void Start()
    {
        //to just check if no roomba get 1 from scene
        if (bot == null) bot = FindObjectOfType<CleanupBot>();
        if (bot != null) 
        {
            uiBotMode.text = bot.currentState.ToString();
            bot.SetConsole(this);
            
        }
        
    }

    public override void Interact(PlayerController player)
    {
        if (bot == null) return; //if roomba is not around for some reason
        switch(bot.currentState)
        {
            case CleanupBot.BotState.Standby:
                bot.currentState = CleanupBot.BotState.Roaming;
                break;

            case CleanupBot.BotState.Roaming:
                bot.currentState = CleanupBot.BotState.Emptying;
                break;
        }
    }
    private void InputUpdated(InputImagesSO inputImagesSO)
    {
        interactImage.GetComponentInChildren<Image>().sprite = inputImagesSO.interact;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            player = other.GetComponent<PlayerController>();
            if (player.IsLocalPlayer) interactImage.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            player = other.GetComponent<PlayerController>();
            if (player.IsLocalPlayer) interactImage.SetActive(false);
        }
    }

    public void SetUIBotMode(string botMode)
    {
        uiBotMode.text = botMode;
    }

}
