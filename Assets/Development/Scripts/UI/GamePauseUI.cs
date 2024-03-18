using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{
    public Button toGame;

    private void Awake()
    {
        // add buttons functionality
        // add NetworkManager.Singleton.Shutdown(); on quit

    }

    private void Start()
    {
        GameManager.Instance.OnLocalGamePaused += GameManager_OnLocalGamePaused;
        GameManager.Instance.OnLocalGameUnpaused += GameManager_OnLocalGameUnpaused;

        toGame.onClick.AddListener(ResumeGame);

        Hide();
    }

    private void GameManager_OnLocalGameUnpaused(object sender, EventArgs e)
    {
        Hide();
    }

    private void GameManager_OnLocalGamePaused(object sender, EventArgs e)
    {
        Show();
    }

    private void ResumeGame()
    {
        GameManager.Instance.TogglePauseGame();
    }

    private void Show()
    {
        gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(Gamepad.current != null ? toGame.gameObject : null);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }


}
