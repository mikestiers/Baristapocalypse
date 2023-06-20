using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static CoffeeGrinderStation;

public class BrewingStation : BaseStation, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    [SerializeField] private List<IngredientSO> ingredientSOList = new List<IngredientSO>();
    [SerializeField] private ParticleSystem interactParticle;
    private List<String> validIngredientTagList = new List<String>();
    
    private int numIngredientsNeeded = 4;

    private float brewingTimer;
    [SerializeField] private BrewingRecipeSO brewingRecipeSO;
    private bool brewing;

    private void Awake()
    {
        validIngredientTagList.Add("CoffeeGrind");
        validIngredientTagList.Add("Milk");
        validIngredientTagList.Add("Sweetener");
        validIngredientTagList.Add("BioMatter");
    }

    private void Update()
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
                Ingredient.SpawnIngredient(brewingRecipeSO.output, this);

                CoffeeAttributes coffeeAttributes = this.GetIngredient().GetComponent<CoffeeAttributes>();
                foreach(IngredientSO ingredient in ingredientSOList)
                {
                    coffeeAttributes.AddSweetness(ingredient.sweetness);
                    coffeeAttributes.AddBitterness(ingredient.bitterness);
                    coffeeAttributes.AddStrength(ingredient.strength);
                    coffeeAttributes.AddTemperature(ingredient.temperature);
                    coffeeAttributes.AddSpiciness(ingredient.spiciness);
                }
                ingredientSOList.Clear();
                brewing = false;
            }
        }
        
    }

    public override void Interact(PlayerStateMachine player)
    {
        if (player.HasIngredient())
        {
            if (TryAddIngredient(player.GetIngredient().GetIngredientSO())){
                SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.interactStation);
                interactParticle.Play();
                player.GetIngredient().DestroyIngredient();
                if(ingredientSOList.Count >= numIngredientsNeeded)
                {
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
            if (HasIngredient())
            {
                GetIngredient().SetIngredientParent(player);
            }
        }
    }

    private bool TryAddIngredient(IngredientSO ingredientSO)
    {
        if(!ValidIngredient(ingredientSO.objectTag))
        {
            return false;
        }

        foreach(IngredientSO ingredient in ingredientSOList)
        {
            if (ingredient.objectTag == ingredientSO.objectTag){
                return false;
            } 
        }

        ingredientSOList.Add(ingredientSO);
        return true;
    }

    private bool ValidIngredient(String ingredientTag)
    {
        if (validIngredientTagList.Contains(ingredientTag))
        {
            return true;
        }
        return false;
    }
}

