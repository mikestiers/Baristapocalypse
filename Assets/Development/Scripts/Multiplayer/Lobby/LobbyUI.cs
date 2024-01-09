using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [Header ("Panels")]
    [SerializeField] private GameObject MultiplayerPanel;
    [SerializeField] private GameObject HostSettingsPanel;
    [SerializeField] private GameObject LobbyPanel;
    [SerializeField] private GameObject JoinLobbyPanel;

    [Header ("Buttons")]
    [SerializeField] private Button HostLobbyButton;
    [SerializeField] private Button JoinLobbyButton;
    [SerializeField] private Button StartHostingButton;
    [SerializeField] private Button BackButton;

    private void Start()
    {
        if (HostLobbyButton)
            HostLobbyButton.onClick.AddListener(ToHostSettings);
        if (JoinLobbyButton)
            JoinLobbyButton.onClick.AddListener(ToJoinLobbies);
        if (StartHostingButton)
            StartHostingButton.onClick.AddListener(ToLobby);
        if (BackButton)
            BackButton.onClick.AddListener(ToPreviousPanel);
    }

    private void ToHostSettings()
    {
        MultiplayerPanel.SetActive(false);
        HostSettingsPanel.SetActive(true);
    }

    private void ToJoinLobbies()
    {
        MultiplayerPanel.SetActive(false);
        JoinLobbyPanel.SetActive(true);
    }

    private void ToLobby()
    {
        //Set up logic to Load into a specific that lobby and display on the lobby stuff

        HostSettingsPanel.SetActive(false);
        LobbyPanel.SetActive(true);
    }

    //Goes to the previous Panel by checking which panel is currently active
    private void ToPreviousPanel()
    {
        if (MultiplayerPanel.activeInHierarchy)
        {
            //leave Multiplayer scene/section
            return;
        }
        if (HostSettingsPanel.activeInHierarchy)
        {
            HostSettingsPanel.SetActive(false);
            MultiplayerPanel.SetActive(true);
            return;
        }
        if (LobbyPanel.activeInHierarchy)
        {
            LobbyPanel.SetActive(false);

            //Logic for closing a Lobby forcibly here!!!

            MultiplayerPanel.SetActive(true);
            return;
        }
        if(JoinLobbyPanel.activeInHierarchy)
        {
            JoinLobbyPanel.SetActive(false);
            MultiplayerPanel.SetActive(true);
        }
    }
}
