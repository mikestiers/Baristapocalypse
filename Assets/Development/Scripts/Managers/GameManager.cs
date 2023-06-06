using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class GameManager : Singleton<GameManager>, PlayerInput.IPlayerActions
{
    //Input Events
    public Vector2 MovementValue { get; private set; }
    public event Action JumpEvent;
    public event Action InteractEvent;
    public event Action InteractAltEvent;
    public GameState gameState = GameState.RUNNING;

    //player movement input
    [HideInInspector] public Vector3 moveDir;
    [HideInInspector] public Vector3 curMoveInput;

    [HideInInspector]
    public PlayerInput playerInput;

    protected override void Awake()
    {
        base.Awake();
        playerInput = new PlayerInput();
        
    }

    private void Start()
    {
        //playerInput.Player.Pause.performed += ctx => Pause(ctx);
        playerInput.Player.SetCallbacks(this);// SetCallbacks calls the methods for us
        playerInput.Player.Enable();
    }

    private void Update()
    {
        
    }

    private void OnDestroy()
    {
        playerInput.Player.Disable();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed) { return; }

        InteractEvent?.Invoke();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed) { return; }

        JumpEvent?.Invoke();
    }

    public void OnMove(InputAction.CallbackContext context)
    {

        if (context.canceled)
        {
            moveDir = Vector3.zero;
            return;
        }

        Vector2 move = context.action.ReadValue<Vector2>();
        move.Normalize();

        moveDir = new Vector3(move.x, 0, move.y).normalized;


    }

    public void OnInteractAlt(InputAction.CallbackContext context)
    {
        if (context.performed) { return; }

        InteractAltEvent?.Invoke();
    }

    public void Pause(InputAction.CallbackContext context)
    {
        if (gameState == GameState.PAUSED)
        {
            UIManager.Instance.pauseMenu.SetActive(false);
            gameState = GameState.RUNNING;
        }
        else
        {
            UIManager.Instance.pauseMenu.SetActive(true);
            gameState = GameState.PAUSED;
        }
    }
}

public enum GameState
{
    RUNNING, PAUSED, LOST
}
