using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGroundState : PlayerBaseState
{

    
    //calls base constructor and pass in base stateMachine 
    public PlayerGroundState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.inputManager.JumpEvent += OnJump;
       
        InputManager.Instance.playerInput.Player.Interact.performed += context => stateMachine.Interact(context);
        InputManager.Instance.playerInput.Player.InteractAlt.performed += context => stateMachine.InteractAlt(context);
        Debug.Log("Player enter moving state");

    }
   

    public override void Tick(float deltaTime)
    {
        //Ground Check
        stateMachine.IsGrounded();
        //player movement
        stateMachine.Move();

        float interactDistance = 6.0f;
        if (Physics.Raycast(stateMachine.transform.position, stateMachine.transform.forward, out RaycastHit raycastHit, interactDistance, stateMachine.isStationLayer))
        {
            if (raycastHit.transform.TryGetComponent(out BaseStation baseStation))
            {
                if (baseStation != stateMachine.selectedStation)
                {
                    stateMachine.SetSelectedStation(baseStation);
                }
            }
            else
            {
                stateMachine.SetSelectedStation(null);
            }
        }
        else
        {
            stateMachine.SetSelectedStation(null);
        }

    }

    public override void Exit()
    {
        
        stateMachine.inputManager.JumpEvent -= OnJump;
        
    }

   
    public void OnJump()
    {
        if (!stateMachine.IsGrounded()) { return; }

        stateMachine.SwitchState(new PlayerJumpingState(stateMachine));

    }

    




}
