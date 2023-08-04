using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    //Input Events
    public Vector2 MovementValue { get; private set; }
    
    public GameState gameState = GameState.RUNNING;

    //spawn players if button was pressed in main menu
    public bool player1Active = false;
    public bool player2Active = false;
    public bool player3Active = false;
    public bool player4Active = false;

    // check if we have called the funtion to spawn all players
    private bool hasSpawnedAllPlayers = false;

    //player movement input
    [HideInInspector] public Vector3 moveDir;
    [HideInInspector] public Vector3 curMoveInput;

    [HideInInspector]
    public PlayerInput playerInput;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        playerInput = new PlayerInput();
        playerInput.Player.Enable();
        
    }

    private void Update()
    {

        if(SceneManager.GetActiveScene() == SceneManager.GetSceneByName("WhiteBox"))
        {
            if (hasSpawnedAllPlayers == true)
            {
                return;
            }
            else
            {
                if (player1Active == true)
                {
                    GameObject.FindGameObjectWithTag("Player").gameObject.SetActive(true);
                }
                if (player2Active == true)
                {
                    GameObject.FindGameObjectWithTag("Player2").gameObject.SetActive(true);
                }
                if (player3Active == true)
                {
                    GameObject.FindGameObjectWithTag("Player3").gameObject.SetActive(true);
                }
                if (player4Active == true)
                {
                    GameObject.FindGameObjectWithTag("Player4").gameObject.SetActive(true);
                }
                hasSpawnedAllPlayers = true;
            }
        }
    }

    private void OnDestroy()
    {
        playerInput.Player.Disable();
    }
}

public enum GameState
{
    RUNNING, PAUSED, GAMEOVER

}
