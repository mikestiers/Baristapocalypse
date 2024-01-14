using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class InputManager : MonoBehaviour, ControllerInputs.IPlayerActions
{
    public static InputManager Instance { get; private set; }

    public Vector2 MovementValue { get; private set; }

    public event Action JumpEvent;
    public event Action InteractEvent;
    public event Action InteractAltEvent;

    public event Action DashEvent;
    public event Action GrabEvent;
    public event Action ThrowEvent;
    public event EventHandler PauseEvent;

    public event Action DebugConsoleEvent;

    //player movement input
    [HideInInspector] public Vector3 moveDir;
    [HideInInspector] public Vector3 curMoveInput;

    // controller dead zone sensitivity
    private float deadZone = 0.5f;

    [HideInInspector] public ControllerInputs controllerInputs;

    // Player Configuration
    [SerializeField] private MeshRenderer playerMesh;
    private PlayerConfiguration playerConfig;

    // Pause Variables

    private void Awake()
    {
        Instance = this;
        controllerInputs = new ControllerInputs();
        //playerPauseDictionary = new Dictionary<ulong, bool>(); playerPauseDictionary = new Dictionary<ulong, bool>();


    }

    private void Start()
    {
        controllerInputs.Player.SetCallbacks(this);// SetCallbacks calls the methods for us
        controllerInputs.Player.Enable();
    }

    private void OnDestroy()
    {
        controllerInputs.Player.Disable();
    }

    //public void InitializePlayer(PlayerConfiguration pc)
    //{
    //    playerConfig = pc;
    //    playerMesh.material = pc.PlayerMaterial;
    //    playerConfig.Input.onActionTriggered += Input_onActionTriggered;
    //}

    //private void Input_onActionTriggered(InputAction.CallbackContext obj)
    //{
    //    // Move
    //    if (obj.action.name == controllerInputs.Player.Move.name)
    //    {
    //        OnMove(obj);
    //    }

    //    // Interact
    //    if (obj.action.name == controllerInputs.Player.Interact.name)
    //    {
    //        OnInteract(obj);
    //    }

    //    // InteractAlt
    //    if (obj.action.name == controllerInputs.Player.InteractAlt.name)
    //    {
    //        OnInteractAlt(obj);
    //    }

    //    // Dash
    //    if (obj.action.name == controllerInputs.Player.Dash.name)
    //    {
    //        OnDash(obj);
    //    }

    //    // Throw
    //    if (obj.action.name == controllerInputs.Player.Throw.name)
    //    {
    //        OnThrow(obj);
    //    }

    //    // Jump
    //    if (obj.action.name == controllerInputs.Player.Jump.name)
    //    {
    //        OnJump(obj);
    //    }

    //    // pause
    //    if (obj.action.name == controllerInputs.Player.Pause.name)
    //    {
    //        OnPause(obj);
    //    }
    //}

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
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
        // check if input is within dead zone
        if (move.magnitude < deadZone)
        {
            moveDir = Vector3.zero;
        }
        else
        {
            move.Normalize();
            moveDir = new Vector3(move.x, 0, move.y).normalized;
        }
    }

    public void OnInteractAlt(InputAction.CallbackContext context)
    {
        if (context.performed)
            InteractAltEvent?.Invoke();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
            PauseEvent?.Invoke(this, EventArgs.Empty);
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

    public void OnDebugConsole(InputAction.CallbackContext context)
    {
        if (context.performed)
            DebugConsoleEvent?.Invoke();
    }
}
