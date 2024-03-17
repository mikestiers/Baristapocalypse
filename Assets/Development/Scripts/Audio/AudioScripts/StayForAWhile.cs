using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StayForAWhile : Singleton<StayForAWhile>
{
    // Update is called once per frame
    void Update()
    {
        if(SceneManager.GetActiveScene() != SceneManager.GetSceneByName("LobbyScene") && SceneManager.GetActiveScene() != SceneManager.GetSceneByName("CharacterSelectScene"))
        {
            Destroy(gameObject);
        }
    }
}
