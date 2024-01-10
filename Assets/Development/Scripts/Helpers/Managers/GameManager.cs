using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using static Unity.Burst.Intrinsics.Arm;

[DefaultExecutionOrder(-1)]
public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    // Testing
    [SerializeField] private GameObject play;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject playButton;
    
    //Input Events
    public Vector2 MovementValue { get; private set; }

    public GameState gameState = GameState.RUNNING;

    [SerializeField] private Transform player1Prefab;
    public GameObject player2Prefab;
    public GameObject player3Prefab;
    public GameObject player4Prefab;

    private void Awake()
    {
        Instance = this;
    }
    public void SetPlayButtonActive()
    {
        playButton.SetActive(true);
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Transform playerTransform = Instantiate(player1Prefab);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }


}

public enum GameState
{
    RUNNING, PAUSED, GAMEOVER

}
