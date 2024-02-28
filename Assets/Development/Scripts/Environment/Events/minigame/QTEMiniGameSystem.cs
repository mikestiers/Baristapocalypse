using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;


public class QTEMiniGameSystem : NetworkBehaviour
{
    public GameObject qteEventCard;
    protected bool qteActive = false;
    protected int currentKey;
    protected float playersMoveSpeed;
     public enum Direction
    {
        None,
        Up,
        Down,
        Left,
        Right,
    }

    public KeyCode[] keySequence;
    private int currentKeyIndex;

    

    protected void CheckInput()
    {
        Direction currentDpadDirection = GetDpadDirection();
        KeyCode expectedKeyCode = GetKeyCodeForDirection(currentDpadDirection);

        if (Input.GetKeyDown(expectedKeyCode))
        {
            currentKeyIndex++;
            if (currentKeyIndex >= keySequence.Length)
            {
                Debug.Log("QTE Success!");
                OnQTECompleted();
            }
        }
        else if (Input.anyKeyDown)
        {
           
            Debug.Log("QTE Failure!");
            EndQTE(); 
        }
    }

    
    private Direction GetDpadDirection()
    {
        Vector2 dpadInput = Gamepad.current.dpad.ReadValue();

        // Check which direction are being pressed
        bool upPressed = dpadInput.y > 0.5f;
        bool downPressed = dpadInput.y < -0.5f;
        bool leftPressed = dpadInput.x < -0.5f;
        bool rightPressed = dpadInput.x > 0.5f;
        
        if (upPressed)
        {
            return Direction.Up;
        }
        else if (downPressed)
        {
            return Direction.Down;
        }
        else if (leftPressed)
        {
            return Direction.Left;
        }
        else if (rightPressed)
        {
            return Direction.Right;
        }
        else
        {
            return Direction.None;
        }
    }

    private KeyCode GetKeyCodeForDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return KeyCode.UpArrow;
            case Direction.Down:
                return KeyCode.DownArrow;
            case Direction.Left:
                return KeyCode.LeftArrow;
            case Direction.Right:
                return KeyCode.RightArrow;
            default:
                return KeyCode.None;
        }
    }

    protected void StartQTE()
    {
        qteActive = true;
        currentKeyIndex = 0;
        qteEventCard.SetActive(true);
        
    }

    private void EndQTE()
    {
        qteEventCard.SetActive(false);
        qteActive = false;
        
    }
    
    public void ResetKeySequnce()
    {
        currentKey = 0;
        
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
    
    protected virtual void OnQTECompleted()
    {
        Debug.Log("Default QTE Completion Handling"); // Default behavior
        EndQTE(); 
    }
}
