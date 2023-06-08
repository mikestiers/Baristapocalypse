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
    public float timeRemaining = 10;
    public Timer timer;
    public GameObject menu;

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
        playerInput.Player.SetCallbacks(this);// SetCallbacks calls the methods for us
        playerInput.Player.Enable();
        timeRemaining -= Time.deltaTime;
        //timer.LoseEvent.AddListener(Lose);
        //timer.WinEvent.AddListener(Win);
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

    private void Lose()
    {
        menu.SetActive(true);
        Time.timeScale = 0f;
    }
    private void Win()
    {
        menu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed) { return; }

        InteractAltEvent?.Invoke();
    }

    public void OnGrab(InputAction.CallbackContext context)
    {
        
    }

    public void OnThrow(InputAction.CallbackContext context)
    {
        
    }
}
