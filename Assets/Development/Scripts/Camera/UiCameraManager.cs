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
    [SerializeField] private Button FullScreenButton;
    [SerializeField] private Button WindowModeButton;
    [SerializeField] private GameObject FullScreenGO;
    [SerializeField] private GameObject WindowModeGO;
    [SerializeField] private Resolution[] resolutions;
    [SerializeField] private Dropdown resoultionDropDown;
   
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
        // this is for screen resolutions
        resolutions = Screen.resolutions;
        resoultionDropDown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height +" : " + resolutions[i].refreshRate;
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height) 
            {
                currentResolutionIndex = i;
            }
        }
        resoultionDropDown.AddOptions(options);
        resoultionDropDown.value = currentResolutionIndex;
        resoultionDropDown.RefreshShownValue();


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

        if (FullScreenButton)
            FullScreenButton.onClick.AddListener(SetFullScreen);

        if (WindowModeButton)
            WindowModeButton.onClick.AddListener(SetWindowMode);
    }

    void ReturnFromSettings() 
    { 
        if(SettingsCamera.Priority ==1) 
        {
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.menuClicks);
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
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.menuClicks);
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
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.menuClicks);
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
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.menuClicks);
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
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.menuClicks);
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
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.menuClicks);
            MainmenuCamera.Priority = 0;
            PlayerSelectionCamera.Priority = 1;
            MainMenuTab.SetActive(false);
            PlayMenuTab.SetActive(true);
        }
    }

    void PlayScene() 
    {
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.menuClicks);
        SceneManager.LoadScene("LobbyScene");
        gameObject.SetActive(false);
    }
    void closeGame() 
    {
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.menuClicks);
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    void SetFullScreen()
    {
        FullScreenGO.SetActive(false);
        WindowModeGO.SetActive(true);
        Screen.fullScreen = true;     
    }

    void SetWindowMode() 
    {
        WindowModeGO.SetActive(false);
        FullScreenGO.SetActive(true);    
        Screen.fullScreen = false;       
    }

   
    public void SetREsolution(int resolutionIndex) 
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}
