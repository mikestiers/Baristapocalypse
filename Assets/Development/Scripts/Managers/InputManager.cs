using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<GameManager>, PlayerInput.IPlayerActions
{

    public Vector2 MovementValue { get; private set; } //

    public event Action JumpEvent;
    public event Action InteractEvent;
    public event Action InteractAltEvent;

    public event Action DashEvent;
    public event Action GrabEvent;
    public event Action ThrowEvent;

    public GameState gameState = GameState.RUNNING;


    //player movement input
    [HideInInspector] public Vector3 moveDir;
    [HideInInspector] public Vector3 curMoveInput;


    [HideInInspector] public bool playerIsmoving;

    private PlayerInput playerInput;

    protected override void Awake()
    {
        base.Awake();
        playerInput = new PlayerInput();
    }

    private void Start()
    {
        playerInput.Player.SetCallbacks(this);// SetCallbacks calls the methods for us
        playerInput.Player.Enable();
       
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
        if(context.performed)
            JumpEvent?.Invoke();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
 
        if (context.canceled)
        {
            moveDir = Vector3.zero;
            playerIsmoving = false;
            return;
        }

        Vector2 move = context.action.ReadValue<Vector2>();
        move.Normalize();

        moveDir = new Vector3(move.x, 0, move.y).normalized;
        playerIsmoving = true;


    }

    public void OnInteractAlt(InputAction.CallbackContext context)
    {
        if (context.performed) { return; }

        InteractAltEvent?.Invoke();
    }

    public void OnPause(InputAction.CallbackContext context)
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

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed)
            DashEvent?.Invoke();
    }

    public void OnGrab(InputAction.CallbackContext context)
    {
        if (context.performed)
            GrabEvent?.Invoke();
    }

    public void OnThrow(InputAction.CallbackContext context)
    {
        if (context.performed)
            ThrowEvent?.Invoke();
    }
        

}
