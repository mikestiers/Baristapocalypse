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
    public GameObject starPrefab;
    [HideInInspector] public List<GameObject> reviewsList = new List<GameObject>();
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
            TextMeshProUGUI customerReviewText = reviewsList[0].GetComponentInChildren<TextMeshProUGUI>();
            CustomerReview customerReview = reviewsList[0].gameObject.GetComponent<CustomerReview>();
            customerReview.GenerateReview(customer);
            customerReviewText.text = customerReview.ReviewText;
            UpdateStarRating(customerReview.ReviewScore);
            reviewsList[0].SetActive(true);
            StartCoroutine(MoveRP(popOutReviewPosition, originalReviewPosition));
        }
        else
        {
            TextMeshProUGUI customerReviewText = reviewsList[1].GetComponentInChildren<TextMeshProUGUI>();
            CustomerReview customerReview = reviewsList[1].gameObject.GetComponent<CustomerReview>();
            customerReview.GenerateReview(customer);
            customerReviewText.text = customerReview.ReviewText;
            UpdateStarRating(customerReview.ReviewScore);
            StartCoroutine(WaitingForLastReview());
        }
    }

    private IEnumerator MoveRP(Vector3 target, Vector3 start)
    {
        reviewInProgress = true;
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
        Destroy(reviewsList[0]);
        reviewsList.RemoveAt(0);
        reviewInProgress = false;
    }

    private IEnumerator WaitingForLastReview()
    {
        Debug.Log("WaitingForLastReview started");

        // Wait until the boolean is true
        while (!reviewInProgress)
        {
            // Add a small delay to avoid unnecessary CPU usage
            yield return new WaitForSeconds(0.5f);
        }

        // The boolean has flipped, perform your desired action here
        Debug.Log("Review Finished, starting the next...");
        reviewsList[0].SetActive(true);
        StartCoroutine(MoveRP(popOutReviewPosition, originalReviewPosition));
    }
    public void UpdateStarRating(int reviewScore)
    {
        Transform starRating = reviewsList[0].GetComponentInChildren<GridLayoutGroup>().gameObject.transform;

        // Instantiate stars. 5 stars max
        for (int i = 1; i <= 5; i++)
        {
            if (i <= reviewScore)
            {
                GameObject star = Instantiate(starPrefab, starRating);
                Image starImage = star.GetComponent<Image>();
            }
            else
            {
                GameObject star = Instantiate(starPrefab, starRating);
                Image starImage = star.GetComponent<Image>();
                starImage.color = Color.red;
            }
        }
    }
}

