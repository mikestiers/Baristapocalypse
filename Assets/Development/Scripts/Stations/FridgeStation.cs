using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FridgeStation : BaseStation, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    [SerializeField] private IngredientSO[] ingredientListSO;
    [SerializeField] private ParticleSystem interactParticle;

    private IngredientSO currentIngredient;
    private int ingredientListSOIndex;

    private void Start()
    {
        ingredientListSOIndex = 0;
        currentIngredient = ingredientListSO[ingredientListSOIndex];  
    }

    public override void Interact(PlayerStateMachine player)
    {
        if (!HasIngredient())
        {   
            if (player.HasIngredient()) //check if player has ingredient
            {
                if (player.GetIngredient().CompareTag("Milk")) //check if player has milk ingredient
                {
                    player.GetIngredient().DestroyIngredient(); //destroy ingredient
                }
            }
            else
            {
                Ingredient.SpawnIngredient(currentIngredient, player);
                SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.interactStation);
                interactParticle.Play();
            }
        }
        else
        {

        }
    }

    public override void InteractAlt(PlayerStateMachine player)
    {
        if(ingredientListSOIndex >= ingredientListSO.Length-1)
        {
            ingredientListSOIndex = 0;
        }
        else
        {
            ingredientListSOIndex++;
        }
        currentIngredient = ingredientListSO[ingredientListSOIndex];
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = ingredientListSOIndex
        });
    }

}
