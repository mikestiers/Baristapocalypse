using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.HableCurve;

public class OrderStatsSegments : MonoBehaviour
{
    public GameObject[] segments; // Change to dynamically find them
    public int cumulativeIngredientsValue;
    public int potentialIngredientValue;
    public int targetAttributeValue;
    public GameObject targetAttributeSelector;
    public GameObject potentialAttributeSelector;

    private void Start()
    {
        Reset();
    }

    public void Reset()
    {
        foreach (var segment in segments)
        {
            segment.GetComponent<Image>().color = Color.green;
            Color segmentColor = segment.GetComponent<Image>().color;
            segmentColor.a = 1.0f;
            segment.GetComponent<Image>().color = segmentColor;
            segment.SetActive(false);
        }
    }

    private void Update()
    {
        // Make this not be required every frame
        int targetValue = MapValue(targetAttributeValue);
        GameObject targetSegment = segments[targetValue];
        SetTarget(targetSegment);
    }

    public void UpdateSegmentColors(int cumulative)
    {
        // Reset all segments every time the segments are updated to clear any invalid colors
        foreach (var segment in segments)
        {
            segment.GetComponent<Image>().color = Color.green;
            Color segmentColor = segment.GetComponent<Image>().color;
            segmentColor.a = 1.0f;
            segment.SetActive(false);
        }

        int potentialValue = MapValue(potentialIngredientValue);
        SetPotential(segments[potentialValue]);
    }

    // Mapping -7 to +7 to 0 to 14
    public int MapValue(int originalValue)
    {
        // Assuming originalValue is in the range of -7 to +7
        return originalValue + 7;
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
        {
            potentialSegment.GetComponent<Image>().color = Color.green;
        }

        else if (cumulativeIngredientsValue + potentialIngredientValue != targetAttributeValue)
        {
            Reset();
            potentialSegment.GetComponent<Image>().color = Color.clear;
        }
        potentialSegment.SetActive(true);
        potentialAttributeSelector.transform.SetParent(potentialSegment.transform);
        potentialAttributeSelector.transform.position = potentialSegment.transform.position;
        potentialAttributeSelector.SetActive(true);
    }
}
