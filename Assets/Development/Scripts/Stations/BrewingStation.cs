using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static CoffeeGrinderStation;

public class BrewingStation : BaseStation, IHasProgress, IHasMinigameTiming
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<IHasMinigameTiming.OnMinigameTimingEventArgs> OnMinigameTimingStarted;

    [SerializeField] private List<IngredientSO> ingredientSOList = new List<IngredientSO>();
    [SerializeField] private ParticleSystem interactParticle;
    private List<String> validIngredientTagList = new List<String>();
    
    private int numIngredientsNeeded = 4;

    private float brewingTimer;
    [SerializeField] private BrewingRecipeSO brewingRecipeSO;
    private bool brewing;
    private float minigameTimer;
    private float maxMinigameTimer = 4.0f;
    private bool minigameTiming = false;

    private void Awake()
    {
        validIngredientTagList.Add("CoffeeGrind");
        validIngredientTagList.Add("Milk");
        validIngredientTagList.Add("Sweetener");
        validIngredientTagList.Add("BioMatter");
    }

    private void Update()
    {
        /*
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
                minigameTiming = true;
            }
        }*/
        if (minigameTiming)
        {
            minigameTimer += Time.deltaTime;
            OnMinigameTimingStarted?.Invoke(this, new IHasMinigameTiming.OnMinigameTimingEventArgs
            {
                minigameTimingNormalized = (float)minigameTimer / maxMinigameTimer
            });
        }
        
    }

    public override void Interact(PlayerStateMachine player)
    {
        if (player.GetNumberOfIngredients() >= 1)
        {
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.interactStation);
            interactParticle.Play();

            for (int i = 0; i < player.ingredientHoldPoints.Length; i++)
            {
                Transform holdPoint = player.ingredientHoldPoints[i];
                if (holdPoint.childCount > 0 )
                {
                    Ingredient ingredient = holdPoint.GetComponentInChildren<Ingredient>();
                    if (TryAddIngredient(ingredient.GetIngredientSO()))
                    {
                        ingredient.DestroyIngredient();
                        if (ingredientSOList.Count >= numIngredientsNeeded)
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
            }
        }
        else
        {
            if (HasIngredient())
            {
                GetIngredient().SetIngredientParent(player);
            }
        }

        minigameTiming = true;
        minigameTimer = 0;
        Debug.Log("ingredientSOList.Count :" + ingredientSOList.Count);
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

