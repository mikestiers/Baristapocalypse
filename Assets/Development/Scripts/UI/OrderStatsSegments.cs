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
        ResetSegments();
    }

    public void ResetSegments()
    {
        foreach (var segment in segments)
        {
            //segment.GetComponent<Image>().color = Color.green;
            Color segmentColor = segment.GetComponent<Image>().color;
            segmentColor.a = 0.0f;
            segment.GetComponent<Image>().color = segmentColor;
            segment.SetActive(false);
        }
        Debug.Log("Resetting segments - how often is this happening");
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
            //segment.GetComponent<Image>().color = Color.green;
            Color segmentColor = segment.GetComponent<Image>().color;
            segmentColor.a = 0.0f;
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
        Color targetSegmentColor = targetSegment.GetComponent<Image>().color;
        targetSegmentColor.a = 1.0f;
        targetSegment.GetComponent<Image>().color = targetSegmentColor;
        targetSegment.SetActive(true);
        targetAttributeSelector.transform.SetParent(targetSegment.transform);
        targetAttributeSelector.transform.position = targetSegment.transform.position;
        targetAttributeSelector.SetActive(true);

        // difficulty range
        Color targetSegmentRangeColor = Color.green;
        targetSegmentColor.a = 1.0f;
        //if (CustomerManager.Instance.difficultySettings.GetDrinkThreshold() == 3)
        //{
        if (3 == 3)
        {
            segments[MapValue(targetAttributeValue) - 1].GetComponent<Image>().color = targetSegmentRangeColor;
            segments[MapValue(targetAttributeValue) + 1].GetComponent<Image>().color = targetSegmentRangeColor;
            segments[MapValue(targetAttributeValue) - 1].SetActive(true);
            segments[MapValue(targetAttributeValue) + 1].SetActive(true);
        }
        //}
    }

    private void SetPotential(GameObject potentialSegment)
    {
        int cumulativeValue = MapValue(cumulativeIngredientsValue);
        int potentialValue = MapValue(potentialIngredientValue);
        int targetValue = MapValue(targetAttributeValue);
        potentialSegment.SetActive(true);
        potentialAttributeSelector.transform.SetParent(potentialSegment.transform);
        potentialAttributeSelector.transform.position = potentialSegment.transform.position;
        potentialAttributeSelector.SetActive(true);
    }
}
