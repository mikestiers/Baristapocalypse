using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpingState : PlayerBaseState
{
    public PlayerJumpingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        
        stateMachine.rb.AddForce(stateMachine.jumpForce * Vector3.up);
        Debug.Log("Player enter jumping state");

    }

    public override void Tick(float deltaTime)
    {
        stateMachine.inputManager.playerMovement.y = stateMachine.rb.velocity.y;
        stateMachine.rb.velocity = stateMachine.inputManager.playerMovement;

        if (stateMachine.IsGrounded())
            stateMachine.SwitchState(new PlayerMoveState(stateMachine));
    }

    public override void Exit()
    {

    }
}
