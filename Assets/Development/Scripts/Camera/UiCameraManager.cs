using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class UiCameraManager : MonoBehaviour
{
    [Header("Cameras")]
    public CinemachineVirtualCamera MainmenuCamera;
    public CinemachineVirtualCamera SettingsCamera;
    public CinemachineVirtualCamera PlayerSelectionCamera;
    public CinemachineVirtualCamera QuitGameCamera;
    [Header("Button")]
    // Main Menu camera buttons
    [SerializeField] private Button StartGame;
    [SerializeField] private Button Settings;
    [SerializeField] private Button QuitGame;
    // Quit Menu Buttons
    [SerializeField] private Button ExitGame;
    [SerializeField] private Button MainMenuFromQuit;
    // Player Selection Buttons
    [SerializeField] private Button Play;
    [SerializeField] private Button MainMenuFromSelection;
    // Settings Menu Buttons
    [SerializeField] private Button MainMenuFromSettings;
    // Add Players To Sceen
    [SerializeField] private Button Player1;
    [SerializeField] private Button Player2;
    [SerializeField] private Button Player3;
    [SerializeField] private Button Player4;
    [SerializeField] private GameObject Player1Red;
    [SerializeField] private GameObject Player2Blue;
    [SerializeField] private GameObject Player3Green;
    [SerializeField] private GameObject Player4Yellow;
    [SerializeField] private Transform Player1SpawnPoint;
    [SerializeField] private Transform Player2SpawnPoint;
    [SerializeField] private Transform Player3SpawnPoint;
    [SerializeField] private Transform Player4SpawnPoint;
    // for enable and desable 
    [SerializeField] private GameObject SettingsMenuTab;
    [SerializeField] private GameObject MainMenuTab;
    [SerializeField] private GameObject ExitMenuTab;
    [SerializeField] private GameObject PlayMenuTab;
    // Start is called before the first frame update
    public void Start()
    {
        MainmenuCamera.Priority = 1;

        if (ExitGame)
            ExitGame.onClick.AddListener(closeGame);

        if (Play)
            Play.onClick.AddListener(PlayScene);

        if (StartGame)
            StartGame.onClick.AddListener(PlayerSelect);

        if (Settings)
            Settings.onClick.AddListener(SettingsScreen);

        if (QuitGame)
            QuitGame.onClick.AddListener(QuitGameSceen);

        if (MainMenuFromQuit)
            MainMenuFromQuit.onClick.AddListener(ReturnToMenuFromQuit);

        if (MainMenuFromSelection)
            MainMenuFromSelection.onClick.AddListener(ReturnFromSelection);

        if (MainMenuFromSettings)
            MainMenuFromSettings.onClick.AddListener(ReturnFromSettings);

        if (Player1)
            Player1.onClick.AddListener(PlayerSpawn1);

        if (Player2)
            Player2.onClick.AddListener(PlayerSpawn2);

        if (Player3)
            Player3.onClick.AddListener(PlayerSpawn3);

        if (Player4)
            Player4.onClick.AddListener(PlayerSpawn4);
    }

    void PlayerSpawn4()
    {
        if (Player4Yellow != null)
        {
            Instantiate(Player4Yellow, Player4SpawnPoint);
        }
        return;

    }
    void PlayerSpawn3()
    {
        if (Player3Green != null)
        {
            Instantiate(Player3Green, Player3SpawnPoint);
        }
        return;

    }
    void PlayerSpawn2()
    {
        if (Player2Blue != null)
        {
            Instantiate(Player2Blue, Player2SpawnPoint);
        }
        return;

    }
    void PlayerSpawn1() 
    {
        if(Player1Red != null) 
        { 
           Instantiate(Player1Red, Player1SpawnPoint);
        }
        return;
    }
    void ReturnFromSettings() 
    { 
        if(SettingsCamera.Priority ==1) 
        { 
            SettingsCamera.Priority= 0;
            MainmenuCamera.Priority= 1;
            MainMenuTab.SetActive(true);
            SettingsMenuTab.SetActive(false);
        }
    }
    void ReturnFromSelection() 
    { 
        if(PlayerSelectionCamera.Priority ==1) 
        { 
            PlayerSelectionCamera.Priority= 0;
            MainmenuCamera.Priority= 1;
            MainMenuTab.SetActive(true);
            PlayMenuTab.SetActive(false);
        }
    }

    void ReturnToMenuFromQuit() 
    { 
        if(QuitGameCamera.Priority ==1) 
        { 
            QuitGameCamera.Priority= 0;
            MainmenuCamera.Priority= 1;
            MainMenuTab.SetActive(true);
            ExitMenuTab.SetActive(false);
        }
    }
    void QuitGameSceen() 
    { 
        if(MainmenuCamera.Priority ==1) 
        { 
            MainmenuCamera.Priority = 0;
            QuitGameCamera.Priority = 1;
            ExitMenuTab.SetActive(true);
            MainMenuTab.SetActive(false);
        }
    }
    void SettingsScreen() 
    { 
        if (MainmenuCamera.Priority == 1)
        {
            MainmenuCamera.Priority = 0;
            SettingsCamera.Priority = 1;
            MainMenuTab.SetActive(false);
            SettingsMenuTab.SetActive(true);
        }
    }
    void PlayerSelect()
    {
        if (MainmenuCamera.Priority == 1)
        {
            MainmenuCamera.Priority = 0;
            PlayerSelectionCamera.Priority = 1;
            MainMenuTab.SetActive(false);
            PlayMenuTab.SetActive(true);
        }
    }

    void PlayScene() 
    {
        SceneManager.LoadScene("WhiteBox");
    }
    void closeGame() 
    { 
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Aplication.Quit();
        #endif
    }
}
