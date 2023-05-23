using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameManager : Singleton<GameManager>
{
    [HideInInspector]
    public PlayerInput playerInput;

    protected override void Awake()
    {
        base.Awake();
        playerInput = new PlayerInput();
        playerInput.Enable();
    }
}
