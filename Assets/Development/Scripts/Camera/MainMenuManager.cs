using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{
    [Header("Cameras")]
    public CinemachineVirtualCamera MainmenuCamera;
    public CinemachineVirtualCamera SettingsCamera;
    public CinemachineVirtualCamera PlayerSelectionCamera;
    public CinemachineVirtualCamera QuitGameCamera;

    [Header("Main Menu")]
    private Button[] MainMenuButtons;
    [SerializeField] private Button StartGame;
    [SerializeField] private Button Settings;
    [SerializeField] private Button QuitGame;

    [Header("Quit Menu")]
    private Button[] QuitMenuButtons;
    [SerializeField] private Button ExitGame;
    [SerializeField] private Button MainMenuFromQuit;

    [Header("Player Menu")]
    private Button[] PlayerSelectionButtons;
    [SerializeField] private Button singlePlayerButton;
    [SerializeField] private Button multiplayerButton;
    [SerializeField] private Button MainMenuFromSelection;

    [Header("Settings Menu")]
    private Button[] SettingsMenuButtons;
    private Slider[] SettingsMenuSliders;
    [SerializeField] private Button MainMenuFromSettings;
    [SerializeField] private Button FullScreenButton;
    [SerializeField] private Button WindowModeButton;
    [SerializeField] private GameObject FullScreenGO;
    [SerializeField] private GameObject WindowModeGO;
    [SerializeField] private Resolution[] resolutions;
    [SerializeField] private Dropdown resoultionDropDown;

    [Header("Difficulty Modes")]
    [SerializeField] private Button EasyButton;
    [SerializeField] private Button MediumButton;
    [SerializeField] private Button HardButton;

    [Header("Menu Tabs")]
    [SerializeField] private GameObject SettingsMenuTab;
    [SerializeField] private GameObject MainMenuTab;
    [SerializeField] private GameObject ExitMenuTab;
    [SerializeField] private GameObject PlayMenuTab;

    public void Start()
    {
        MainmenuCamera.Priority = 1;

        if (ExitGame)
            ExitGame.onClick.AddListener(closeGame);

        if (singlePlayerButton)
            singlePlayerButton.onClick.AddListener(PlayScene_SinglePlayer);

        if (multiplayerButton)
            multiplayerButton.onClick.AddListener(LobbyScene);

        if (StartGame)
            StartGame.onClick.AddListener(PlayerSelect);

        if (Settings)
            Settings.onClick.AddListener(SettingsScreen);

        if (MainMenuFromSelection)
            MainMenuFromSelection.onClick.AddListener(ReturnFromSelection);

        if (MainMenuFromSettings)
            MainMenuFromSettings.onClick.AddListener(ReturnFromSettings);

        if (FullScreenButton)
            FullScreenButton.onClick.AddListener(SetFullScreen);

        if (WindowModeButton)
            WindowModeButton.onClick.AddListener(SetWindowMode);

        if (EasyButton)
            EasyButton.onClick.AddListener(() => SetDifficulty("Easy"));

        if (MediumButton)
            MediumButton.onClick.AddListener(() => SetDifficulty("Medium"));

        if (HardButton)
            HardButton.onClick.AddListener(() => SetDifficulty("Hard"));

        MainMenuButtons = MainmenuCamera.GetComponentsInChildren<Button>();
        SettingsMenuButtons = SettingsMenuTab.GetComponentsInChildren<Button>();
        SettingsMenuSliders = SettingsMenuTab.GetComponentsInChildren<Slider>();
        PlayerSelectionButtons = PlayMenuTab.GetComponentsInChildren<Button>();

        SetInteractableButtons(MainMenuButtons, true);
        SetInteractableButtons(SettingsMenuButtons, false);
        SetInteractableSliders(SettingsMenuSliders, false);
        SetInteractableButtons(PlayerSelectionButtons, false);
    }

    void ReturnFromSettings() 
    { 
        if(SettingsCamera.Priority ==1) 
        {
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.menuClicks);
            SettingsCamera.Priority= 0;
            MainmenuCamera.Priority= 1;
            EventSystem.current.SetSelectedGameObject(StartGame.gameObject);
            SetInteractableButtons(MainMenuButtons, true);
            SetInteractableButtons(SettingsMenuButtons, false);
            SetInteractableSliders(SettingsMenuSliders, false);
            SetInteractableButtons(PlayerSelectionButtons, false);
        }
    }
    void ReturnFromSelection() 
    { 
        if(PlayerSelectionCamera.Priority ==1) 
        {
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.menuClicks);
            PlayerSelectionCamera.Priority= 0;
            MainmenuCamera.Priority= 1;
            EventSystem.current.SetSelectedGameObject(StartGame.gameObject);
            SetInteractableButtons(MainMenuButtons, true);
            SetInteractableButtons(SettingsMenuButtons, false);
            SetInteractableSliders(SettingsMenuSliders, false);
            SetInteractableButtons(PlayerSelectionButtons, false);
        }
    }

    void SettingsScreen() 
    { 
        if (MainmenuCamera.Priority == 1)
        {
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.menuClicks);
            MainmenuCamera.Priority = 0;
            SettingsCamera.Priority = 1;
            EventSystem.current.SetSelectedGameObject(FullScreenGO.gameObject);
            SetInteractableButtons(MainMenuButtons, false);
            SetInteractableButtons(SettingsMenuButtons, true);
            SetInteractableSliders(SettingsMenuSliders, true);
            SetInteractableButtons(PlayerSelectionButtons, false);
        }
    }
    void PlayerSelect()
    {
        if (MainmenuCamera.Priority == 1)
        {
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.menuClicks);
            MainmenuCamera.Priority = 0;
            PlayerSelectionCamera.Priority = 1;
            EventSystem.current.SetSelectedGameObject(singlePlayerButton.gameObject);
            SetInteractableButtons(MainMenuButtons, false);
            SetInteractableButtons(SettingsMenuButtons, false);
            SetInteractableSliders(SettingsMenuSliders, false);
            SetInteractableButtons(PlayerSelectionButtons, true);
        }
    }

    void LobbyScene() 
    {
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.menuClicks);
        BaristapocalypseMultiplayer.playMultiplayer = true;
        SceneManager.LoadScene("LobbyScene");
        gameObject.SetActive(false);
    }

    void PlayScene_SinglePlayer()
    {
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.menuClicks);
        BaristapocalypseMultiplayer.playMultiplayer = false;
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

    public void SetDifficulty(string Difficulty)
    {
        GameValueHolder.Instance.DifficultyString = Difficulty;
    }

    private void SetInteractableButtons(Button[] buttons, bool interactable)
    {
        foreach (Button button in buttons)
        {
            button.interactable = interactable;
        }
    }

    private void SetInteractableSliders(Slider[] sliders, bool interactable)
    {
        foreach (Slider slider in sliders)
        {
            slider.interactable = interactable;
        }
    }
}
