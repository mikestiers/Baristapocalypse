using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    
    [field: SerializeField] public IngredientSO IngredientSO { get; private set; }

    private IIngredientParent ingredientParent;

    public IngredientSO GetIngredientSO()
    {
        return IngredientSO;
    }

    public void SetIngredientParent(IIngredientParent ingredientParent)
    {
        if(this.ingredientParent != null)
        {
            this.ingredientParent.ClearIngredient();
        }
        this.ingredientParent = ingredientParent;

        if (ingredientParent.HasIngredient())
        {
            Debug.Log("Counter already has object");
        }
        ingredientParent.SetIngredient(this);

        transform.parent = ingredientParent.GetIngredientTransform();
        transform.localPosition = Vector3.zero;
    }

    public IIngredientParent GetIngredientParent()
    {
        return ingredientParent;
    }

    public void DestroyIngredient()
    {
        ingredientParent.ClearIngredient();
        Destroy(gameObject);
    }

    public static Ingredient SpawnIngredient(IngredientSO ingredientSO, IIngredientParent ingredientParent)
    {
        GameObject ingredientPrefab = Instantiate(ingredientSO.prefab);
        Ingredient ingredient = ingredientPrefab.GetComponent<Ingredient>();
        ingredient.GetComponent<Ingredient>().SetIngredientParent(ingredientParent);
        // Disable the collider immediately after instantiation
        Collider ingredientCollider = ingredient.GetComponent<Collider>();
        if (ingredientCollider != null)
        {
            ingredientCollider.enabled = false;
        }

        return ingredient;
    }
}
