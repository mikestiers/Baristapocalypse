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
    public GameObject targetAttributeSelector;
    public GameObject potentialAttributeSelector;
    private int previousCumulativeIngredientsValue;

    private void Start()
    {
        // Reset all segments to inactive
        foreach (var segment in segments)
        {
            segment.SetActive(false);
        }
    }

    private void Update()
    {
        UpdateSegmentColors(cumulativeIngredientsValue); // remove this later
    }

    public void UpdateSegmentColors(int cumulative)
    {
        // Reset all segments to the default color (e.g., white)
        foreach (var segment in segments)
        {
            segment.GetComponent<Image>().color = Color.green;
            Color segmentColor = segment.GetComponent<Image>().color;
            segmentColor.a = 1.0f;
            segment.SetActive(false);
        }

        //if (cumulative == 0)
        //{

        //}

        if (cumulative >= segments.Length / 2)
        {
            Debug.LogError("Value is greater than the number of segments.");
            return;
        }

        if (targetAttributeValue < 0 && cumulative != targetAttributeValue)
        {
            GameObject targetSegment = segments[(segments.Length / 2) + targetAttributeValue];
            SetTarget(targetSegment);
        }

        if (targetAttributeValue > 0 && cumulative != targetAttributeValue)
        {
            GameObject targetSegment = segments[(segments.Length / 2 - 1) + targetAttributeValue];
            SetTarget(targetSegment);
        }

        if (potentialIngredientValue < 0)
        {
            int startIndex = segments.Length / 2;
            int endIndex = startIndex + cumulative;
            SetPotential(segments[endIndex + potentialIngredientValue]);
        }

        if (potentialIngredientValue > 0)
        {
            int startIndex = segments.Length / 2 - 1;
            int endIndex = startIndex + cumulative;
            SetPotential(segments[endIndex + potentialIngredientValue]);
        }

        if (potentialIngredientValue == 0)
        {
            potentialAttributeSelector.SetActive(false);
        }

        //if (cumulative < 0)
        //{
        //    int startIndex = segments.Length / 2 - 1;
        //    int endIndex = startIndex + cumulative;
        //    for (int i = startIndex; i > endIndex; i--)
        //    {
        //        segments[i].GetComponent<Image>().color = Color.blue;
        //    }

        //    if (potentialIngredientValue < 0)
        //    {
        //        for (int i = endIndex; i > endIndex + potentialIngredientValue; i--)
        //        {
        //            segments[i].GetComponent<Image>().color = Color.red;
        //        }
        //    }

        //    if (potentialIngredientValue > 0)
        //    {
        //        for (int i = endIndex; i < endIndex + potentialIngredientValue; i++)
        //        {
        //            segments[i].GetComponent<Image>().color = Color.red;
        //        }
        //    }

        //}
        //if (cumulative > 0)
        //{
        //    int startIndex = segments.Length / 2 - 1;
        //    int endIndex = startIndex + cumulative;
        //    segments[endIndex].GetComponent<Image>().color = Color.blue;
        //    segments[endIndex].SetActive(true);
        //    //for (int i = startIndex; i < endIndex; i++)
        //    //{
        //    //    segments[i].GetComponent<Image>().color = Color.blue;
        //    //}

        //    //if (potentialIngredientValue < 0)
        //    //{
        //    //    Debug.Log(endIndex);
        //    //    for (int i = endIndex; i > endIndex + potentialIngredientValue; i--)
        //    //    {
        //    //        Debug.Log(i + ": " + segments[i]);
        //    //        segments[i - 2].GetComponent<Image>().color = Color.red;
        //    //    }
        //    //}

        //    //if (potentialIngredientValue > 0)
        //    //{
        //    //    for (int i = endIndex; i < endIndex + potentialIngredientValue; i++)
        //    //    {
        //    //        segments[i].GetComponent<Image>().color = Color.red;
        //    //    }
        //    //}
        //}

        //if (targetAttributeValue < 0 && cumulative != targetAttributeValue)
        //{
        //    GameObject targetSegment = segments[(segments.Length / 2) + targetAttributeValue];
        //    targetSegment.SetActive(true);  // GetComponent<Image>().color = Color.green;
        //                                    //targetAttributeSelector.pare
        //}

        //if (targetAttributeValue > 0 && cumulative != targetAttributeValue)
        //{
        //    GameObject targetSegment = segments[(segments.Length / 2 - 1) + targetAttributeValue];
        //    targetSegment.SetActive(true);  // GetComponent<Image>().color = Color.green;
        //}
    }

    private void SetTarget(GameObject targetSegment)
    {
        targetSegment.GetComponent<Image>().color = Color.green;
        targetSegment.SetActive(true);
        targetAttributeSelector.transform.SetParent(targetSegment.transform);
        targetAttributeSelector.transform.position = targetSegment.transform.position;
        targetAttributeSelector.SetActive(true);
    }

    private void SetPotential(GameObject potentialSegment)
    {
        potentialSegment.GetComponent<Image>().color = Color.clear;
        potentialSegment.SetActive(true);
        potentialAttributeSelector.transform.SetParent(potentialSegment.transform);
        potentialAttributeSelector.transform.position = potentialSegment.transform.position;
        potentialAttributeSelector.SetActive(true);
    }

    private void SetCumulative(GameObject cumulativeSegment)
    {

    }
}
