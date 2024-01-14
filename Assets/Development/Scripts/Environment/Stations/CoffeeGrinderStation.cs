using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoffeeGrinderStation : BaseStation, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    [SerializeField] private CoffeeGrindRecipeSO[] coffeeGrindSOArray;
    [SerializeField] private ParticleSystem interactParticle;

    private int grindProgress = 0;
    public override void Interact(PlayerController player)
    {
        if (!HasIngredient())
        {
            if (player.HasIngredient())
            {
                if (HasValidRecipe(player.GetIngredient().GetIngredientSO()))
                {
                    Ingredient ingredient = player.GetIngredient();
                    player.GetIngredient().SetIngredientParent(this);
     
                    InteractLogicPlaceObjectOnGrinderServerRpc();
                }
            }
            /*if (player.GetNumberOfIngredients() >= 1)
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
            }*/
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

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnGrinderServerRpc()
    {
        InteractLogicPlaceObjectOnGrinderClientRpc();
    }

    [ClientRpc]
    private void InteractLogicPlaceObjectOnGrinderClientRpc()
    {
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.interactStation);
        interactParticle.Play();
        grindProgress = 0;

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = 0f
        });
    }

    public override void InteractAlt(PlayerController player)
    {
        //Grind coffee if on table and valid input
        if (HasIngredient() && HasValidRecipe(GetIngredient().GetIngredientSO()))
        {
            GrindBeanServerRpc();
            TestGrindProgressDoneServerRpc();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void GrindBeanServerRpc()
    {
        GrindBeanClientRpc();
    }

    [ClientRpc]
    private void GrindBeanClientRpc()
    {
        grindProgress++;
        CoffeeGrindRecipeSO coffeeGrindRecipeSO = GetCoffeeGrindRecipeSOWithInput(GetIngredient().GetIngredientSO());
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = (float)grindProgress / coffeeGrindRecipeSO.grindMax
        });
        
    }

    [ServerRpc(RequireOwnership = false)]
    private void TestGrindProgressDoneServerRpc()
    {
        CoffeeGrindRecipeSO coffeeGrindRecipeSO = GetCoffeeGrindRecipeSOWithInput(GetIngredient().GetIngredientSO());

        if (grindProgress >= coffeeGrindRecipeSO.grindMax)
        {
            IngredientSO coffeeGrindSO = GetCoffeeGrind(GetIngredient().GetIngredientSO());

            Ingredient.DestroyIngredient(GetIngredient());
            Ingredient.SpawnIngredient(coffeeGrindSO, this);
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