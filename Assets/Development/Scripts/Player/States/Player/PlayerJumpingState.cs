using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class PlayerJumpingState : PlayerBaseState
{
    public PlayerJumpingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        AudioManager.Instance.Playoneshot(stateMachine.JumpSFX, false);
        stateMachine.rb.AddForce(stateMachine.jumpForce * Vector3.up);
        Debug.Log("Player enter jumping state");

    }

    public override void Tick(float deltaTime)
    {
        stateMachine.Move(stateMachine.moveSpeed);

        if (stateMachine.IsGrounded())
            stateMachine.SwitchState(new PlayerGroundState(stateMachine));
    }

    public override void Exit()
    {
        
    }
}
