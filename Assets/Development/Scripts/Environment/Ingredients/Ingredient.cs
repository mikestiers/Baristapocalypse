using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Ingredient : NetworkBehaviour
{ 
    [field: SerializeField] public IngredientSO IngredientSO { get; private set; }

    private IIngredientParent ingredientParent;
    public IngredientFollowTransform followTransform;

    protected virtual void Awake()
    {
        followTransform = GetComponent<IngredientFollowTransform>();   
    }

    public IngredientSO GetIngredientSO()
    {
        return IngredientSO;
    }

    public void SetIngredientParent(IIngredientParent ingredientParent)
    {
        SetIngredientParentServerRpc(ingredientParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetIngredientParentServerRpc(NetworkObjectReference ingredientParentNetworkObjectReference)
    {
        SetIngredientParentClientRpc(ingredientParentNetworkObjectReference);   
    }

    [ClientRpc]
    private void SetIngredientParentClientRpc(NetworkObjectReference ingredientParentNetworkObjectReference)
    {
        ingredientParentNetworkObjectReference.TryGet(out NetworkObject ingredientParentNetworkObject);
        IIngredientParent ingredientParent = ingredientParentNetworkObject.GetComponent<IIngredientParent>();

        if (this.ingredientParent != null)
        {
            this.ingredientParent.ClearIngredient();
        }
        this.ingredientParent = ingredientParent;
        ingredientParent.SetIngredient(this);

        followTransform.SetTargetTransform(ingredientParent.GetIngredientTransform());
        DisableIngredientCollision(this);
    }

    public void DisableIngredientCollision(Ingredient ingredient)
    {
        DisableIngredientCollisionServerRpc(ingredient.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void DisableIngredientCollisionServerRpc(NetworkObjectReference ingredientNetworkObjectReference)
    {
        DisableIngredientCollisionClientRpc(ingredientNetworkObjectReference);
    }

    [ClientRpc]
    private void DisableIngredientCollisionClientRpc(NetworkObjectReference ingredientNetworkObjectReference)
    {
        ingredientNetworkObjectReference.TryGet(out NetworkObject ingredientNetworkObject);
        Ingredient ingredient = ingredientNetworkObject.GetComponent<Ingredient>();
        // Disable the collider immediately after instantiation
        Collider ingredientCollider = ingredient.GetComponent<Collider>();
        if (ingredientCollider != null)
        {
            ingredientCollider.enabled = false;
        }
    }

    public void EnableIngredientCollision(Ingredient ingredient)
    {
        EnableIngredientCollisionServerRpc(ingredient.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void EnableIngredientCollisionServerRpc(NetworkObjectReference ingredientNetworkObjectReference)
    {
        EnableIngredientCollisionClientRpc(ingredientNetworkObjectReference);
    }

    [ClientRpc]
    private void EnableIngredientCollisionClientRpc(NetworkObjectReference ingredientNetworkObjectReference)
    {
        ingredientNetworkObjectReference.TryGet(out NetworkObject ingredientNetworkObject);
        Ingredient ingredient = ingredientNetworkObject.GetComponent<Ingredient>();
        // Disable the collider immediately after instantiation
        Collider ingredientCollider = ingredient.GetComponent<Collider>();
        if (ingredientCollider != null)
        {
            ingredientCollider.enabled = true;
        }
    }

    public IIngredientParent GetIngredientParent()
    {
        return ingredientParent;
    }

    public static void DestroyIngredient(Ingredient ingredient)
    {
        BaristapocalypseMultiplayer.Instance.DestroyIngredient(ingredient);
    }

    public static void SpawnIngredient(IngredientSO ingredientSO, IIngredientParent ingredientParent)
    {
        BaristapocalypseMultiplayer.Instance.SpawnIngredient(ingredientSO, ingredientParent);
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void ClearIngredientOnParent()
    {
        ingredientParent.ClearIngredient();
    }
}
