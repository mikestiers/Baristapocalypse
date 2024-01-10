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

    public GameObject player1Prefab;
    public GameObject player2Prefab;
    public GameObject player3Prefab;
    public GameObject player4Prefab;

    private void Awake()
    {
        Instance = this;
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
