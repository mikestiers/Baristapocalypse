using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


public class TestingNetcodeUI : NetworkBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Button spawnCustomerButton;
    [SerializeField] private CustomerManager customerManager;

    // temporary to assign camera to player
    private PlayerController playerController;
    [SerializeField] private InitializeLevel initializeLevel;

    private void Awake()
    {

        hostButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });
        clientButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });
        spawnCustomerButton.onClick.AddListener(SpawnCustomer);
    }

    private void SpawnCustomer()
    {
        if (IsServer)
        {
            CustomerManager test = Instantiate(customerManager); // temporary
            test.GetComponent<NetworkObject>().Spawn(true);// temporary***

        }

        if (initializeLevel)
        {
        // temp to assign camera to player
            playerController = FindObjectOfType<PlayerController>();
            initializeLevel.AddCameraToPlayer(playerController.gameObject);

        }

    }
}
