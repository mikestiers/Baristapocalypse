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

    private void Start()
    {
        if (hostLobbyButton)
            hostLobbyButton.onClick.AddListener(ToHostSettings);
        if (joinLobbyButton)
            joinLobbyButton.onClick.AddListener(ToJoinLobbies);
        if (startHostingButton)
            startHostingButton.onClick.AddListener(ToLobby);
        if (backButton)
            backButton.onClick.AddListener(ToPreviousPanel);
    }

    private void ToHostSettings()
    {
        multiplayerPanel.SetActive(false);
        hostSettingsPanel.SetActive(true);
    }

    private void ToJoinLobbies()
    {
        multiplayerPanel.SetActive(false);
        joinLobbyPanel.SetActive(true);
    }

    private void ToLobby()
    {
        //Set up logic to Load into a specific that lobby and display on the lobby stuff

        hostSettingsPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    //Goes to the previous Panel by checking which panel is currently active
    private void ToPreviousPanel()
    {
        if (multiplayerPanel.activeInHierarchy)
        {
            //leave Multiplayer scene/section
            return;
        }
        if (hostSettingsPanel.activeInHierarchy)
        {
            hostSettingsPanel.SetActive(false);
            multiplayerPanel.SetActive(true);
            return;
        }
        if (lobbyPanel.activeInHierarchy)
        {
            lobbyPanel.SetActive(false);

            //Logic for closing a Lobby forcibly here!!!

            multiplayerPanel.SetActive(true);
            return;
        }
        if(joinLobbyPanel.activeInHierarchy)
        {
            joinLobbyPanel.SetActive(false);
            multiplayerPanel.SetActive(true);
        }
    }
}
