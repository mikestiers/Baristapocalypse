using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Share methods between the different player states
/// </summary>
public abstract class PlayerBaseState : State
{
    //reference to the player, so each state can reference the player
    protected PlayerStateMachine stateMachine;

    //Constructor
    public PlayerBaseState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

}
