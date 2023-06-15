using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrowState : PlayerBaseState
{
    
    public PlayerThrowState(PlayerStateMachine stateMachine) : base(stateMachine){}

    public override void Enter()
    {
        
       //place for animations and sounds


    }

    public override void Tick(float deltaTime)
    {
        
        stateMachine.SwitchState(new PlayerGroundState(stateMachine));
    }

    public override void Exit()
    {
       
    }

    
}
