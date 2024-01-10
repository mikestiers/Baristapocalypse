using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionResponseUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(Hide);
    }

    private void Start()
    {
        BaristapocalypseMultiplayer.Instance.OnFailToJoinGame += BaristapocalypseMultiplayer_OnFailToJoinGame;

        Hide();
    }

    private void BaristapocalypseMultiplayer_OnFailToJoinGame(object sender, EventArgs e)
    {
        Show();

        messageText.text = NetworkManager.Singleton.DisconnectReason;

        if(messageText.text == "")
        {
            messageText.text = "Connection timed out";
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

    private void OnDestroy()
    {
        BaristapocalypseMultiplayer.Instance.OnFailToJoinGame -= BaristapocalypseMultiplayer_OnFailToJoinGame;
    }
}
