using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShiftEvaluationUI : MonoBehaviour
{
    private int previousCustomersServed;
    private int previousCustomersLeave;
    private int previousTipsAcquired;

    public float rPSpeed;
    public float rPArrivalThreshold;
    public float popOutReviewTime;
    private Vector3 originalEvaluationPosition;
    private Vector3 popOutEvaluationPosition;

    [Header("Pop Out Transforms")]
    public Transform EvaluationPanel;
    public Transform popOutEvaluationPanel;

    [Header("Text")]
    public TextMeshProUGUI customersServedValue;
    public TextMeshProUGUI customersLeaveValue;
    public TextMeshProUGUI tipsAcquiredValue;

    public List<Image> starImages;
    public List<GameObject> containers;

    // Start is called before the first frame update
    void Start()
    {
        previousCustomersLeave = 0;
        previousTipsAcquired = 0;
        previousCustomersServed = 0;

        originalEvaluationPosition = EvaluationPanel.transform.position;
        popOutEvaluationPosition = popOutEvaluationPanel.position;
    }

    public void Evaluate()
    {
        GameManager.Instance.isEvaluationOn = true;
        int customersServed = CustomerManager.Instance.GetCustomerServed() - previousCustomersServed;
        int customersLeave = CustomerManager.Instance.GetCustomerLeave() - previousCustomersLeave;
        int tipsAcquired = GameManager.Instance.moneySystem.GetCurrentMoney() - previousTipsAcquired;
        int currentRatings = Mathf.FloorToInt(GameManager.Instance.moneySystem.GetAverageReviewrating());

        customersServedValue.text = customersServed.ToString();
        customersLeaveValue.text = customersLeave.ToString();
        tipsAcquiredValue.text = tipsAcquired.ToString();

        UpdateStarRating(currentRatings);

        StartCoroutine(MoveEP(popOutEvaluationPosition, originalEvaluationPosition));
    }

    public void UpdatePreviousValues() 
    {
        previousCustomersServed = CustomerManager.Instance.GetCustomerServed();
        previousCustomersLeave = CustomerManager.Instance.GetCustomerLeave();
        previousTipsAcquired =GameManager.Instance.moneySystem.GetCurrentMoney();
    }

    private IEnumerator MoveEP(Vector3 target, Vector3 start)
    {
        float startTime = Time.time;
        StartCoroutine(ShowElements());

        // Move towards the target
        while (Time.time - startTime < popOutReviewTime)
        {
            float t = (Time.time - startTime) / popOutReviewTime * rPSpeed;
            EvaluationPanel.transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

       
    }

    private IEnumerator ShowElements()
    {
        for (int i = 0; i < containers.Count; i++) 
        {
            StartCoroutine(ActivateMessage(containers[i]));

            yield return new WaitForSeconds(.5f);
        }

        yield return new WaitForSeconds(8f);

        StartCoroutine(MoveBackEP(originalEvaluationPosition, popOutEvaluationPosition));
    }

    private IEnumerator MoveBackEP(Vector3 target, Vector3 start)
    {
        float startTime = Time.time;

        startTime = Time.time;

        // Move back to the initial position
        while (Time.time - startTime < popOutReviewTime)
        {
            float t = (Time.time - startTime) / popOutReviewTime * rPSpeed;
            EvaluationPanel.transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        yield return new WaitForSeconds(popOutReviewTime);

        UpdatePreviousValues();

        GameManager.Instance.isEvaluationOn = false;
    }

    //set
    public IEnumerator ActivateMessage(GameObject Container)
    {
        yield return new WaitForSeconds(2f);

        Container.SetActive(true);

        StartCoroutine(DeactivateContainer(Container));
    }

    public IEnumerator DeactivateContainer(GameObject Container)
    {
        yield return new WaitForSeconds(30f);

        Container.SetActive(false);
    }

    public void UpdateStarRating(int reviewScore)
    {
        // change stars color based on review (5 stars max)
        for (int i = 0; i < starImages.Count; i++)
        {
            if (i >= reviewScore)
            {
                starImages[i].color = Color.black;
            }
        }
    }
}
