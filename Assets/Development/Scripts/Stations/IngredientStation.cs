using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class IngredientStation : BaseStation
{
    [SerializeField] private IngredientSO ingredientSO;
    [SerializeField] private ParticleSystem interactParticle;
    public TextMeshPro text;
    private string curText;

    public override void Interact(PlayerStateMachine player)
    {
        if (!HasIngredient())
        {
            if (!player.HasIngredient())
            {
                // Check if the player has already spawned 4 ingredients
                if (player.GetNumberOfIngredients() >= 4)
                {
                    Debug.Log("Max ingredients spawned!");
                    return;
                }

                Ingredient.SpawnIngredient(ingredientSO, player);
                curText = text.text;
                text.text = curText + "\n" + ingredientSO.ToString(); //Updates text above player with ingredient
                player.GetNumberOfIngredients();
                SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.interactStation);
                interactParticle.Play();

            }
        }
    }
}
