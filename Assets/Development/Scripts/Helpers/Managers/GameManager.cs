using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    //Frame rate cap
    private const int maxFrameRate = 60;

    // Quick Time Events
    [SerializeField] private List<RandomEventBase> randomEventList;
    [SerializeField] public RandomEventEffects randomEventEffects;
    [SerializeField] private float minRandomTime = 1f;
    [SerializeField] private float maxRandomTime = 2f;
    [HideInInspector] public NetworkVariable<bool> isEventActive = new NetworkVariable<bool>(false);
    [HideInInspector] public NetworkVariable<bool> isGravityStorm = new NetworkVariable<bool>(false);
    [HideInInspector] public NetworkVariable<bool> isWifiEvent = new NetworkVariable<bool>(false);
    [HideInInspector] public RandomEventBase currentRandomEvent;
    [HideInInspector] public bool isEvaluationOn = false;
    private float evaluationTimer = 9.0f;
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
    [HideInInspector] public bool isLocalGamePaused = false;
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
    public DifficultySO currentDifficulty;

    public MoneySystem moneySystem;

    //bool for endgame -> please update code
    public bool iSEndGame = false;

    // Animation
    private readonly int BP_Barista_SufferHash = Animator.StringToHash("BP_Barista_Suffer");
    private const float CrossFadeDuration = 0.1f;

    [Header("Spills")]
    public bool canSpawnSpill = false;
    private int spills;
    private int spillLimit;
    private void Awake()
    {
        Instance = this;
 
        playerReadyDictionary = new Dictionary<ulong, bool>();
        playerPauseDictionary = new Dictionary<ulong, bool>();
        currentDifficulty = GameValueHolder.Instance.currentDifficulty;
    }


    private void Start()
    {
        Application.targetFrameRate = maxFrameRate;
        InitializePauseEventServerRpc();
        OnRandomEventTriggered += HandleRandomEvent;
        SetRandomEventTimes();

        // debug for random event times (to be deleted)
        Debug.LogWarning("Current difficulty " + currentDifficulty.difficultyString);
        for (int i = 0; i < randomEventTimes.Count; i++)
        {
            Debug.LogWarning("random Time"+ i + " " + randomEventTimes[i]);
        }

        SpillLimit(currentDifficulty.difficultyString);
    }

    public override void OnDestroy()
    {
        OnRandomEventTriggered -= HandleRandomEvent;
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
            PlayerController[] playerControllers = FindObjectsOfType<PlayerController>();
            for (int i = 0; i < playerControllers.Length; i++)
            {
                playerControllers[i].movementToggle = true;
            }
            gameState.Value = GameState.CountdownToStart;
        }
    }

    private void Update()
    {
        if (!IsServer) { return; }

        // Temporary for Testing Random Events
        if (Input.GetKeyDown(KeyCode.B))
        {
            TriggerRandomEvent();

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

                timeSinceStart += Time.deltaTime;


                if (currentDifficulty != null)
                {
                    // Adjust the difficulty based on time passed
                    foreach (float randomEventTime in randomEventTimes)
                    {
                        if (timeSinceStart > randomEventTime && timeSinceStart < (randomEventTime + evaluationTimer))
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
                CheckSpillAmount();
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

    public void SetLocalPlayerReady()
    {
        if (gameState.Value == GameState.WaitingToStart)
        {
            isLocalPlayerReady = true;
            OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
            SetPlayerReadyServerRpc();
        }
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

    public void GameOver()
    {
        Debug.LogWarning("Game Over......");
        PlayerController playerController = FindObjectOfType<PlayerController>();
        //playerController.anim.CrossFadeInFixedTime(BP_Barista_SufferHash, CrossFadeDuration);
        playerController.movementToggle = false;
        CustomerManager.Instance.BarClosing();
        iSEndGame = true;
    }

    public void TogglePauseGame()
    {
        ((InputSystemUIInputModule)EventSystem.current.currentInputModule).point.action.Enable();
        ((InputSystemUIInputModule)EventSystem.current.currentInputModule).leftClick.action.Enable();
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

    // Needs a better way of doing it
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
        return startTime;
    }
    public void SpillLimit(string difficulty)
    {
        switch (difficulty)
        {
            case "Easy":
                GetSpillLimit();
                break;
            case "Medium":
                GetSpillLimit();
                break;
            case "Hard":
                GetSpillLimit();
                break;
        }
    }

    private void GetSpillLimit()
    {
        spillLimit = currentDifficulty.maxSpillAmount;
         
        Debug.Log(message: spillLimit);
    }

    private void CheckSpillAmount()
    {
        if (spills >= spillLimit)
        {
            canSpawnSpill = false;
        }
        else
        {
            canSpawnSpill = true;
        }
    }

    public void AddSpill()
    {
        spills++;
    }

    public void RemoveSpill()
    {
        spills--;
    }
    private void HandleRandomEvent()
    {
        ActivateRandomEventClientRpc();
    }

    private void TriggerRandomEvent()
    {
        OnRandomEventTriggered?.Invoke();
    }

    [ClientRpc]
    private void ActivateRandomEventClientRpc()
    {
        ActivateRandomEvent();
    }

    // Activate random Event after x amount of random time, will add the time variable after testing
    private void ActivateRandomEvent()
    {
        if (!isEventActive.Value && !isEvaluationOn)
        {
            Debug.LogWarning("Triggering event");
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
        isEventActive.Value = true;
        if (randomEvent.GetComponent<GravityStorm>()) 
        {
            GravityStorm gravityStorm = randomEvent.GetComponent<GravityStorm>();
            
            isGravityStorm.Value = true;
            //randomEvent.GetComponent<GravityStorm>().GravityEventLights.SetActive(true);
            gravityStorm.SetEventBool(true);
            gravityStorm.ActivateDeactivateEvent();
            randomEventEffects.TurnOnOffEventEffectServerRpc(true);
            
        }
        else if (randomEvent.GetComponent<WifiStation>()) 
        {
            randomEvent.gameObject.GetComponent<WifiStation>().WifiEventIsStartingServerRpc();
            isWifiEvent.Value = true;
        }
        else if (randomEvent.GetComponent<RadioStation>()) 
        {
            randomEvent.gameObject.GetComponent<RadioStation>().EventOnClientRpc();
        }
    }

    public void DeactivateEvent(RandomEventBase randomEvent)
    {
        isEventActive.Value = false;
        if (randomEvent.GetComponent<GravityStorm>()) 
        {
            GravityStorm gravityStorm = randomEvent.GetComponent<GravityStorm>();
            isGravityStorm.Value = false;
            //gravityStorm.TurnOnOffEventEffectServerRpc(false);
            gravityStorm.SetEventBool(false);
            gravityStorm.ActivateDeactivateEvent();
            randomEventEffects.TurnOnOffEventEffectServerRpc(true);
        }
        else if (randomEvent.GetComponent<WifiStation>()) 
        {
            randomEvent.gameObject.GetComponent<WifiStation>().WifiEventIsDoneServerRpc();
            isWifiEvent.Value = false;
        }
        else if (randomEvent.GetComponent<RadioStation>()) 
        {
            randomEvent.gameObject.GetComponent<RadioStation>().EventOffServerRpc();
        }

    }

    [ClientRpc]
    public void UpdateClientRpc(int numberOfPlayers)
    {
        InitializeDifficultyMoney(numberOfPlayers);
    }

    public void InitializeDifficultyMoney(int numberOfPlayers)
    {
        GameValueHolder.Instance.difficultySettings.SetAmountOfPlayers(numberOfPlayers);
        moneySystem = new MoneySystem(GameValueHolder.Instance.difficultySettings.GetMoneyToPass());
    }


    // Server RPC to subcribe to pause event only one player was subscribing to it
    [ServerRpc(RequireOwnership = false)]
    private void InitializePauseEventServerRpc()
    {
        InitializePauseEventClientRpc();
    }

    [ClientRpc]
    private void InitializePauseEventClientRpc()
    {
        // There is a delay on initializing the Inputmanager on multiplayer due to connection timing and order of execution
        // Delay InitializeInputManager()
        Invoke("InitializePauseEvent", 1);
    }

    private void InitializePauseEvent()
    {
        InputManager.Instance.PauseEvent += InputManager_PauseEvent;
    }

}

public enum GameState
{
    WaitingToStart,
    CountdownToStart,
    GamePlaying,
    GameOver,
}


