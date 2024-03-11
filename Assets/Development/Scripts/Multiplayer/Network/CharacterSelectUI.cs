using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;

    private void Awake()
    {
        if (mainMenuButton)
        {
            mainMenuButton.onClick.AddListener(() =>
            {
                LobbyManager.Instance.LeaveLobby();
                NetworkManager.Singleton.Shutdown();
                Loader.LoadNetwork(Loader.Scene.MainMenuScene);
            });
        }

        if (readyButton)
        {
            readyButton.onClick.AddListener(() =>
            {
                CharacterSelectReady.Instance.SetPlayerReady();
            });
        }
        
    }

    private void Start()
    {
        Lobby lobby = LobbyManager.Instance.GetLobby();
        lobbyNameText.text = "Lobby Name: " + lobby.Name;
        lobbyCodeText.text = "Code: " + lobby.LobbyCode;
    }
}   
