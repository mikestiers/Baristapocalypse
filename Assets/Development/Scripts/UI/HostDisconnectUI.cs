using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class HostDisconnectUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    private string mainMenuScene = "LobbyScene";

    private void Awake()
    {
        if (mainMenuButton)
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;

        Hide();
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        if (clientId == NetworkManager.ServerClientId) 
        {
            // Server is shutting down
            Show();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void ReturnToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
        NetworkManager.Singleton.Shutdown();
        Hide();
    }

}
