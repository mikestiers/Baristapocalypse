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

    public GameObject prefabPlayer1;
    public GameObject prefabPlayer2;
    public GameObject prefabPlayer3;
    public GameObject prefabPlayer4;

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
                    GameObject.FindGameObjectWithTag("LevelSpawnpoint").gameObject.SetActive(true);
                    GameObject player1 = Instantiate(prefabPlayer1, GameObject.FindGameObjectWithTag("LevelSpawnpoint").GetComponent<LevelSpawnpoints>().spawnpointPlayer1);
                }
                if (player2Active == true)
                {
                    GameObject.FindGameObjectWithTag("LevelSpawnpoint").gameObject.SetActive(true);
                    GameObject player2 = Instantiate(prefabPlayer2, GameObject.FindGameObjectWithTag("LevelSpawnpoint").GetComponent<LevelSpawnpoints>().spawnpointPlayer2);
                }
                if (player3Active == true)
                {
                    GameObject.FindGameObjectWithTag("LevelSpawnpoint").gameObject.SetActive(true);
                    GameObject player3 = Instantiate(prefabPlayer3, GameObject.FindGameObjectWithTag("LevelSpawnpoint").GetComponent<LevelSpawnpoints>().spawnpointPlayer3);
                }
                if (player4Active == true)
                {
                    GameObject.FindGameObjectWithTag("LevelSpawnpoint").gameObject.SetActive(true);
                    GameObject player4 = Instantiate(prefabPlayer4, GameObject.FindGameObjectWithTag("LevelSpawnpoint").GetComponent<LevelSpawnpoints>().spawnpointPlayer4);
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
