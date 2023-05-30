using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CoffeeGrinderStation;

public class BrewingStation : BaseStation, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    [SerializeField] private BrewingRecipeSO[] brewingRecipeSOArray;

    private float brewingTimer;
    private BrewingRecipeSO brewingRecipeSO;
    private bool brewing;
    private void Update()
    {
        if (HasIngredient())
        {
            if (brewing)
            {
                brewingTimer += Time.deltaTime;
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = (float)brewingTimer / brewingRecipeSO.brewingMax
                });

                if (brewingTimer >= brewingRecipeSO.brewingMax)
                {
                    GetIngredient().DestroyIngredient();
                    Ingredient.SpawnIngredient(brewingRecipeSO.output, this);
                    brewing = false;
                }
            }
        }
    }

    public override void Interact(PlayerStateMachine player)
    {
        if (!HasIngredient())
        {
            if (player.HasIngredient())
            {
                //Place ingredient if able to use at the machine
                if (HasValidRecipe(player.GetIngredient().GetIngredientSO()))
                {
                    player.GetIngredient().SetIngredientParent(this);

                    brewingRecipeSO = GetBrewingRecipeSOWithInput(GetIngredient().GetIngredientSO());
                    brewingTimer = 0;
                    brewing = true;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = 0f
                    });
                }
            }
        }
        else
        {
            //Grab object if player is holding nothing
            if (!player.HasIngredient())
            {
                GetIngredient().SetIngredientParent(player);

                brewingTimer = 0;
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = 0f
                }); 
            }
        }  
    }

    private bool HasValidRecipe(IngredientSO inputIngredientSO)
    {
        BrewingRecipeSO brewingRecipeSO = GetBrewingRecipeSOWithInput(inputIngredientSO);
        return brewingRecipeSO != null;
    }

    private IngredientSO GetCoffeeGrind(IngredientSO inputIngredientSO)
    {
        BrewingRecipeSO brewingRecipeSO = GetBrewingRecipeSOWithInput(inputIngredientSO);
        if (brewingRecipeSO)
        {
            return brewingRecipeSO.output;
        }
        else
        {
            return null;
        }
    }

    private BrewingRecipeSO GetBrewingRecipeSOWithInput(IngredientSO inputIngredientSO)
    {
        foreach (BrewingRecipeSO brewingRecipeSO in brewingRecipeSOArray)
        {
            if (brewingRecipeSO.input == inputIngredientSO)
            {
                return brewingRecipeSO;
            }
        }
        return null;
    }
}

