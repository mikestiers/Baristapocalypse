using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{

    [SerializeField] GameObject prefabPlayer1;
    [SerializeField] GameObject prefabPlayer2;
    [SerializeField] GameObject prefabPlayer3;
    [SerializeField] GameObject prefabPlayer4;

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
        GameManager.Instance.SpawnPlayers(GameManager.Instance.player1Active, GameManager.Instance.player2Active, GameManager.Instance.player3Active, GameManager.Instance.player4Active);
    }
}
