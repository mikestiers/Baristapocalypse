using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEditor.ShaderGraph;
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
    public event Action BrewingStationSelectEvent;
    public event Action BrewingStationEmptyEvent;
    public event Action AnyPressedEvent;

    public event Action DebugConsoleEvent;

    public InputDevice inputDevice;
    public InputImagesSO inputImagesSOXbox;
    public InputImagesSO inputImagesSODualSense;
    public InputImagesSO inputImagesSOKeyboardMouse;
    private InputImagesSO inputImagesSO;
    public static event Action<InputImagesSO> OnInputChanged;

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

        if (Gamepad.current != null && Gamepad.current.displayName.Contains("Xbox"))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            inputImagesSO = inputImagesSOXbox;
            inputDevice = InputDevice.Xbox;
        }
        else if (Gamepad.current != null && Gamepad.current.displayName.Contains("DualSense"))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked; 
            inputImagesSO = inputImagesSODualSense;
            inputDevice = InputDevice.DualSense;
        }
        else
        {
            inputImagesSO = inputImagesSOKeyboardMouse;
            inputDevice = InputDevice.KeyboardMouse;
        }
        OnInputChanged?.Invoke(inputImagesSO);
    }

    private void Start()
    {
        controllerInputs.Player.SetCallbacks(this);// SetCallbacks calls the methods for us
        controllerInputs.Player.Enable();
    }

    public void OnMouseDetection(InputAction.CallbackContext context)
    {
        if (inputDevice == InputDevice.KeyboardMouse)
            return;
        if (context.performed)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            inputImagesSO = inputImagesSOKeyboardMouse;
            inputDevice = InputDevice.KeyboardMouse;
            OnInputChanged?.Invoke(inputImagesSO);
        }
    }

    public void OnGamepadDetection(InputAction.CallbackContext context)
    {
        if (inputDevice == InputDevice.DualSense || inputDevice == InputDevice.Xbox)
            return;
        if (context.performed)
            GamepadTypeDetection(context);
    }

    public void OnKeyboardDetection(InputAction.CallbackContext context)
    {
        if (inputDevice == InputDevice.KeyboardMouse)
            return;
        if (context.performed)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            inputImagesSO = inputImagesSOKeyboardMouse;
            inputDevice = InputDevice.KeyboardMouse;
            OnInputChanged?.Invoke(inputImagesSO);
        }
    }

    public void GamepadTypeDetection(InputAction.CallbackContext context)
    {
        if (Gamepad.current != null && Gamepad.current.displayName.Contains("DualSense"))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            inputImagesSO = inputImagesSODualSense;
            inputDevice = InputDevice.DualSense;
            OnInputChanged?.Invoke(inputImagesSO);
        }
        else if (Gamepad.current != null && Gamepad.current.displayName.Contains("Xbox"))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            inputImagesSO = inputImagesSOXbox;
            inputDevice = InputDevice.Xbox;
            OnInputChanged?.Invoke(inputImagesSO);
        }
        else // some other gamepad, just show xbox
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            inputImagesSO = inputImagesSOXbox;
            inputDevice = InputDevice.Xbox;
            OnInputChanged?.Invoke(inputImagesSO);
        }
    }

    private void OnDestroy()
    {
        controllerInputs.Player.Disable();
    }

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

    public void OnBrewingStationSelect(InputAction.CallbackContext context)
    {
        if (context.performed)
            BrewingStationSelectEvent?.Invoke();
    }

    public void OnBrewingStationEmpty(InputAction.CallbackContext context)
    {
        if (context.performed)
            BrewingStationEmptyEvent?.Invoke();
    }

    public void OnDebugConsole(InputAction.CallbackContext context)
    {
        if (context.performed)
            DebugConsoleEvent?.Invoke();
    }

    public enum InputDevice
    {
        None,
        DualSense,
        Xbox,
        KeyboardMouse
    }
}
