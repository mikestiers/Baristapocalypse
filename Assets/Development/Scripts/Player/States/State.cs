using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Basic state to create player and other characters base satates
/// </summary>
public abstract class State
{
    public abstract void Enter();// Works as Start from MonoBehaviour, actions on state enter
    public abstract void Tick(float deltaTime);// Works as Update from MonoBehaviour
    public abstract void Exit(); // actions on state exit

}
