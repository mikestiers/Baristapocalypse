using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Base : NetworkBehaviour, IIngredientParent, ISpill
{
    [SerializeField] private Transform stationTopPoint;
    [SerializeField] private Spill spill;
    private Ingredient ingredient;
    [SerializeField] private Transform baseSpillSpawnPoint;

    public virtual void Interact(PlayerController player)
    {

        //Debug.LogError("BaseCounter.Interact();");
    }
    public virtual void InteractAlt(PlayerController player)
    {
        //Debug.LogError("BaseCounter.InteractAlt();");
    }

    public Transform GetIngredientTransform()
    {
        return stationTopPoint;
    }

    public void SetIngredient(Ingredient ingredient)
    {
        this.ingredient = ingredient;
    }

    public Ingredient GetIngredient()
    {
        return ingredient;
    }

    public void ClearIngredient()
    {
        ingredient = null;
    }

    public bool HasIngredient()
    {
        return ingredient != null;
    }

    public Transform GetSpillTransform()
    {
        return baseSpillSpawnPoint;
    }

    public void SetSpill(Spill spill)
    {
        this.spill = spill;
    }

    public void ClearSpill()
    {
        spill = null;
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
}
