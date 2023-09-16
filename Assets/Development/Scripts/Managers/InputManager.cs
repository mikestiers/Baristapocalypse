using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class InputManager : Singleton<InputManager>, ControllerInputs.IPlayerActions
{
    public Vector2 MovementValue { get; private set; }

    public event Action JumpEvent;
    public event Action InteractEvent;
    public event Action InteractAltEvent;

    public event Action DashEvent;
    public event Action GrabEvent;
    public event Action ThrowEvent;

    //player movement input
    [HideInInspector] public Vector3 moveDir;
    [HideInInspector] public Vector3 curMoveInput;

    [HideInInspector] public ControllerInputs playerInput;

    protected override void Awake()
    {
        base.Awake();
        playerInput = new ControllerInputs();
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

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (UIManager.Instance.gameOverMenu.activeSelf)
            {
                return;
            }
            bool isPaused = !UIManager.Instance.pauseMenu.activeSelf;
            UIManager.Instance.pauseMenu.SetActive(isPaused);

            Time.timeScale = isPaused ? 0f : 1f;
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
