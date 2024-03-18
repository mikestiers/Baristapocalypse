using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class CustomerReviewManager : NetworkBehaviour
{
    public static CustomerReviewManager Instance { get; private set; }

    [SerializeField] private RectTransform customerReviewWindowTransform;
    [SerializeField] private Vector2 startPos;
    [SerializeField] private Vector2 finalPos;

    private float transitionSpeed = 0.5f;

    public GameObject customerReviewPrefab;
    public List<GameObject> starPrefabs;
    public List<GameObject> starPrefabs2;
    public GameObject customerReview;
    public GameObject customerReview2;
    public float rPSpeed;
    public float rPArrivalThreshold;
    public float popOutReviewTime;
    public bool gameOverBool;
    private bool reviewInProgress;
    private TextMeshProUGUI customerReviewText;

    // Event to receive Supervisor Message
    public delegate void CustomerReviewHandler(CustomerBase customer);
    public static event CustomerReviewHandler OnCustomerReviewReceived;

    public delegate void OneStarCustomerReviewHandler(CustomerBase customer, float starAmount);
    public static event OneStarCustomerReviewHandler OnOneStarCustomerReviewReceived;
    [HideInInspector] public int reviewScore { get; private set; }


    private void Awake()
    {
        Instance = this;
        OnCustomerReviewReceived += HandleCustomerReview;
        OnOneStarCustomerReviewReceived += OneStarHandleCustomerReview;
    }

    public override void OnDestroy()
    {
        OnCustomerReviewReceived -= HandleCustomerReview;
        OnOneStarCustomerReviewReceived -= OneStarHandleCustomerReview;
    }

    private void HandleCustomerReview(CustomerBase customer)
    {
       ShowCustomerReview(customer);
    }
    private void OneStarHandleCustomerReview(CustomerBase customer, float starAmount)
    {
        ShowCustomerReview(customer, starAmount);
    }

    public void CustomerReviewEvent(CustomerBase customer)
    {
        OnCustomerReviewReceived?.Invoke(customer);
    }
    public void CustomerReviewEvent(CustomerBase customer, float starAmount)
    {
        OnOneStarCustomerReviewReceived?.Invoke(customer, starAmount);
    }

    public void ShowCustomerReview(CustomerBase customer, float starAmount)
    {
        if (gameOverBool == true) 
            return;

        if (reviewInProgress == false)
        {
            customerReviewText = customerReview.GetComponentInChildren<TextMeshProUGUI>();
            CustomerReview customerReviewInstance = customerReview.GetComponent<CustomerReview>();
            customerReviewInstance.GenerateReview(customer, starAmount);
            reviewScore = customerReviewInstance.ReviewScore;
        }
        else
        {
            customerReviewText = customerReview2.GetComponentInChildren<TextMeshProUGUI>();
            CustomerReview customerReviewInstance = customerReview2.GetComponent<CustomerReview>();
            customerReviewInstance.GenerateReview(customer, starAmount);
            reviewScore = customerReviewInstance.ReviewScore;
        }

        CustomerReviewDisplayClientRpc();
    }

    public void ShowCustomerReview(CustomerBase customer)
    {
        if(reviewInProgress == false)
        {
            customerReviewText = customerReview.GetComponentInChildren<TextMeshProUGUI>();
            CustomerReview customerReviewInstance = customerReview.GetComponent<CustomerReview>();
            customerReviewInstance.GenerateReview(customer);
            reviewScore = customerReviewInstance.ReviewScore;
        }
        else
        {
            customerReviewText = customerReview2.GetComponentInChildren<TextMeshProUGUI>();
            CustomerReview customerReviewInstance = customerReview2.GetComponent<CustomerReview>();
            customerReviewInstance.GenerateReview(customer);
            reviewScore = customerReviewInstance.ReviewScore;
        }
        
        CustomerReviewDisplayClientRpc();
    }

    [ClientRpc]
    private void CustomerReviewDisplayClientRpc()
    {
        CustomerReviewDisplay();
    }

    private void CustomerReviewDisplay()
    {
        if (TutorialManager.Instance != null && TutorialManager.Instance.tutorialEnabled && !TutorialManager.Instance.firstDrinkDelivered)
            TutorialManager.Instance.FirstDrinkDelivered();

        if (reviewInProgress == false)
        {
            customerReviewText.text = customerReview.GetComponent<CustomerReview>().ReviewText;
            UpdateStarRating(customerReview.GetComponent<CustomerReview>().ReviewScore, starPrefabs);
            StartCoroutine(MoveRP());
        }
        else
        {  
            customerReviewText.text = customerReview2.GetComponent<CustomerReview>().ReviewText;
            UpdateStarRating(customerReview2.GetComponent<CustomerReview>().ReviewScore, starPrefabs2);
            StartCoroutine(WaitingForLastReview());
        }
    }

    private IEnumerator MoveRP()
    {
        reviewInProgress = true;
        customerReview2.SetActive(false);
        float startTime = 0f;

        // Move towards the target
        while (startTime < transitionSpeed)
        {
            float t = startTime / transitionSpeed;
            customerReviewWindowTransform.anchoredPosition = Vector3.Lerp(startPos, finalPos, t);
            startTime += Time.deltaTime;
            yield return null;
        }

        reviewInProgress = false;

        StartCoroutine(ShowElements());
    }

    private IEnumerator WaitingForLastReview()
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
        float startTime = 0f;

        // Move towards the target
        while (startTime < transitionSpeed)
        {
            float t = startTime / transitionSpeed;
            customerReviewWindowTransform.anchoredPosition = Vector3.Lerp(startPos, finalPos, t);
            startTime += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(ShowElements());
        customerReview.SetActive(true);
    }

    private IEnumerator ShowElements()
    {
        yield return new WaitForSeconds(popOutReviewTime);

        StartCoroutine(MoveBackEP());
    }

    private IEnumerator MoveBackEP()
    {
        float startTime = 0;

        while (startTime < transitionSpeed)
        {
            float t = startTime / transitionSpeed;
            customerReviewWindowTransform.anchoredPosition = Vector3.Lerp(finalPos, startPos, t);
            startTime += Time.deltaTime;
            yield return null;
        }
    }

    public void UpdateStarRating(int reviewScore, List<GameObject> starPrefabs)
    {
        // change stars color based on review (5 stars max)
        for (int i = 0; i < starPrefabs.Count; i++)
        {
            starPrefabs[i].GetComponent<Image>().color = Color.white;
            if (i >= reviewScore)
            {
                starPrefabs[i].GetComponent<Image>().color = Color.black;
            }
        }
    }
}

