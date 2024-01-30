using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomerReviewManager : Singleton<CustomerReviewManager>
{
    public Transform reviewsPanel;
    public Transform popOutReviewsPanel;
    public GameObject customerReviewPrefab;
    public List<GameObject> starPrefabs;
    public List<GameObject> starPrefabs2;
    public GameObject customerReview;
    public GameObject customerReview2;
    public float rPSpeed;
    public float rPArrivalThreshold;
    public float popOutReviewTime;
    private Vector3 originalReviewPosition;
    private Vector3 popOutReviewPosition;
    private bool reviewInProgress;

    private void Start()
    {
        originalReviewPosition = reviewsPanel.transform.position;
        popOutReviewPosition = popOutReviewsPanel.position;
    }

    public void ShowCustomerReview(CustomerBase customer)
    {
        if (reviewInProgress == false)
        {
            TextMeshProUGUI customerReviewText = customerReview.GetComponentInChildren<TextMeshProUGUI>();
            customerReview.GetComponent<CustomerReview>().GenerateReview(customer);
            customerReviewText.text = customerReview.GetComponent<CustomerReview>().ReviewText;
            UpdateStarRating(customerReview.GetComponent<CustomerReview>().ReviewScore, starPrefabs);
            StartCoroutine(MoveRP(popOutReviewPosition, originalReviewPosition));
        }
        else
        {
            TextMeshProUGUI customerReviewText = customerReview2.GetComponentInChildren<TextMeshProUGUI>();
            customerReview2.GetComponent<CustomerReview>().GenerateReview(customer);
            customerReviewText.text = customerReview2.GetComponent<CustomerReview>().ReviewText;
            UpdateStarRating(customerReview2.GetComponent<CustomerReview>().ReviewScore, starPrefabs2);
            StartCoroutine(WaitingForLastReview(popOutReviewPosition, originalReviewPosition));
        }
    }

    private IEnumerator MoveRP(Vector3 target, Vector3 start)
    {
        reviewInProgress = true;
        customerReview2.SetActive(false);
        float startTime = Time.time;

        // Move towards the target
        while (Time.time - startTime < popOutReviewTime)
        {
            float t = (Time.time - startTime) / popOutReviewTime * rPSpeed;
            reviewsPanel.transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        // Wait at the target for specified time
        yield return new WaitForSeconds(popOutReviewTime);

        startTime = Time.time;

        // Move back to the initial position
        while (Time.time - startTime < popOutReviewTime)
        {
            float t = (Time.time - startTime) / popOutReviewTime * rPSpeed;
            reviewsPanel.transform.position = Vector3.Lerp(target, start, t);
            yield return null;
        }
        reviewInProgress = false;
        customerReview2.SetActive(true);
    }

    private IEnumerator WaitingForLastReview(Vector3 target, Vector3 start)
    {
        Debug.Log("WaitingForLastReview started");

        // Wait until the boolean is true
        while (reviewInProgress)
        {
            // Add a small delay to avoid unnecessary CPU usage
            yield return new WaitForSeconds(0.5f);
        }

        // The boolean has flipped, perform your desired action here
        Debug.Log("Review Finished, starting the next...");
        customerReview.SetActive(false);
        float startTime = Time.time;

        // Move towards the target
        while (Time.time - startTime < popOutReviewTime)
        {
            float t = (Time.time - startTime) / popOutReviewTime * rPSpeed;
            reviewsPanel.transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        // Wait at the target for specified time
        yield return new WaitForSeconds(popOutReviewTime);

        startTime = Time.time;

        // Move back to the initial position
        while (Time.time - startTime < popOutReviewTime)
        {
            float t = (Time.time - startTime) / popOutReviewTime * rPSpeed;
            reviewsPanel.transform.position = Vector3.Lerp(target, start, t);
            yield return null;
        }
        customerReview.SetActive(true);
    }
    public void UpdateStarRating(int reviewScore, List<GameObject> starPrefabs)
    {
        // change stars color based on review (5 stars max)
        for (int i = 0; i < starPrefabs.Count; i++)
        {
            if (i >= reviewScore)
            {
                starPrefabs[i].GetComponent<Image>().color = Color.black;
            }
        }
    }
}

