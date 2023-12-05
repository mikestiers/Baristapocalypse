using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class InitializeLevel : MonoBehaviour
{
    [SerializeField] private Transform[] playerSpawns;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Camera virtualCamera;
    // Start is called before the first frame update
    private void Start()
    {
        //var playerConfigs = PlayerConfigurationManager.Instance.GetPlayerConfigs().ToArray();
        //for (int i = 0; i < playerConfigs.Length; i++)
        //{
        //var player = Instantiate(playerPrefab, playerSpawns[i].position, playerSpawns[i].rotation, gameObject.transform);
        //player.GetComponent<InputManager>().InitializePlayer(playerConfigs[i]);
        //Debug.Log("player" +  player);


        //virtualCamera.GetComponent<CameraManager>().targets.Add(player.transform);
        //}
    }

   /* public void AddCameraToPlayer(GameObject playerOnScene)
    {
        virtualCamera.GetComponent<CameraManager>().targets.Add(playerOnScene.transform);
    }*/
}