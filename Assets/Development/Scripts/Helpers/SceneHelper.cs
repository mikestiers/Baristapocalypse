using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System;

public class SceneHelper : MonoBehaviour
{
    private static SceneHelper _instance;

    public static SceneHelper Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SceneHelper>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("SceneHelper");
                    _instance = singletonObject.AddComponent<SceneHelper>();
                }
            }

            return _instance;
        }
    }

    private List<string> componentsToDestroy = new List<string>
    {
        "NetworkManager",
        "CustomerManager",
        "GameValueHolder",
        "ScoreTimerManager",
        "LobbyManager",
        "PlayerConfigurationManager",
        "BaristapocalypseMultiplayer"
    };

    protected virtual void Awake()
    {
        if (!_instance)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }

        Destroy(gameObject);
    }

    public void ResetGame()
    {
        ShutdownAndDestroy<NetworkManager>();
        FindObjectOfType<CustomerManager>()?.ResetSingleton();
        FindObjectOfType<GameValueHolder>()?.ResetSingleton();
        FindObjectOfType<ScoreTimerManager>()?.ResetSingleton();
        DestroyIfExists<LobbyManager>();
        DestroyIfExists<PlayerConfigurationManager>();
        DestroyIfExists<BaristapocalypseMultiplayer>();
        DestroyIfExists<NetworkManager>();
    }

    private void ShutdownAndDestroy<T>() where T : MonoBehaviour
    {
        T component = FindObjectOfType<T>();
        if (component != null)
        {
            if (component is NetworkManager networkManager)
                networkManager.Shutdown();

            Destroy(component);
        }
    }

    private void DestroyIfExists<T>() where T : MonoBehaviour
    {
        T component = FindObjectOfType<T>();
        if (component != null)
        {
            Destroy(component);
        }
    }
}

