using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnHandler : MonoBehaviour
{
    public delegate void GameManager();

    public static event GameManager OnPlayerSpawnEvent;

    public static void RaisePlayerSpawnEvent()
    {
        if (OnPlayerSpawnEvent != null)
        {
            OnPlayerSpawnEvent();
        }
    }
}
