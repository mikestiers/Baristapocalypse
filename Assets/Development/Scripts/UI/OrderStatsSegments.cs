using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.HableCurve;

public class OrderStatsSegments : MonoBehaviour
{
    public GameObject[] segments; // Assign your segment objects in the inspector
    public int cumulativeIngredientsValue;
    public int potentialIngredientValue;
    public int targetAttributeValue;
    private int previousCumulativeIngredientsValue;

    private void Start()
    {
        // Reset all segments to the default color (e.g., white)
        foreach (var segment in segments)
        {
            segment.GetComponent<Image>().color = Color.white;
        }
    }

    private void Update()
    {
        // run updatesegmentcolors when cumulativeIngredientsValue changes
        
        //if (cumulativeIngredientsValue != previousCumulativeIngredientsValue)
        //{
        //    UpdateSegmentColors(cumulativeIngredientsValue);
        //    previousCumulativeIngredientsValue = cumulativeIngredientsValue;
        //}

        UpdateSegmentColors(cumulativeIngredientsValue); // remove this later
    }

    public void UpdateSegmentColors(int cumulative)
    {
        // Reset all segments to the default color (e.g., white)
        foreach (var segment in segments)
        {
            segment.GetComponent<Image>().color = Color.white;
        }

        // Color the segments blue up to the value
        if (cumulative == 0)
        {
            if (targetAttributeValue < 0 && cumulative != targetAttributeValue)
                segments[(segments.Length / 2) + targetAttributeValue].GetComponent<Image>().color = Color.green;
            if (targetAttributeValue > 0 && cumulative != targetAttributeValue)
                segments[(segments.Length / 2 - 1) + targetAttributeValue].GetComponent<Image>().color = Color.green;
            int startIndex = segments.Length / 2 - 1;
            int endIndex = startIndex + cumulative;
            if (potentialIngredientValue < 0)
            {
                for (int i = endIndex; i > endIndex + potentialIngredientValue; i--)
                {
                    segments[i].GetComponent<Image>().color = Color.red;
                }
            }

            if (potentialIngredientValue > 0)
            {
                for (int i = endIndex; i < endIndex + potentialIngredientValue; i++)
                {
                    segments[i].GetComponent<Image>().color = Color.red;
                }
            }
            return;
        }

        if (cumulative >= segments.Length / 2)
        {
            Debug.LogError("Value is greater than the number of segments.");
            return;
        }

        if (cumulative < 0)
        {
            int startIndex = segments.Length / 2 - 1;
            int endIndex = startIndex + cumulative;
            for (int i = startIndex; i > endIndex; i--)
            {
                segments[i].GetComponent<Image>().color = Color.blue;
            }

            if (potentialIngredientValue < 0)
            {
                for (int i = endIndex; i > endIndex + potentialIngredientValue; i--)
                {
                    segments[i].GetComponent<Image>().color = Color.red;
                }
            }

            if (potentialIngredientValue > 0)
            {
                for (int i = endIndex; i < endIndex + potentialIngredientValue; i++)
                {
                    segments[i].GetComponent<Image>().color = Color.red;
                }
            }

        }
        if (cumulative > 0)
        {
            int startIndex = segments.Length / 2;
            int endIndex = startIndex + cumulative;
            for (int i = startIndex; i < endIndex; i++)
            {
                segments[i].GetComponent<Image>().color = Color.blue;
            }

            if (potentialIngredientValue < 0)
            {
                Debug.Log(endIndex);
                for (int i = endIndex; i > endIndex + potentialIngredientValue; i--)
                {
                    Debug.Log(i + ": " + segments[i]);
                    segments[i - 2].GetComponent<Image>().color = Color.red;
                }
            }

            if (potentialIngredientValue > 0)
            {
                for (int i = endIndex; i < endIndex + potentialIngredientValue; i++)
                {
                    segments[i].GetComponent<Image>().color = Color.red;
                }
            }
        }

        if (targetAttributeValue < 0 && cumulative != targetAttributeValue)
            segments[(segments.Length / 2) + targetAttributeValue].GetComponent<Image>().color = Color.green;
        if (targetAttributeValue > 0 && cumulative != targetAttributeValue)
            segments[(segments.Length / 2 -1) + targetAttributeValue].GetComponent<Image>().color = Color.green;
    }
}
