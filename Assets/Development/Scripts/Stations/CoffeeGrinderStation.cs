using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoffeeGrinderStation : BaseStation, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    [SerializeField] private CoffeeGrindRecipeSO[] coffeeGrindSOArray;
    [SerializeField] private ParticleSystem interactParticle;

    private int grindProgress = 0;
    public override void Interact(PlayerStateMachine player)
    {
        if (!HasIngredient())
        {
            if (player.GetNumberOfIngredients() >= 1)
            {
                foreach (Transform holdPoint in player.ingredientHoldPoints)
                {
                    Ingredient ingredient = holdPoint.GetComponentInChildren<Ingredient>();
                    if (ingredient != null && HasValidRecipe(ingredient.GetIngredientSO()))
                    {
                        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.interactStation);
                        interactParticle.Play();
                        ingredient.SetIngredientParent(this);
                        grindProgress = 0;

                        CoffeeGrindRecipeSO coffeeGrindRecipeSO = GetCoffeeGrindRecipeSOWithInput(ingredient.GetIngredientSO());
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = (float)grindProgress / coffeeGrindRecipeSO.grindMax
                        });
                        break;
                    }
                }

            }
        }
        else
        {
            //Grab object if player is holding nothing
            if (!player.HasIngredient())
            {
                SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.interactStation);
                grindProgress = 0;

                CoffeeGrindRecipeSO coffeeGrindRecipeSO = GetCoffeeGrindRecipeSOWithInput(GetIngredient().GetIngredientSO());
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = 0f
                });

                GetIngredient().SetIngredientParent(player);
            }
        }
    }

    public override void InteractAlt(PlayerStateMachine player)
    {
        //Grind coffee if on table and valid input
        if (HasIngredient() && HasValidRecipe(GetIngredient().GetIngredientSO()))
        {
            grindProgress++;
            CoffeeGrindRecipeSO coffeeGrindRecipeSO = GetCoffeeGrindRecipeSOWithInput(GetIngredient().GetIngredientSO());
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = (float)grindProgress / coffeeGrindRecipeSO.grindMax
            });
            if (grindProgress >= coffeeGrindRecipeSO.grindMax)
            { 
                IngredientSO coffeeGrindSO = GetCoffeeGrind(GetIngredient().GetIngredientSO());
                GetIngredient().DestroyIngredient();
                Ingredient.SpawnIngredient(coffeeGrindSO, this);
            }
        }
    }

    private bool HasValidRecipe(IngredientSO inputIngredientSO)
    {
        CoffeeGrindRecipeSO coffeeGrindRecipeSO = GetCoffeeGrindRecipeSOWithInput(inputIngredientSO);
        return coffeeGrindRecipeSO != null;
    }

    private IngredientSO GetCoffeeGrind(IngredientSO inputIngredientSO)
    {
        CoffeeGrindRecipeSO coffeeGrindRecipeSO = GetCoffeeGrindRecipeSOWithInput(inputIngredientSO);
        if (coffeeGrindRecipeSO)
        {
            return coffeeGrindRecipeSO.output;
        }
        else
        {
            return null;
        }
    }

    private CoffeeGrindRecipeSO GetCoffeeGrindRecipeSOWithInput(IngredientSO inputIngredientSO)
    {
        foreach (CoffeeGrindRecipeSO coffeeGrindRecipeSO in coffeeGrindSOArray)
        {
            if (coffeeGrindRecipeSO.input == inputIngredientSO)
            {
                return coffeeGrindRecipeSO;
            }
        }
        return null;
    }
}
