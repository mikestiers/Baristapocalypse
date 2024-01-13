using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using static Unity.Burst.Intrinsics.Arm;

[DefaultExecutionOrder(-1)]
public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    // Testing
    [SerializeField] private GameObject play;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject playButton;
    
    //Input Events
    public Vector2 MovementValue { get; private set; }

    public GameState gameState = GameState.RUNNING;

    [SerializeField] private Transform player1Prefab;
    public GameObject player2Prefab;
    public GameObject player3Prefab;
    public GameObject player4Prefab;

    // Pause
    private bool isLocalGamePaused = false;
    private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);
    private Dictionary<ulong, bool> playerPauseDictionary;
    public event EventHandler OnMultiplayerGamePaused;
    public event EventHandler OnMultiplayerGameUnpaused;

    private void Awake()
    {
        Instance = this;
        playerPauseDictionary = new Dictionary<ulong, bool>();
    }

    private void Start()
    {
        InputManager.Instance.PauseEvent += InputManager_PauseEvent;

    }

    private void InputManager_PauseEvent(object sender, EventArgs e)
    {
        TogglePauseGame();
    }

    public void SetPlayButtonActive()
    {
        playButton.SetActive(true);
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
        isGamePaused.OnValueChanged += isGamePaused_OnValueChanged;
    }

    private void isGamePaused_OnValueChanged(bool previousValue, bool newValue)
    {
        if (isGamePaused.Value) 
        {
            Time.timeScale = 0f;
            OnMultiplayerGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1f;
            OnMultiplayerGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Transform playerTransform = Instantiate(player1Prefab);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }

    private void TogglePauseGame()
    {
        isLocalGamePaused = !isLocalGamePaused;
        if (isLocalGamePaused) 
        {
            PauseGameServerRpc();
            
            UIManager.Instance.pauseMenu.SetActive(true); // UI pause menu
        }
        else
        {
            UnpauseGameServerRpc();
            UIManager.Instance.pauseMenu.SetActive(false); // UI pause menu
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerPauseDictionary[serverRpcParams.Receive.SenderClientId] = true;

        TestGamePauseState();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UnpauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerPauseDictionary[serverRpcParams.Receive.SenderClientId] = false;

        TestGamePauseState();
    }

    private void TestGamePauseState()
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (playerPauseDictionary.ContainsKey(clientId) && playerPauseDictionary[clientId])
            {
                // This PLayer is paused
                isGamePaused.Value = true;
                return;
            }
        }

        // All Players are unpaused
        isGamePaused.Value = false;

    }

}

public enum GameState
{
    RUNNING, PAUSED, GAMEOVER

}
