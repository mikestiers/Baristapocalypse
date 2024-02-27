using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MainMenuManager : MonoBehaviour
{
    [Header("Cameras")]
    public CinemachineVirtualCamera MainmenuCamera;
    public CinemachineVirtualCamera SettingsCamera;
    public CinemachineVirtualCamera PlayerSelectionCamera;
    public CinemachineVirtualCamera CreditsCamera;
    [SerializeField] private Animator leftDoorAnimator;
    [SerializeField] private Animator rightDoorAnimator;

    [Header("Main Menu")]
    public Button StartGame;
    public Button Settings;
    public Button Credits;
    public Button QuitGame;
    private Button[] MainMenuButtons;

    [Header("Player Menu")]
    public Button singlePlayerButton;
    public Button multiplayerButton;
    public Button EasyButton;
    public Button MediumButton;
    public Button HardButton;
    public Button MainMenuFromSelection;
    private Button[] PlayerSelectionButtons;

    [Header("Settings Menu")]
    public Button MainMenuFromSettings;
    public Button FullScreenButton;
    public Button WindowModeButton;
    public GameObject FullScreenGO;
    public GameObject WindowModeGO;
    private Button[] SettingsMenuButtons;
    private Slider[] SettingsMenuSliders;

    [Header("Credits Menu")]
    public Button MainMenuFromCredits;
    public TMPro.TextMeshProUGUI creditsText;
    public TextScroller creditsTextScroller;
    private Button[] CreditsMenuButtons;

    [Header("Menu Tabs")]
    public GameObject SettingsMenuTab;
    public GameObject MainMenuTab;
    public GameObject PlayMenuTab;
    public GameObject CreditsMenuTab;

    [Header("Button Sounds")]
    public AudioClip buttonSwitchedSound;
    public AudioClip backButtonPressedSound;
    public AudioClip nextButtonPressedSound;

    [Header("Level Loader")]
    [SerializeField] private LevelLoader levelLoader;

    private void Awake()
    {
        SceneHelper.Instance.ResetGame();

        if (SceneHelper.Instance.isRestartGame)
        {
            if (!BaristapocalypseMultiplayer.playMultiplayer)
            {
                SceneHelper.Instance.isRestartGame = false;
                PlayScene_SinglePlayer();
            }
            else
            {
                SceneHelper.Instance.isRestartGame = false;
                LobbyScene();
            }
        }
    }

    public void Start()
    {
        Time.timeScale = 1f;
        MainmenuCamera.Priority = 1;

        if (QuitGame)
            QuitGame.onClick.AddListener(closeGame);

        if (singlePlayerButton)
            singlePlayerButton.onClick.AddListener(PlayScene_SinglePlayer);

        if (multiplayerButton)
            multiplayerButton.onClick.AddListener(LobbyScene);

        if (StartGame)
            StartGame.onClick.AddListener(PlayerSelect);

        if (Settings)
            Settings.onClick.AddListener(SettingsScreen);

        if (Credits)
            Credits.onClick.AddListener(CreditsScreen);

        if (MainMenuFromCredits)
            MainMenuFromCredits.onClick.AddListener(ReturnFromCredits);

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

        MainMenuButtons = MainMenuTab.GetComponentsInChildren<Button>();
        SettingsMenuButtons = SettingsMenuTab.GetComponentsInChildren<Button>();
        SettingsMenuSliders = SettingsMenuTab.GetComponentsInChildren<Slider>();
        PlayerSelectionButtons = PlayMenuTab.GetComponentsInChildren<Button>();
        CreditsMenuButtons = CreditsMenuTab.GetComponentsInChildren<Button>();

        SetInteractableButtons(MainMenuButtons, true);
        SetInteractableButtons(SettingsMenuButtons, false);
        SetInteractableSliders(SettingsMenuSliders, false);
        SetInteractableButtons(PlayerSelectionButtons, false);
        SetInteractableButtons(CreditsMenuButtons, false);

        creditsTextScroller.enabled = false;
    }

    void ReturnFromSettings() 
    { 
        if(SettingsCamera.Priority ==1) 
        {
            SoundManager.Instance.PlayOneShot(backButtonPressedSound);
            SettingsCamera.Priority= 0;
            MainmenuCamera.Priority= 1;
            EventSystem.current.SetSelectedGameObject(StartGame.gameObject);
            SetInteractableButtons(MainMenuButtons, true);
            SetInteractableButtons(SettingsMenuButtons, false);
            SetInteractableSliders(SettingsMenuSliders, false);
            SetInteractableButtons(PlayerSelectionButtons, false);
            SetInteractableButtons(CreditsMenuButtons, false);
        }
    }
    void ReturnFromSelection() 
    { 
        if(PlayerSelectionCamera.Priority == 1) 
        {
            SoundManager.Instance.PlayOneShot(backButtonPressedSound);
            leftDoorAnimator.SetBool("isOpen", false);
            rightDoorAnimator.SetBool("isOpen", false);
            PlayerSelectionCamera.Priority= 0;
            MainmenuCamera.Priority= 1;
            EventSystem.current.SetSelectedGameObject(StartGame.gameObject);
            SetInteractableButtons(MainMenuButtons, true);
            SetInteractableButtons(SettingsMenuButtons, false);
            SetInteractableSliders(SettingsMenuSliders, false);
            SetInteractableButtons(PlayerSelectionButtons, false);
            SetInteractableButtons(CreditsMenuButtons, false);
        }
    }

    void ReturnFromCredits()
    {
        if (CreditsCamera.Priority == 1)
        {
            SoundManager.Instance.PlayOneShot(backButtonPressedSound);
            CreditsCamera.Priority = 0;
            MainmenuCamera.Priority = 1;
            EventSystem.current.SetSelectedGameObject(StartGame.gameObject);
            creditsTextScroller.enabled = false;
            SetInteractableButtons(MainMenuButtons, true);
            SetInteractableButtons(SettingsMenuButtons, false);
            SetInteractableSliders(SettingsMenuSliders, false);
            SetInteractableButtons(PlayerSelectionButtons, false);
            SetInteractableButtons(CreditsMenuButtons, false);
        }
    }


    void SettingsScreen() 
    { 
        if (MainmenuCamera.Priority == 1)
        {
            SoundManager.Instance.PlayOneShot(nextButtonPressedSound);
            MainmenuCamera.Priority = 0;
            SettingsCamera.Priority = 1;
            EventSystem.current.SetSelectedGameObject(FullScreenGO.gameObject);
            SetInteractableButtons(MainMenuButtons, false);
            SetInteractableButtons(SettingsMenuButtons, true);
            SetInteractableSliders(SettingsMenuSliders, true);
            SetInteractableButtons(PlayerSelectionButtons, false);
            SetInteractableButtons(CreditsMenuButtons, false);
        }
    }

    void CreditsScreen()
    {
        if (MainmenuCamera.Priority == 1)
        {
            SoundManager.Instance.PlayOneShot(nextButtonPressedSound);
            MainmenuCamera.Priority = 0;
            CreditsCamera.Priority = 1;
            EventSystem.current.SetSelectedGameObject(FullScreenGO.gameObject);
            SetInteractableButtons(MainMenuButtons, false);
            SetInteractableButtons(SettingsMenuButtons, false);
            SetInteractableSliders(SettingsMenuSliders, false);
            SetInteractableButtons(PlayerSelectionButtons, false);
            SetInteractableButtons(CreditsMenuButtons, true);
            creditsTextScroller.enabled = true;
        }
    }

    void PlayerSelect()
    {
        if (MainmenuCamera.Priority == 1)
        {
            SoundManager.Instance.PlayOneShot(nextButtonPressedSound);
            leftDoorAnimator.SetBool("isOpen", true);
            rightDoorAnimator.SetBool("isOpen", true);
            MainmenuCamera.Priority = 0;
            PlayerSelectionCamera.Priority = 1;
            EventSystem.current.SetSelectedGameObject(singlePlayerButton.gameObject);
            SetInteractableButtons(MainMenuButtons, false);
            SetInteractableButtons(SettingsMenuButtons, false);
            SetInteractableSliders(SettingsMenuSliders, false);
            SetInteractableButtons(PlayerSelectionButtons, true);
            SetInteractableButtons(CreditsMenuButtons, false);
        }
    }

    void LobbyScene() 
    {
        SoundManager.Instance.PlayOneShot(nextButtonPressedSound);
        BaristapocalypseMultiplayer.playMultiplayer = true;
        if (!levelLoader.isActiveAndEnabled)
        {
            levelLoader.gameObject.SetActive(true);
        }
        levelLoader.PlaySceneTransition();
        gameObject.SetActive(false);
    }

    void PlayScene_SinglePlayer()
    {
        SoundManager.Instance.PlayOneShot(nextButtonPressedSound);
        BaristapocalypseMultiplayer.playMultiplayer = false;
        if (!levelLoader.isActiveAndEnabled)
        {
            levelLoader.gameObject.SetActive(true);
        }
        levelLoader.PlaySceneTransition();
        gameObject.SetActive(false);
    }

    void closeGame() 
    {
        SoundManager.Instance.PlayOneShot(nextButtonPressedSound);
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

    public void SetDifficulty(string Difficulty)
    {
        GameValueHolder.Instance.SetCurrentDifficultyTo(Difficulty);
    }

    private void SetInteractableButtons(Button[] buttons, bool interactable)
    {
        foreach (Button button in buttons)
        {
            button.interactable = interactable;
            Debug.Log($"Button {button.name} is interactable: {interactable}");
        }
    }

    private void SetInteractableSliders(Slider[] sliders, bool interactable)
    {
        foreach (Slider slider in sliders)
        {
            slider.interactable = interactable;
        }
    }

    private void Update()
    {
        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            SoundManager.Instance.PlayOneShot(buttonSwitchedSound);
        }
        else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            SoundManager.Instance.PlayOneShot(buttonSwitchedSound);
        }
        else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            SoundManager.Instance.PlayOneShot(buttonSwitchedSound);
        }
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            SoundManager.Instance.PlayOneShot(buttonSwitchedSound);
        }

        if (Gamepad.current != null)
        {
            if (Gamepad.current.dpad.up.wasPressedThisFrame)
            {
                SoundManager.Instance.PlayOneShot(buttonSwitchedSound);
            }
            else if (Gamepad.current.dpad.down.wasPressedThisFrame)
            {
                SoundManager.Instance.PlayOneShot(buttonSwitchedSound);
            }
            else if (Gamepad.current.dpad.left.wasPressedThisFrame)
            {
                SoundManager.Instance.PlayOneShot(buttonSwitchedSound);
            }
            else if (Gamepad.current.dpad.right.wasPressedThisFrame)
            {
                SoundManager.Instance.PlayOneShot(buttonSwitchedSound);
            }
        }
    }
}
