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
    //Frame rate cap
    private const int maxFrameRate = 60;

    // Quick Time Events
    [SerializeField] private List<RandomEventBase> randomEventList;
    [SerializeField] private float minRandomTime = 1f;
    [SerializeField] private float maxRandomTime = 2f;
    [HideInInspector] public bool isEventActive = false;
    [HideInInspector] public RandomEventBase currentRandomEvent;
    // Event to trigger events
    public delegate void RandomEventHandler();
    public static event RandomEventHandler OnRandomEventTriggered;
    // List of random times to trigger the events
    private List<float> randomEventTimes = new List<float>();
    private float timeSinceStart = 0f;

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

    public string difficultyString;

    //bool for endgame -> please update code
    public bool iSEndGame = false;

    //bool for input manager;
    public bool hasInputManager = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }


        playerReadyDictionary = new Dictionary<ulong, bool>();
        playerPauseDictionary = new Dictionary<ulong, bool>();
    }

    private void Start()
    {
        Application.targetFrameRate = maxFrameRate;

        OnRandomEventTriggered += HandleRandomEvent;

        SetRandomEventTimes();

        // debug for random event times (to be deleted)
        Debug.LogWarning("Current difficulty " + currentDifficulty.difficultyString);
        for (int i = 0; i < randomEventTimes.Count; i++)
        {
            Debug.LogWarning("random Time" + i + " " + randomEventTimes[i]);
        }

        difficultySettings = new DifficultySettings();
        difficultyString = "Easy";
    }

    public override void OnDestroy()
    {
        OnRandomEventTriggered -= HandleRandomEvent;
    }

    private void InputManager_OnInteractEvent()
    {
        Debug.Log("Player Activated");
        if (gameState.Value == GameState.WaitingToStart && SceneManager.GetActiveScene().buildIndex == 2)
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
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
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
            TriggerRandomEvent();

            //Debug.Log("randomEventObject " + randomEventList[0].name); 
            //randomEventList[0].SetEventBool(true);
            //randomEventList[0].ActivateDeactivateEvent();

        }

        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            switch (gameState.Value)
            {
                case GameState.WaitingToStart:
                    if (!hasInputManager)
                    {
                        hasInputManager = true;

                        if (InputManager.Instance)
                        {
                            InputManager.Instance.PauseEvent += InputManager_PauseEvent;
                            InputManager.Instance.InteractEvent += InputManager_OnInteractEvent;
                        }
                    }

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

                    timeSinceStart += Time.deltaTime;

                    if (currentDifficulty != null)
                    {
                        // Adjust the difficulty based on time passed
                        foreach (float randomEventTime in randomEventTimes)
                        {
                            if (timeSinceStart > randomEventTime)
                            {
                                TriggerRandomEvent();
                            }
                        }
                    }
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
        }
        else if (gameState.Value != GameState.WaitingToStart) gameState.Value = GameState.WaitingToStart;


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
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
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
                difficultyString = "Easy";
                currentDifficulty = Difficulties[0];
                difficultySettings.SetDifficulty(currentDifficulty);
                break;

            case "Medium":
                difficultyString = "Medium";
                currentDifficulty = Difficulties[1];
                difficultySettings.SetDifficulty(currentDifficulty);
                break;

            case "Hard":
                difficultyString = "Hard";
                currentDifficulty = Difficulties[2];
                difficultySettings.SetDifficulty(currentDifficulty);
                break;

        }

    }

    // Quick Random Events

    private void SetRandomEventTimes()
    {
        if (currentDifficulty != null)
        {
            int numberOfRandomEvents = GetNumberOfRandomEvents(currentDifficulty.difficultyString);

            for (int i = 0; i < numberOfRandomEvents; i++)
            {
                float randomTime = CalculateRandomEventTime(currentDifficulty.difficultyString);
                randomEventTimes.Add(randomTime);
            }
        }
    }

    private int GetNumberOfRandomEvents(string difficulty)
    {
        switch (difficulty)
        {
            case "Easy":
                return 1;
            case "Medium":
                return 2;
            case "Hard":
                return 3;
            default:
                return 0;
        }
    }

    private float CalculateRandomEventTime(string difficulty)
    {
        float totalTime = gamePlayingTimerMax;
        float timeWindow = totalTime / 2f;

        float startTime = 0f;
        switch (difficulty)
        {
            case "Easy":
                startTime = UnityEngine.Random.Range(120f, 240f);  // bewtween 2 to 4 minutes
                break;
            case "Medium":
                if (randomEventTimes.Count == 0)
                {
                    // First random time around minute 2.5
                    startTime = UnityEngine.Random.Range(150f, 190);
                }
                else
                {
                    // Second random time around minute 4.5
                    startTime = UnityEngine.Random.Range(270f, 300f);
                }
                break;
            case "Hard":
                switch (randomEventTimes.Count)
                {
                    case 0:
                        // First random time around minute 1.5
                        startTime = UnityEngine.Random.Range(90f, 105f);
                        break;
                    case 1:
                        // Second random time around minute 3.5
                        startTime = UnityEngine.Random.Range(165f, 210f);
                        break;
                    case 2:
                        // Third random time around minute 5
                        startTime = UnityEngine.Random.Range(270f, 310f);
                        break;
                }
                break;
        }
        //switch (difficulty)
        //{
        //    case "Easy":
        //        startTime = UnityEngine.Random.Range(timeWindow * 0.9f, timeWindow * 1.3f);
        //        break;
        //    case "Medium":
        //        startTime = UnityEngine.Random.Range(timeWindow * 0.6f, timeWindow * 1.5f);
        //        break;
        //    case "Hard":
        //        startTime = UnityEngine.Random.Range(timeWindow * 0.5f, timeWindow * 1.7f);
        //        break;
        //}

        return startTime;
    }

    private void HandleRandomEvent()
    {
        ActivateRandomEvent();
    }

    private void TriggerRandomEvent()
    {
        OnRandomEventTriggered?.Invoke();
    }

    // Activate random Event after x amount of random time, will add the time variable after testing
    private void ActivateRandomEvent()
    {
        if (!isEventActive)
        {
            currentRandomEvent = GetRandomEvent();
            if (currentRandomEvent != null)
            {
                ActivateEvent(currentRandomEvent);
                AISupervisor.Instance.SupervisorMessageToDisplay(currentRandomEvent.GetRandomEvent().supervisorMessageOnEventTriggered);
            }
        }
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
        SetCurrentDifficultyTo(difficultyString);
        difficultySettings.SetAmountOfPlayers(numberOfPlayers); // setdifficulty based on amount of players & Updates difficulty

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


