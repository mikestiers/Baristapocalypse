using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BaristapocalypseMultiplayer : NetworkBehaviour
{
    [SerializeField] private IngredientListSO ingredientListSO;

    public static BaristapocalypseMultiplayer Instance { get; private set; }

    private void Awake()
    {  
        Instance = this; 
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
        Debug.Log(ingredientSO.sweetness);
        int i = ingredientListSO.ingredientSOList.IndexOf(ingredientSO);
        Debug.Log(ingredientListSO.ingredientSOList[i].sweetness);
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
}
