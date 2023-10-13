using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStation : MonoBehaviour, IIngredientParent
{
    [SerializeField] private Transform stationTopPoint;
    
    private Ingredient ingredient;
    
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


}
