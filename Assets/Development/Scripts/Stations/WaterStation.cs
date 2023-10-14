using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WaterStation : BaseStation, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    [SerializeField] private IngredientSO[] ingredientListSO;
    [SerializeField] private ParticleSystem interactParticle;

    private IngredientSO currentIngredient;
    private int ingredientListSOIndex;

    //for slider

    [SerializeField] private Slider slidertemp;

    private void Start()
    {
        ingredientListSOIndex = 0;
        currentIngredient = ingredientListSO[ingredientListSOIndex];

        slidertemp.value = 0;
    }

    public override void Interact(PlayerController player)
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

    public override void InteractAlt(PlayerController player)
    {
        if (ingredientListSOIndex >= ingredientListSO.Length - 1)
        {
            ingredientListSOIndex = 0;
            slidertemp.value = 0;
        }
        else
        {
            ingredientListSOIndex++;
            slidertemp.value++;
        }
        currentIngredient = ingredientListSO[ingredientListSOIndex];
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = ingredientListSOIndex
        });
    }

}
