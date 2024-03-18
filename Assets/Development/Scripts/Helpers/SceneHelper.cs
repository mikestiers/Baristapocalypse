using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System;

public class SceneHelper : MonoBehaviour
{
    public bool isRestartGame = false;
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

    // Not used for the moment
    private List<string> componentsToDestroy = new List<string>
    {
        "NetworkManager",
        "CustomerManager",
        "GameValueHolder",
        "ScoreTimerManager",
        "LobbyManager",
        "PlayerConfigurationManager",
        "BaristapocalypseMultiplayer",
        "CustomerReviewManager",
        "OrderManager",
        "ShiftEvaluationUI"
    };

    public void ResetGame()
    {
        Debug.Log("Reset Game Function Called");
        LobbyManager.Instance.LeaveLobby();
        ShutdownAndDestroy<NetworkManager>();
        FindObjectOfType<CustomerManager>()?.ResetSingleton();
        FindObjectOfType<ScoreTimerManager>()?.ResetSingleton();
        FindObjectOfType<GameValueHolder>()?.Reset();
        DestroyIfExists<CustomerReviewManager>();
        DestroyIfExists<LobbyManager>();
        DestroyIfExists<PlayerConfigurationManager>();
        DestroyIfExists<BaristapocalypseMultiplayer>();
        DestroyIfExists<NetworkManager>();
        DestroyIfExists<OrderManager>();
        DestroyIfExists<TutorialManager>();
    }

    private void ShutdownAndDestroy<T>() where T : MonoBehaviour
    {
        T component = FindObjectOfType<T>();
        if (component != null)
        {
            if (component is NetworkManager networkManager)
                networkManager.Shutdown();

            Destroy(component);
            Destroy(component.gameObject);
        }
    }

    private void DestroyIfExists<T>() where T : MonoBehaviour
    {
        T component = FindObjectOfType<T>();
        if (component != null)
        {
            Destroy(component);
            Destroy(component.gameObject);
        }
    }
}

