using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveState : PlayerBaseState
{

    
    //calls base constructor and pass in base stateMachine 
    public PlayerMoveState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.inputManager.JumpEvent += OnJump;
        //mouse left click
        stateMachine.inputManager.InteractEvent += OnInteractEvent;
        //mouse right click
        stateMachine.inputManager.ThrowEvent += OnThrowEvent;

        Debug.Log("Player enter moving state");

    }

  

    public override void Tick(float deltaTime)
    {
        stateMachine.IsGrounded();
        //player movement
        stateMachine.HandleMovement(deltaTime);
        /*stateMachine.inputManager.playerMovement.y = stateMachine.rb.velocity.y;
        stateMachine.rb.velocity = stateMachine.inputManager.playerMovement;*/

        stateMachine.HandleInteractions();


    }

    public override void Exit()
    {
        stateMachine.inputManager.JumpEvent -= OnJump;
        stateMachine.inputManager.InteractEvent -= OnInteractEvent;
        stateMachine.inputManager.ThrowEvent -= OnThrowEvent;
    } 


    public void OnJump()
    {
        if (!stateMachine.IsGrounded()) { return; }

        stateMachine.SwitchState(new PlayerJumpingState(stateMachine));

        //stateMachine.rb.AddForce(stateMachine.jumpForce * Vector3.up);

    }

    private void OnInteractEvent()
    {
        if(stateMachine.HandleInteractions() == true)
            stateMachine.SwitchState(new PlayerPickUpState(stateMachine));

    }

    private void OnThrowEvent()
    {
        if(stateMachine.isPickedup)
            stateMachine.SwitchState(new PlayerThrowState(stateMachine));

    }





}
