using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class ShiftEvaluationUI : NetworkBehaviour
{
    public static ShiftEvaluationUI Instance {  get; private set; }

    private NetworkVariable<int> previousCustomersServed = new NetworkVariable<int>();
    private NetworkVariable<int> previousCustomersLeave = new NetworkVariable<int>();
    private NetworkVariable<int> previousTipsAcquired = new NetworkVariable<int>();

    [SerializeField] private float transitionSpeed;
    [SerializeField] private float popOutReviewTime;
    [SerializeField] private RectTransform shiftEvaluationRectTransform;

    [SerializeField] private Vector2 startPos;
    [SerializeField] private Vector2 endPos;

    [Header("Text")]
    public TextMeshProUGUI customersServedValue;
    public TextMeshProUGUI customersLeaveValue;
    public TextMeshProUGUI tipsAcquiredValue;

    public List<Image> starImages;
    public List<GameObject> containers;

    public delegate void ShiftEvaluationHandler();
    public static event ShiftEvaluationHandler OnShiftEvaluation;

    private void Awake()
    {
        Instance = this;
        OnShiftEvaluation += HandleShiftEvaluation;
    }

    public override void OnDestroy()
    {
        OnShiftEvaluation -= HandleShiftEvaluation;
    }

    // Start is called before the first frame update
    void Start()
    {
        previousCustomersLeave.Value = 0;
        previousTipsAcquired.Value = 0;
        previousCustomersServed.Value = 0;
    }

    public void HandleShiftEvaluation()
    {
        HandleShiftEvaluationClientRpc();
    }

    [ClientRpc]
    private void HandleShiftEvaluationClientRpc()
    {
        Evaluate();
    }

    public void Evaluate()
    {
        GameManager.Instance.isEvaluationOn = true;
        int customersServed = CustomerManager.Instance.GetCustomerServed() - previousCustomersServed.Value;
        int customersLeave = CustomerManager.Instance.GetCustomerLeave() - previousCustomersLeave.Value;
        int tipsAcquired = GameManager.Instance.moneySystem.GetCurrentMoney() - previousTipsAcquired.Value;
        int currentRatings = Mathf.FloorToInt(GameManager.Instance.moneySystem.GetAverageReviewrating());

        customersServedValue.text = customersServed.ToString();
        customersLeaveValue.text = customersLeave.ToString();
        tipsAcquiredValue.text = tipsAcquired.ToString();

        UpdateStarRating(currentRatings);

        StartCoroutine(MoveEP());
    }

    public void UpdatePreviousValues() 
    {
        previousCustomersServed.Value = CustomerManager.Instance.GetCustomerServed();
        previousCustomersLeave.Value = CustomerManager.Instance.GetCustomerLeave();
        previousTipsAcquired.Value = GameManager.Instance.moneySystem.GetCurrentMoney();
    }

    private IEnumerator MoveEP()
    {
        float startTime = 0f;

        // Move towards the target
        while (startTime < transitionSpeed)
        {
            float t = startTime / transitionSpeed;
            shiftEvaluationRectTransform.anchoredPosition = Vector3.Lerp(startPos, endPos, t);
            startTime += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(ShowElements());
    }

    private IEnumerator ShowElements()
    {
        for (int i = 0; i < containers.Count; i++) 
        {
            StartCoroutine(ActivateMessage(containers[i]));

            yield return new WaitForSeconds(.5f);
        }

        yield return new WaitForSeconds(6f);

        StartCoroutine(MoveBackEP());
    }

    private IEnumerator MoveBackEP()
    {
        float startTime = 0;

        // Move back to the initial position
        while (startTime < transitionSpeed)
        {
            float t = startTime / transitionSpeed;
            shiftEvaluationRectTransform.anchoredPosition = Vector3.Lerp(endPos, startPos, t);
            startTime += Time.deltaTime;
            yield return null;
        }

        UpdatePreviousValues();

        GameManager.Instance.isEvaluationOn = false;
    }

    //set
    public IEnumerator ActivateMessage(GameObject Container)
    {
        yield return new WaitForSeconds(1f);

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
