using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static Unity.Burst.Intrinsics.Arm;

[DefaultExecutionOrder(-1)]
public class GameManager : Singleton<GameManager>
{
    // Testing
    [SerializeField] private GameObject play;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject playButton;
    
    //Input Events
    public Vector2 MovementValue { get; private set; }
    
    public GameState gameState = GameState.RUNNING;

    public GameObject player1Prefab;
    public GameObject player2Prefab;
    public GameObject player3Prefab;
    public GameObject player4Prefab;

    //spawnpoints for network
    public int spawnpoint = 0;

    //spawn players if button was pressed in main menu
    public bool player1Active = false;
    public bool player2Active = false;
    public bool player3Active = false;
    public bool player4Active = false;

    //player movement input
    [HideInInspector] public Vector3 moveDir;
    [HideInInspector] public Vector3 curMoveInput;

    [HideInInspector]
    //public PlayerInput playerInput;

    //DifficultySettings
    public int cusFrequency = 1; //customer frequency
    public int minCustomers = 1;
    public int maxCustomers = 2;
    public int cusPatience = 0; //customer patience

    public delegate void PlayerSpawnHandler(bool player1Active, bool player2Active, bool player3Active, bool player4Active);

    public static event PlayerSpawnHandler OnPlayersSpawned;

    public void SpawnPlayers(bool player1Active, bool player2Active, bool player3Active, bool player4Active)
    {
        if (OnPlayersSpawned != null)
        {
            OnPlayersSpawned(player1Active,player2Active,player3Active,player4Active);
        }
    }

    private void OnSceneChanged(Scene currentScene, Scene nextScene)
    {
        if (nextScene.name == "TestScene")
        {
            SpawnPlayers(player1Active, player2Active, player3Active, player4Active);
        }
    }

    private void Start()
    {
        //playerInput = new PlayerInput();
        //playerInput.Player.Enable();

    }

    private void OnDestroy()
    {
        //playerInput.Player.Disable();
    }

    public void SetPlayButtonActive()
    {
        playButton.SetActive(true);
    }

    public void SetEasyDifficulty()
    {
        //simple difficulty features
        Debug.Log("Easy");
    }

    public void SetMediumDifficulty()
    {
        //Medium difficulty featuers
        Debug.Log("Medium");
    }

    public void SetHardDifficulty()
    {
        //Hard difficulty features
        Debug.Log("Hard");
    }

}

public enum GameState
{
    RUNNING, PAUSED, GAMEOVER

}
