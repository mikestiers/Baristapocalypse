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
    public event Action InteractAltEvent;

    //player movement input
    [HideInInspector] public Vector3 moveDir;
    [HideInInspector] public Vector3 curMoveInput;
    
    


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
        if(context.performed) { return; }

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
}
