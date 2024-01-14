using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{
    private void Start()
    {
        BaristapocalypseMultiplayer.Instance.OnTryingToJoinGame += BaristapocalypseMultiplayer_OnTryingToJoinGame;
        BaristapocalypseMultiplayer.Instance.OnFailToJoinGame += BaristapocalypseMultiplayer_OnFailToJoinGame;
             
        Hide();
    }

    private void BaristapocalypseMultiplayer_OnFailToJoinGame(object sender, EventArgs e)
    {
        Hide();
    }

    private void BaristapocalypseMultiplayer_OnTryingToJoinGame(object sender, EventArgs e)
    {
        Show();
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
        BaristapocalypseMultiplayer.Instance.OnTryingToJoinGame -= BaristapocalypseMultiplayer_OnTryingToJoinGame;
        BaristapocalypseMultiplayer.Instance.OnFailToJoinGame -= BaristapocalypseMultiplayer_OnFailToJoinGame;
    }
}
