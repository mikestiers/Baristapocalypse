using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerPickUpState : PlayerBaseState
{
    public PlayerPickUpState(PlayerStateMachine stateMachine) : base(stateMachine) {}


    public override void Enter()
    {
        Debug.Log("picking up....");
        if (!stateMachine.pickupPoint)
            stateMachine.pickupPoint = GameObject.FindGameObjectWithTag("AttachPoint").transform;

        if (stateMachine.dropForce <= 0)
            stateMachine.dropForce = 10f;
        stateMachine.isPickedup = true;
    }

    public override void Tick(float deltaTime)
    {
        stateMachine.target.position = stateMachine.pickupPoint.position;
        stateMachine.target.SetParent(stateMachine.pickupPoint);
        stateMachine.target.rotation = stateMachine.pickupPoint.rotation;

        stateMachine.SwitchState(new PlayerMoveState(stateMachine));

    }

    public override void Exit()
    {
        
    }


    
}
