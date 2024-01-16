using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;
using System;

public class BaristapocalypseMultiplayer  : NetworkBehaviour
{
    public const int MAX_PLAYERS = 4;

    [SerializeField] private IngredientListSO ingredientListSO;
    [SerializeField] private PickupListSo pickupList;
    public static BaristapocalypseMultiplayer Instance { get; private set; }

    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailToJoinGame;
    public event EventHandler OnPlayerDataNetworkListChanged;

    private NetworkList<PlayerData> playerDataNetworkList;

    private void Awake()
    {  
        Instance = this;

        DontDestroyOnLoad(gameObject);

        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
    {
        for(int i=0; i<playerDataNetworkList.Count; i++)
        {
            PlayerData playerData = playerDataNetworkList[i];
            if(playerData.clientId == clientId)
            {
                playerDataNetworkList.RemoveAt(i);
            }
        }
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        playerDataNetworkList.Add(new PlayerData
        {
            clientId = clientId,
        });
    }

    public void StartClient()
    {
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);

        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        if (SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectScene.ToString())
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game has already started";
            return;
        }

        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYERS)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game is full";
            return;
        }

        connectionApprovalResponse.Approved = true;
    }

    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId)
    {
        OnFailToJoinGame?.Invoke(this, EventArgs.Empty);
    }


    public void SpawnIngredient(IngredientSO ingredientSO, IIngredientParent ingredientParent)
    {
        SpawnIngredientServerRpc(GetIngredientSOIndex(ingredientSO), ingredientParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnIngredientServerRpc(int ingredientSOIndex, NetworkObjectReference ingredientParentNetworkObjectReference)
    {
        IngredientSO ingredientSO = GetIngredientSOFromIndex(ingredientSOIndex);
        GameObject ingredientPrefab = Instantiate(ingredientSO.prefab);

        NetworkObject ingredientNetworkObject = ingredientPrefab.GetComponent<NetworkObject>();
        ingredientNetworkObject.Spawn(true);
        Ingredient ingredient = ingredientPrefab.GetComponent<Ingredient>();

        ingredientParentNetworkObjectReference.TryGet(out NetworkObject ingredientParentNetworkObject);
        IIngredientParent ingredientParent = ingredientParentNetworkObject.GetComponent<IIngredientParent>();
        ingredient.SetIngredientParent(ingredientParent);

        ingredient.DisableIngredientCollision(ingredient);
 
    }

    public int GetIngredientSOIndex(IngredientSO ingredientSO)
    {
        return ingredientListSO.ingredientSOList.IndexOf(ingredientSO);
    }

    public IngredientSO GetIngredientSOFromIndex(int ingredientSOIndex)
    {
        return ingredientListSO.ingredientSOList[ingredientSOIndex];
    }

    public void DestroyIngredient(Ingredient ingredient)
    {
        DestroyIngredientServerRpc(ingredient.NetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyIngredientServerRpc(NetworkObjectReference ingredientNetworkObjectReference)
    {
        ingredientNetworkObjectReference.TryGet(out NetworkObject ingredientNetworkObject);
        if(ingredientNetworkObject == null)
        {
            return;
        }
        Ingredient ingredient = ingredientNetworkObject.GetComponent<Ingredient>();
        ClearIngredientOnParentClientRpc(ingredientNetworkObjectReference);
        ingredient.DestroySelf();
    }

    [ClientRpc]
    private void ClearIngredientOnParentClientRpc(NetworkObjectReference ingredientNetworkObjectReference)
    {
        ingredientNetworkObjectReference.TryGet(out NetworkObject ingredientNetworkObject);
        Ingredient ingredient = ingredientNetworkObject.GetComponent<Ingredient>();

        ingredient.ClearIngredientOnParent();
    }

    public bool IsPlayerIndexConnected(int playerIndex)
    {
        return playerIndex < playerDataNetworkList.Count;
    }

    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return playerDataNetworkList[playerIndex];  
    }

    public void SpawnPickupObject(PickupSO pickupSo,IPickupObjectParent pickupObjectParent )
    {
        SpawnPickupObjectServerRpc(GetPickupObjectSoIndex(pickupSo),pickupObjectParent.GetNetworkObject());
        
    }
    [ServerRpc(RequireOwnership = false)]
    public void SpawnPickupObjectServerRpc(int pickupSoIndex, NetworkObjectReference pickupObjectNetworkObjectReference)
    {
        
        PickupSO pickupSo = GetPickupSoFromIndex(pickupSoIndex);
        Debug.Log($"Index: {pickupSoIndex}, PickupSO: {pickupSo}");
        GameObject pickupGameObject = Instantiate(pickupSo.prefab);
        
        NetworkObject pickupObjectNetworkObject = pickupGameObject.GetComponent<NetworkObject>();
        pickupObjectNetworkObject.Spawn(true);
        Pickup pickup = pickupGameObject.GetComponent<Pickup>();

        pickupObjectNetworkObjectReference.TryGet(out NetworkObject pickupObjectParentNetworkObject);
        IPickupObjectParent pickupObjectParent = pickupObjectNetworkObject.GetComponent<IPickupObjectParent>();
        
      //  pickup.SetpickupObjectParent(pickupObjectParent);
       
    }

    public int GetPickupObjectSoIndex(PickupSO pickupSo)
    {
        Debug.Log(pickupSo +" scanned");
         return pickupList.PickupListSO.IndexOf(pickupSo);
    }

    public PickupSO GetPickupSoFromIndex(int pickupSoIndex)
    {
        return pickupList.PickupListSO[pickupSoIndex];
    }
}
