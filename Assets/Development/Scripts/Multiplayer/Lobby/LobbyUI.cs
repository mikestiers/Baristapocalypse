using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [Header ("Panels")]
    [SerializeField] private GameObject multiplayerPanel;
    [SerializeField] private GameObject hostSettingsPanel;

    [Header ("Buttons")]
    [SerializeField] private Button hostLobbyButton;
    [SerializeField] private Button quickJoinLobbyButton;
    [SerializeField] private Button joinLobbyByCodeButton;
    [SerializeField] private Button createPrivateLobbyButton;
    [SerializeField] private Button createPublicLobbyButton;
    [SerializeField] private Button backButton;

    [Header("Input Field")]
    [SerializeField] private TMP_InputField lobbyNameInputField;
    [SerializeField] private TMP_InputField maxPlayersInputField;
    [SerializeField] private TMP_InputField lobbyCodeInputField;

    private void Awake()
    {
        if (hostLobbyButton)
        {
            hostLobbyButton.onClick.AddListener(() =>
            {
                multiplayerPanel.SetActive(false);
                hostSettingsPanel.SetActive(true);
            });
        }
            
        if (quickJoinLobbyButton)
        {
            quickJoinLobbyButton.onClick.AddListener(() =>
            {
                LobbyManager.Instance.QuickJoinLobby();
            });
        }

        if(joinLobbyByCodeButton)
        {
            joinLobbyByCodeButton.onClick.AddListener(() =>
            {
                LobbyManager.Instance.JoinLobbyByCode(lobbyCodeInputField.text);
            });
        }

        if (createPrivateLobbyButton)
        {
            createPrivateLobbyButton.onClick.AddListener(() =>
            {
                LobbyManager.Instance.CreateLobby(lobbyNameInputField.text, true, int.Parse(maxPlayersInputField.text));
            });
        }

        if (createPublicLobbyButton)
        {
            createPublicLobbyButton.onClick.AddListener(() =>
            {
                LobbyManager.Instance.CreateLobby(lobbyNameInputField.text, false, int.Parse(maxPlayersInputField.text));
            });
        }
    }

    
}
