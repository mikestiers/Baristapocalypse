using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem;

public class SpawnPlayerSetup : MonoBehaviour
{
    [SerializeField] private GameObject playerSetupMenuPrefab;
    [SerializeField] private PlayerInput input;

    private void Awake()
    {
        var rootMenu = GameObject.Find("PlayerSelection");
        if (rootMenu != null)
        {
            var menu = Instantiate(playerSetupMenuPrefab, rootMenu.transform);
            input.uiInputModule = menu.GetComponentInChildren<InputSystemUIInputModule>();
            menu.GetComponent<PlayerSetupMenuController>().SetPlayerIndex(input.playerIndex);
        }
    }
}
