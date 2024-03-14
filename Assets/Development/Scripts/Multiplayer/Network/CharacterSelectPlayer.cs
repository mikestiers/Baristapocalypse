using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectPlayer : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject readyText;
    [SerializeField] private PlayerColorChoice playerVisual;
    
    private void Start()
    {
        BaristapocalypseMultiplayer.Instance.OnPlayerDataNetworkListChanged += BaristapocalypseMultiplayer_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyChanged += CharacterSelectReady_OnReadyChanged;

        UpdatePlayer();
    }

    private void CharacterSelectReady_OnReadyChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    private void BaristapocalypseMultiplayer_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    private void UpdatePlayer()
    {
        if(BaristapocalypseMultiplayer.Instance.IsPlayerIndexConnected(playerIndex))
        {
            Show();

            PlayerData playerData = BaristapocalypseMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            readyText.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientId));

            playerVisual.SetPlayerColor(BaristapocalypseMultiplayer.Instance.GetPlayerColor(playerData.colorId));
            playerVisual.SetRingColor(BaristapocalypseMultiplayer.Instance.GetPlayerEmissionColor((playerData.colorId)));
        }
        else
        {
            Hide();
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
        BaristapocalypseMultiplayer.Instance.OnPlayerDataNetworkListChanged -= BaristapocalypseMultiplayer_OnPlayerDataNetworkListChanged;
    }
}
