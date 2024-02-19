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

    private IngredientListSO ingredientListSO;
    [SerializeField] private PickupListSo pickupList;
    [SerializeField] private MessListSO MessList;
    [SerializeField] private ParticleListSO particleListSO;
    public static BaristapocalypseMultiplayer Instance { get; private set; }

    public static bool playMultiplayer;

    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailToJoinGame;
    public event EventHandler OnPlayerDataNetworkListChanged;

    public int test;

    private NetworkList<PlayerData> playerDataNetworkList;
    public List<Color> playerColorList;

    private void Awake()
    {  
        Instance = this;

        DontDestroyOnLoad(gameObject);

        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    private void Start()
    {
        if (!playMultiplayer)
        {
            StartHost();
            Loader.LoadNetwork(Loader.Scene.T5M3_BUILD);
        }

        ingredientListSO = GameValueHolder.Instance.difficultySettings.allIngredientsList;
    }
    // This should not be in Update() but difficultysettings are not available when the game starts for some reaso

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
        for(int i = 0; i < playerDataNetworkList.Count; i++)
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
            colorId = GetFirstUnusedColorId()
        }); ;
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
        Debug.Log("Spawning ingredient");
        IngredientSO ingredientSO = GetIngredientSOFromIndex(ingredientSOIndex);

        ingredientParentNetworkObjectReference.TryGet(out NetworkObject ingredientParentNetworkObject);
        IIngredientParent ingredientParent = ingredientParentNetworkObject.GetComponent<IIngredientParent>();

        Transform ingredientTransform = Instantiate(ingredientSO.prefab.transform);

        NetworkObject ingredientNetworkObject = ingredientTransform.GetComponent<NetworkObject>();
        ingredientNetworkObject.Spawn(true);

        Ingredient ingredient = ingredientTransform.GetComponent<Ingredient>();
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

    public void DestroyPickup(Pickup pickup)
    {
        DestroyPickupServerRpc(pickup.NetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyPickupServerRpc(NetworkObjectReference pickupNetworkObjectReference)
    {
        pickupNetworkObjectReference.TryGet(out NetworkObject pickupNetworkObject);
        if (pickupNetworkObject == null)
        {
            return;
        }
        Pickup pickup = pickupNetworkObject.GetComponent<Pickup>();
        ClearPickupOnParentClientRpc(pickupNetworkObjectReference);
        pickup.DestroySelf();
    }

    [ClientRpc]
    private void ClearPickupOnParentClientRpc(NetworkObjectReference pickupNetworkObjectReference)
    {
        pickupNetworkObjectReference.TryGet(out NetworkObject pickupNetworkObject);
        Pickup pickup = pickupNetworkObject.GetComponent<Pickup>();

        pickup.ClearPickupOnParent();
    }

    public bool IsPlayerIndexConnected(int playerIndex)
    {
        return playerIndex < playerDataNetworkList.Count;
    }

    public int GetPlayerDataIndexFromClientId(ulong clientId)
    {

        for(int i=0; i < playerDataNetworkList.Count; i++)
        {
            if (playerDataNetworkList[i].clientId == clientId)
            {
                return i;
            }
        }

        return -1;
    }

    public PlayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach (PlayerData playerData in playerDataNetworkList)
        {
            if (playerData.clientId == clientId)
            {
                return playerData;
            }
        }
        return default;
    }

    public PlayerData GetPlayerData()
    {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }
    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return playerDataNetworkList[playerIndex];  
    }

    public Color GetPlayerColor(int colorId)
    {
        return playerColorList[colorId];
    }

    public void ChangePlayerColor(int colorId)
    {
        ChangePlayerColorServerRpc(colorId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default)
    {
        if (!isColorAvailable(colorId))
        {
            // Color not available
            return;
        }

        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.colorId = colorId;

        playerDataNetworkList[playerDataIndex] = playerData;
    }

    private bool isColorAvailable(int colorId)
    {
        foreach(PlayerData playerData in playerDataNetworkList)
        {
            if (playerData.colorId == colorId)
            {
                // Using Color Already
                return false;
            }
        }
        return true;
    }

     private int GetFirstUnusedColorId()
     {
        for (int i=0; i < playerColorList.Count; i++)
        {
            if (isColorAvailable(i))
            {
                return i;
            }               
        }
        return -1;
     }

    public void SpawnPickupObject(PickupSO pickupSo, Base pickupObjectParent)
    {
        SpawnPickupObjectServerRpc(GetPickupObjectSoIndex(pickupSo),pickupObjectParent.GetNetworkObject());
        
    }
    [ServerRpc(RequireOwnership = false)]
    public void SpawnPickupObjectServerRpc(int pickupSoIndex, NetworkObjectReference pickupObjectNetworkObjectReference)
    {
        PickupSO pickupSo = GetPickupSoFromIndex(pickupSoIndex);
        pickupObjectNetworkObjectReference.TryGet(out NetworkObject pickupObjectParentNetworkObject);
        Base pickupObjectParent = pickupObjectParentNetworkObject.GetComponent<Base>();

        Transform pickupObjectTransform = Instantiate(pickupSo.prefab.transform);

        NetworkObject pickupObjectNetworkObject = pickupObjectTransform.GetComponent<NetworkObject>();
        pickupObjectNetworkObject.Spawn(true);
        pickupObjectNetworkObject.transform.position = pickupObjectParent.transform.position;
    }

    public int GetPickupObjectSoIndex(PickupSO pickupSo)
    {
        return pickupList.PickupListSO.IndexOf(pickupSo);
    }

    public PickupSO GetPickupSoFromIndex(int pickupSoIndex)
    {
       return pickupList.PickupListSO[pickupSoIndex];
    }
    public  void PlayerCreateSpill(MessSO messSo, ISpill messObjectParent )
    {
        PlayerCreateSpillServerRpc(GetMessObjectSoIndex(messSo),messObjectParent.GetNetworkObject() );
    }
    [ServerRpc(RequireOwnership = false)]
    private void PlayerCreateSpillServerRpc(int MessIndex, NetworkObjectReference spillNetworkObjectReference)
    {
        MessSO spillPrefab = GetMessSoFromIndex(MessIndex);
        GameObject messGameObject = Instantiate(spillPrefab.prefab);

        NetworkObject spillNetworkObject = messGameObject.GetComponent<NetworkObject>();
        spillNetworkObject.Spawn(true);
        Spill Mess = messGameObject.GetComponent<Spill>();

        spillNetworkObjectReference.TryGet(out NetworkObject messObjectParentNetworkObject);
        ISpill messObjectParent = messObjectParentNetworkObject.GetComponent<ISpill>();
        
        Mess.SetSpillPosition(messObjectParent);
        

        // PlayerCreateSpillClientRpc(spillNetworkObjectReference);
    }
    
    
    public int GetMessObjectSoIndex(MessSO messSo)
    {
        return MessList.MessSoList.IndexOf(messSo);
    }

    public MessSO GetMessSoFromIndex(int MessSoIndex)
    {
        return MessList.MessSoList[MessSoIndex];
    }

    // Particle Networking Implementation


    public void SpawnParticle(ParticleSO particleSO, IParticleParent particleParent)
    {
        SpawnParticleServerRpc(GetParticleSOIndex(particleSO), particleParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnParticleServerRpc(int particleSOIndex, NetworkObjectReference particleParentNetworkObjectReference)
    {
        ParticleSO particleSO = GetParticleSOFromIndex(particleSOIndex);

        particleParentNetworkObjectReference.TryGet(out NetworkObject particleParentNetworkObject);
        IParticleParent particleParent = particleParentNetworkObject.GetComponent<IParticleParent>();

        Transform particleTransform = Instantiate(particleSO.particlePrefab.transform);

        NetworkObject particleNetworkObject = particleTransform.GetComponent<NetworkObject>();
        particleNetworkObject.Spawn(true);

        Particle particle = particleTransform.GetComponent<Particle>();
        particle.SetParticleParent(particleParent);

    }

    public void DestroyParticle(Particle particle)
    {
        DestroyParticleServerRpc(particle.NetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyParticleServerRpc(NetworkObjectReference particleNetworkObjectReference)
    {
        particleNetworkObjectReference.TryGet(out NetworkObject particleNetworkObject);
        if (particleNetworkObject == null)
        {
            return;
        }
        Particle particle = particleNetworkObject.GetComponent<Particle>();
        ClearParticleOnParentClientRpc(particleNetworkObjectReference);
        particle.DestroySelf();
    }

    [ClientRpc]
    private void ClearParticleOnParentClientRpc(NetworkObjectReference particleNetworkObjectReference)
    {
        particleNetworkObjectReference.TryGet(out NetworkObject particleNetworkObject);
        Particle particle = particleNetworkObject.GetComponent<Particle>();

        particle.ClearParticleOnParent();
    }

    public int GetParticleSOIndex(ParticleSO particleSO)
    {
        return particleListSO.particleSOList.IndexOf(particleSO);
    }

    public ParticleSO GetParticleSOFromIndex(int particleSOIndex)
    {
        return particleListSO.particleSOList[particleSOIndex];
    }
}
