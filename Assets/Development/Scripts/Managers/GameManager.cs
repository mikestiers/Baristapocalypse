using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    //Input Events
    public Vector2 MovementValue { get; private set; }
    
    public GameState gameState = GameState.RUNNING;

    //player movement input
    [HideInInspector] public Vector3 moveDir;
    [HideInInspector] public Vector3 curMoveInput;

    [HideInInspector]
    public PlayerInput playerInput;

    private void Awake()
    {
        
        playerInput = new PlayerInput();

    }

    private void Start()
    {

    }

    private void Update()
    {

    }

    private void OnDestroy()
    {

    }
}

public enum GameState
{
    RUNNING, PAUSED, LOST

}
