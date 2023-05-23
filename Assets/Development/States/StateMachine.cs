using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores the current state the character is in, and switch between states
/// </summary>
public abstract class StateMachine : MonoBehaviour
{
    //Holds the current state
    private State currentState;

    public void SwitchState(State newState)
    {
        currentState?.Exit();// exit current state if not null, to change to a different state
        currentState = newState;
        currentState?.Enter();// switch state to new state if not null
    }
   
    // Update is called once per frame
    private void Update()
    {
        currentState?.Tick(Time.deltaTime); //Gets the current state every frame if not null
        
        
    }


}
