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
        //targetAttributeValue = 0;
        //cumulativeIngredientsValue = 0;
    }

    private void Update()
    {
        // Make thhis not be required every frame
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

        if (targetAttributeValue == 0)
        {
            targetAttributeSelector.transform.SetParent(gameObject.transform);
            targetAttributeSelector.transform.position = gameObject.transform.position;
            targetAttributeSelector.SetActive(false);
        }
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

        if (cumulative >= segments.Length / 2)
        {
            Debug.LogError("Value is greater than the number of segments.");
            return;
        }

        if (potentialIngredientValue < 0)
        {
            int startIndex = segments.Length / 2;
            int endIndex = startIndex + cumulative;
            SetPotential(segments[endIndex]);
        }

        if (potentialIngredientValue > 0)
        {
            int startIndex = segments.Length / 2 - 1;
            int endIndex = startIndex + cumulative;
            SetPotential(segments[endIndex]);
        }

        // add logic for zero value. need to update ui object to accept 0
        if (potentialIngredientValue == 0)
            potentialAttributeSelector.SetActive(false);
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
            potentialSegment.GetComponent<Image>().color = Color.blue;
        }
        potentialSegment.SetActive(true);
        potentialAttributeSelector.transform.SetParent(potentialSegment.transform);
        potentialAttributeSelector.transform.position = potentialSegment.transform.position;
        potentialAttributeSelector.SetActive(true);
    }
}
