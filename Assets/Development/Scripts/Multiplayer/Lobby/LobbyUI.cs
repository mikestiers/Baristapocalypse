using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [Header("Panels")] 
    [SerializeField] private GameObject multiplayerPanel;
    [SerializeField] private GameObject hostSettingsPanel;

    [Header("Buttons")] [SerializeField] private Button hostLobbyButton;
    [SerializeField] private Button quickJoinLobbyButton;
    [SerializeField] private Button joinLobbyByCodeButton;
    [SerializeField] private Button createPrivateLobbyButton;
    [SerializeField] private Button createPublicLobbyButton;
    [SerializeField] private Button backButton;

    [SerializeField] private TMP_Dropdown dropdownPlayers;
    private int selectedMaxPlayers;
    [Header("Input Field")] [SerializeField]
    private TMP_InputField lobbyNameInputField;

    [SerializeField] private TMP_InputField maxPlayersInputField;
    [SerializeField] private TMP_InputField lobbyCodeInputField;


    [SerializeField] private Transform lobbyContainer;
    [SerializeField] private Transform lobbyTemplate;

    private void Awake()
    {
        selectedMaxPlayers = int.Parse(dropdownPlayers.options[dropdownPlayers.value].text);
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
            quickJoinLobbyButton.onClick.AddListener(() => { LobbyManager.Instance.QuickJoinLobby(); });
        }

        if (joinLobbyByCodeButton)
        {
            joinLobbyByCodeButton.onClick.AddListener(() =>
            {
                LobbyManager.Instance.JoinLobbyByCode(lobbyCodeInputField.text);
            });
        }

        if (dropdownPlayers)
        {
            dropdownPlayers.onValueChanged.AddListener(OnDropdownValueChanged);
        }

        if (createPrivateLobbyButton)
        {
            createPrivateLobbyButton.onClick.AddListener(() =>
            {
                LobbyManager.Instance.CreateLobby(lobbyNameInputField.text, true, selectedMaxPlayers);
            });
        }

        if (createPublicLobbyButton)
        {
            createPublicLobbyButton.onClick.AddListener(() =>
            {
                LobbyManager.Instance.CreateLobby(lobbyNameInputField.text, false, selectedMaxPlayers);
            });
        }

        lobbyTemplate.gameObject.SetActive(false);
    }

    private void OnDropdownValueChanged(int numPlayers)
    {
        selectedMaxPlayers = int.Parse(dropdownPlayers.options[dropdownPlayers.value].text);
        Debug.Log("Max players " + selectedMaxPlayers);
    }

    private void Start()
    {
        if (BaristapocalypseMultiplayer.playMultiplayer == false)
        {
            multiplayerPanel.SetActive(false);
        }

        LobbyManager.Instance.OnLobbyListChanged += LobbyManager_OnLobbyListChanged;
        UpdateLobbyList(new List<Lobby>());
    }

    private void LobbyManager_OnLobbyListChanged(object sender, LobbyManager.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        foreach (Transform child in lobbyContainer)
        {
            if (child == lobbyTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (Lobby lobby in lobbyList)
        {
            Transform lobbyTransform = Instantiate(lobbyTemplate, lobbyContainer);
            lobbyTransform.gameObject.SetActive(true);
            lobbyTransform.GetComponent<LobbyListSingleUI>().SetLobby(lobby);
        }
    }

    private void OnDestroy()
    {
        LobbyManager.Instance.OnLobbyListChanged -= LobbyManager_OnLobbyListChanged;
    }

    

}
