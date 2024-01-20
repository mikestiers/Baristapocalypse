using UnityEngine;
using UnityEngine.UI;

public class OrderStatsSegments : MonoBehaviour
{
    public GameObject[] segments; // Change to dynamically find them
    public int cumulativeIngredientsValue;
    public int potentialIngredientValue;
    public int targetAttributeValue;
    public GameObject targetAttributeSelector;
    public GameObject potentialAttributeSelector;
    private int previousCumulativeIngredientsValue;

    private void Start()
    {
        // Ensure all segments to inactive
        foreach (var segment in segments)
        {
            segment.SetActive(false);
        }
        cumulativeIngredientsValue = 0;
    }

    private void Update()
    {
        UpdateSegmentColors(cumulativeIngredientsValue); // remove this later so it doesn't run every frame
    }

    public void UpdateSegmentColors(int cumulative)
    {
        // Reset all segments every tick for now just to make sure things are updating during testing
        foreach (var segment in segments)
        {
            segment.GetComponent<Image>().color = Color.green;
            Color segmentColor = segment.GetComponent<Image>().color;
            segmentColor.a = 1.0f;
            segment.SetActive(false);
        }

        if (cumulative >= segments.Length / 2)
        {
            Debug.LogError("Value is greater than the number of segments.");
            return;
        }

        if (targetAttributeValue < 0)
        {
            GameObject targetSegment = segments[(segments.Length / 2) + targetAttributeValue];
            SetTarget(targetSegment);
        }

        if (targetAttributeValue > 0)
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
            return; // add logic for zero value. need to update ui object to accept 0
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
        if (cumulativeIngredientsValue + potentialIngredientValue == targetAttributeValue)
            potentialSegment.GetComponent<Image>().color = Color.green;
        else if (cumulativeIngredientsValue + potentialIngredientValue != targetAttributeValue)
            potentialSegment.GetComponent<Image>().color = Color.clear;
        potentialSegment.SetActive(true);
        potentialAttributeSelector.transform.SetParent(potentialSegment.transform);
        potentialAttributeSelector.transform.position = potentialSegment.transform.position;
        potentialAttributeSelector.SetActive(true);
    }
}
