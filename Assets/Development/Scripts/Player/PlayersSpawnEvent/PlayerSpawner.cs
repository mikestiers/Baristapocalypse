using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{

    GameObject prefabPlayer1;
    GameObject prefabPlayer2;
    GameObject prefabPlayer3;
    GameObject prefabPlayer4;

    private void OnEnable()
     {
        // Subscribe to the GameManager's OnPlayerSpawn event.
        GameManager.OnPlayersSpawned += HandlePlayersSpawned;
     }

    private void OnDisable()
    {
        // Unsubscribe from the GameManager's OnPlayerSpawn event.
        GameManager.OnPlayersSpawned -= HandlePlayersSpawned;
    }

    private void HandlePlayersSpawned(bool player1Active, bool player2Active, bool player3Active, bool player4Active)
    {
        if (player1Active == true)
        {
            GameObject player1 = Instantiate(prefabPlayer1, GameObject.FindGameObjectWithTag("LevelSpawnpoint").GetComponent<LevelSpawnpoints>().spawnpointPlayer1);
        }
        if (player2Active == true)
        {
            GameObject player2 = Instantiate(prefabPlayer2, GameObject.FindGameObjectWithTag("LevelSpawnpoint").GetComponent<LevelSpawnpoints>().spawnpointPlayer2);
        }
        if (player3Active == true)
        {
            GameObject player3 = Instantiate(prefabPlayer3, GameObject.FindGameObjectWithTag("LevelSpawnpoint").GetComponent<LevelSpawnpoints>().spawnpointPlayer3);
        }
        if (player4Active == true)
        {
            GameObject player4 = Instantiate(prefabPlayer4, GameObject.FindGameObjectWithTag("LevelSpawnpoint").GetComponent<LevelSpawnpoints>().spawnpointPlayer4);
        }
    }

    private void Start()
    {
        prefabPlayer1 = GameManager.Instance.player1Prefab;
        prefabPlayer2 = GameManager.Instance.player2Prefab;
        prefabPlayer3 = GameManager.Instance.player3Prefab;
        prefabPlayer4 = GameManager.Instance.player4Prefab;
        GameManager.Instance.SpawnPlayers(GameManager.Instance.player1Active, GameManager.Instance.player2Active, GameManager.Instance.player3Active, GameManager.Instance.player4Active);
    }
}
