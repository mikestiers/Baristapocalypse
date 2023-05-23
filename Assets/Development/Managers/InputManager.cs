using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<GameManager>, PlayerInput.IPlayerActions
{

    public Vector2 MovementValue {  get; private set; } //

    public event Action JumpEvent;
    public event Action InteractEvent;
    public event Action ThrowEvent;

    //player movement input
    [HideInInspector] public Vector3 movInput;
    [HideInInspector] public Vector3 playerMovement;
    public float playerMoveSpeed;
    


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
        if (playerMoveSpeed == 0f)
            playerMoveSpeed = 10f;
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
        if(context.performed) { return; }

        JumpEvent?.Invoke();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
      
        MovementValue = context.ReadValue<Vector2>();
        MovementValue.Normalize();

        movInput = new Vector3(MovementValue.x, 0f, MovementValue.y).normalized;

        
        playerMovement = movInput * playerMoveSpeed;


    }

    public void OnThrow(InputAction.CallbackContext context)
    {
        if (context.performed) { return; }

        ThrowEvent?.Invoke();
    }
}
