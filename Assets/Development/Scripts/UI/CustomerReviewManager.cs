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
    private bool reviewInProgress;
    private TextMeshProUGUI customerReviewText;
    public AudioClip popOutSound;

    // Event to receive Supervisor Message
    public delegate void CustomerReviewHandler(CustomerBase customer);
    public static event CustomerReviewHandler OnCustomerReviewReceived;


    private void Awake()
    {
        Instance = this;
        OnCustomerReviewReceived += HandleCustomerReview;
    }

    public override void OnDestroy()
    {
        OnCustomerReviewReceived -= HandleCustomerReview;
    }

    private void HandleCustomerReview(CustomerBase customer)
    {
       ShowCustomerReview(customer);
    }

    public void CustomerReviewEvent(CustomerBase customer)
    {
        OnCustomerReviewReceived?.Invoke(customer);
    }

    public void ShowCustomerReview(CustomerBase customer)
    {
        if(reviewInProgress == false)
        {
            customerReviewText = customerReview.GetComponentInChildren<TextMeshProUGUI>();
            customerReview.GetComponent<CustomerReview>().GenerateReview(customer);
        }
        else
        {
            customerReviewText = customerReview2.GetComponentInChildren<TextMeshProUGUI>();
            customerReview2.GetComponent<CustomerReview>().GenerateReview(customer);
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
            SoundManager.Instance.PlayOneShot(popOutSound);
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
            if (i >= reviewScore)
            {
                starPrefabs[i].GetComponent<Image>().color = Color.black;
            }
        }
    }
}

