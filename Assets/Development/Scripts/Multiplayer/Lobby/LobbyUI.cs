using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [Header ("Panels")]
    [SerializeField] private GameObject multiplayerPanel;
    [SerializeField] private GameObject hostSettingsPanel;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject joinLobbyPanel;

    [Header ("Buttons")]
    [SerializeField] private Button hostLobbyButton;
    [SerializeField] private Button joinLobbyButton;
    [SerializeField] private Button startHostingButton;
    [SerializeField] private Button backButton;

    private void Awake()
    {
        if (hostLobbyButton)
        {
            hostLobbyButton.onClick.AddListener(() =>
            {
                BaristapocalypseMultiplayer.Instance.StartHost();
                Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
            });
        }
            
        if (joinLobbyButton)
        {
            joinLobbyButton.onClick.AddListener(() =>
            {
                BaristapocalypseMultiplayer.Instance.StartClient();
            });
        }
    }

    
}
