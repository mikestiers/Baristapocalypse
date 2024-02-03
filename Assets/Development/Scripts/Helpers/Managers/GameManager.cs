using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System.Collections;

[DefaultExecutionOrder(-1)]
public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    // Quick Time Events
    [SerializeField] private List<RandomEventBase> randomEventList;
    [SerializeField] private float minRandomTime = 1f;
    [SerializeField] private float maxRandomTime = 2f;
    [HideInInspector] public bool isEventActive = false;
    [HideInInspector] public RandomEventBase currentRandomEvent;
    public event EventHandler OnPlayerDeactivateEvent;

    //Input Events
    public Vector2 MovementValue { get; private set; }

    // Game State vars
    private NetworkVariable<GameState> gameState = new NetworkVariable<GameState>(GameState.WaitingToStart);
    private NetworkVariable<float> countdownToStartTimer = new NetworkVariable<float>(3f);
    private NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(0f);
    [SerializeField] private float gamePlayingTimerMax = 360f;
    private bool isLocalPlayerReady;

    [SerializeField] private Transform player1Prefab;
    [SerializeField] public Transform[] playerSpawnPoints;

    // Pause Vars
    private bool isLocalGamePaused = false;
    private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);

    private Dictionary<ulong, bool> playerReadyDictionary;
    private Dictionary<ulong, bool> playerPauseDictionary;
    private bool autoTestGamePausedState;

    // UI Events
    public event EventHandler OnGameStateChanged;

    public event EventHandler OnLocalGamePaused;
    public event EventHandler OnLocalGameUnpaused;

    public event EventHandler OnMultiplayerGamePaused;
    public event EventHandler OnMultiplayerGameUnpaused;

    public event EventHandler OnLocalPlayerReadyChanged;

    [SerializeField] private CustomerManager customerManager;

    // Difficulty Settings

    [SerializeField] public DifficultySO[] Difficulties; //In Customer Manager for now move to Game Manager

    public DifficultySettings difficultySettings; //will move to GameManager when gamemanager is owki, change references to GameManager aswell

    public DifficultySO currentDifficulty;

    public MoneySystem moneySystem;

    //bool for endgame -> please update code
    public bool iSEndGame = false;

    private void Awake()
    {
        Instance = this;
 
        playerReadyDictionary = new Dictionary<ulong, bool>();
        playerPauseDictionary = new Dictionary<ulong, bool>();
    }

    private void Start()
    {
        if (InputManager.Instance)
        {
            InputManager.Instance.PauseEvent += InputManager_PauseEvent;
            InputManager.Instance.InteractEvent += InputManager_OnInteractEvent;
        }


    }

    private void InputManager_OnInteractEvent()
    {
        if (gameState.Value == GameState.WaitingToStart) 
        {
            //gameState = GameState.CountdownToStart;
            //OnGameStateChanged?.Invoke(this, EventArgs.Empty);
            isLocalPlayerReady = true;
            OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);

            SetPlayerReadyServerRpc();
        }
    }

    public override void OnNetworkSpawn()
    {
        gameState.OnValueChanged += GameState_OnValueChanged;
        isGamePaused.OnValueChanged += isGamePaused_OnValueChanged;

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        autoTestGamePausedState = true;
    }

    private void GameState_OnValueChanged(GameState previousValue, GameState newValue)
    {
        OnGameStateChanged?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;
        foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) 
        { 
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId] ) 
            { 
                // This Player Is not ready
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady) 
        {
            gameState.Value = GameState.CountdownToStart;
        }
    }

    private void Update()
    {
        if (!IsServer) { return; }

        // Temporary for Testing Random Events
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.LogWarning("ActivateRandomEvent " + randomEventList.Count);
            ActivateRandomEvent();

            //Debug.Log("randomEventObject " + randomEventList[0].name); 
            //randomEventList[0].SetEventBool(true);
            //randomEventList[0].ActivateDeactivateEvent();

        }

        switch (gameState.Value) 
        { 
            case GameState.WaitingToStart:
                break;

            case GameState.CountdownToStart:
                countdownToStartTimer.Value -= Time.deltaTime;
                if (countdownToStartTimer.Value < 0f)
                {
                    gameState.Value = GameState.GamePlaying;
                    gamePlayingTimer.Value = gamePlayingTimerMax;
                    CustomerManager test = Instantiate(customerManager);
                    test.GetComponent<NetworkObject>().Spawn(true);

                    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                    int numberOfPlayers = (players.Length - Mathf.FloorToInt(players.Length * 0.5f));


                    UpdateClientRpc(numberOfPlayers);
                }
                break;

            case GameState.GamePlaying:
                if (iSEndGame == true) gameState.Value = GameState.GameOver;

                /*
                gamePlayingTimer.Value -= Time.deltaTime;
                if (gamePlayingTimer.Value < 0f)
                {
                    gameState.Value = GameState.GameOver;
                    //OnGameStateChanged?.Invoke(this, EventArgs.Empty);
                }

                */

                break;

            case GameState.GameOver:
                break; 
        }

        //Debug.Log("autoTestGamePausedState" + autoTestGamePausedState);
    }

    private void LateUpdate()
    {
        if (autoTestGamePausedState)
        {
            //Debug.Log("autoTestGamePausedState" + autoTestGamePausedState);
            TestGamePauseState();
            autoTestGamePausedState = false;
        }
    }

    public bool IsGamePlaying()
    {
        return gameState.Value == GameState.GamePlaying;
    }

    public bool IsCountdownToStartActive() 
    {
        return gameState.Value == GameState.CountdownToStart;
    }

    public float GetCountdownToStartTimer()
    {
        return countdownToStartTimer.Value;
    }

    public bool IsGameOver()
    {
        return gameState.Value == GameState.GameOver;
    }

    public float GetGamePlayingTimer()
    {
        return gamePlayingTimer.Value;
    }

    public bool IsLocalPlayerReady()
    {
        return isLocalPlayerReady;
    }

    private void InputManager_PauseEvent(object sender, EventArgs e)
    {
        TogglePauseGame();
    }

    public void SetPlayButtonActive()
    {
        //playButton.SetActive(true);
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
            Transform playerTransform = Instantiate(player1Prefab, playerSpawnPoints[(int)clientId]);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }

    private void TogglePauseGame()
    {
        isLocalGamePaused = !isLocalGamePaused;
        if (isLocalGamePaused) 
        {
            PauseGameServerRpc();

            OnLocalGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            UnpauseGameServerRpc();

            OnLocalGameUnpaused?.Invoke(this, EventArgs.Empty);
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
    public void SetCurrentDifficultyTo(string difficulty)
    {
        switch (difficulty)
        {
            case "Easy":
                currentDifficulty = Difficulties[0];
                break;

            case "Medium":
                currentDifficulty = Difficulties[1];
                break;

            case "Hard":
                currentDifficulty = Difficulties[2];
                break;

        }

    }

    // Quick Random Events

    // Activate random Event after x amount of random time, will add the time variable after testing
    private void ActivateRandomEvent()
    {
        //while (true)
        //{
        //    yield return new WaitForSeconds(5f); // temp time for testing WaitForSeconds(UnityEngine.Random.Range(minRandomTime, maxRandomTime))

            if (!isEventActive)
            {
                currentRandomEvent = GetRandomEvent();
                if (currentRandomEvent != null)
                {
                    ActivateEvent(currentRandomEvent);
                }
            }
        //}
    }

    // Get a random Event from the Random Event List
    private RandomEventBase GetRandomEvent()
    {
        if (randomEventList.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, randomEventList.Count);
            Debug.Log("GetRandomEvent  " + randomIndex);
            return randomEventList[randomIndex];
        }

        return null;
    }

    // Simple probability of event happening, will change after
    private bool ShouldEventOccur(float chanceOfOccurrence)
    {
        return UnityEngine.Random.value <= chanceOfOccurrence;
    }

    public void ActivateEvent(RandomEventBase randomEvent)
    {
        isEventActive = true;
        randomEvent.SetEventBool(true);
        randomEvent.ActivateDeactivateEvent();
        //OnPlayerDeactivateEvent?.Invoke(this, EventArgs.Empty);
    }

    public void DeactivateEvent(RandomEventBase randomEvent)
    {
        isEventActive = true;
        randomEvent.SetEventBool(true);
        randomEvent.ActivateDeactivateEvent();
       
    }

    [ClientRpc]
    public void UpdateClientRpc(int numberOfPlayers)
    {
        InitializeDifficultyMoney(numberOfPlayers);
    }

    public void InitializeDifficultyMoney(int numberOfPlayers)
    {
        SetCurrentDifficultyTo(GameValueHolder.Instance.DifficultyString);

        difficultySettings = new DifficultySettings(currentDifficulty, numberOfPlayers);

        difficultySettings.SetAmountOfPlayers(numberOfPlayers); // setdifficulty based on amount of players

        moneySystem = new MoneySystem(difficultySettings.GetMoneyToPass());
    }

}

public enum GameState
{
    WaitingToStart,
    CountdownToStart,
    GamePlaying,
    GameOver,
}


