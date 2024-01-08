using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IngredientStation_2 : BaseStation, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    [SerializeField] private IngredientSO[] ingredientListSO;
    [SerializeField] private ParticleSystem interactParticle;
    [SerializeField] private TextMeshPro currentIngredienIndicator;

    private IngredientSO currentIngredient;
    private int ingredientListSOIndex;

    private void Start()
    {
        ingredientListSOIndex = 0;
        currentIngredient = ingredientListSO[ingredientListSOIndex];
        currentIngredienIndicator.text = currentIngredient.name;
    }

    public override void Interact(PlayerController player)
    {
        if (!HasIngredient())
        {
            if (player.HasIngredient()) //check if player has ingredient
            {
                if (player.GetIngredient().CompareTag("Milk")) //check if player has milk ingredient
                {
                    Ingredient.DestroyIngredient(player.GetIngredient()); //destroy ingredient
                }
            }
            else
            {
                Ingredient.SpawnIngredient(currentIngredient, player);
                player.GetNumberOfIngredients();
                SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.interactStation);
                interactParticle.Play();
            }
        }
        else
        {

        }
    }

    public override void InteractAlt(PlayerController player)
    {
        if (ingredientListSOIndex >= ingredientListSO.Length - 1)
        {
            ingredientListSOIndex = 0;
        }
        else
        {
            ingredientListSOIndex++;
        }
        currentIngredient = ingredientListSO[ingredientListSOIndex];
        currentIngredienIndicator.text = currentIngredient.name;
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = ingredientListSOIndex
        });
    }
}
