using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class AISupervisor : NetworkBehaviour
{
    public static AISupervisor Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI supervisorMessageText;
    private string stringTest = "There's no story in video games";

    [SerializeField] private float transitionSpeed;
    [SerializeField] private float popOutReviewTime;
    [SerializeField] private RectTransform startFeedbackWindowTransform;

    [SerializeField] private Vector2 startpos;
    [SerializeField] private Vector2 finalpos;

    // Event to receive Supervisor Message
    public delegate void FeedbackMessageHandler(string feedbackMessage);
    public static event FeedbackMessageHandler OnFeedbackMessageReceived;

    private void Awake()
    {
        Instance = this;
        OnFeedbackMessageReceived += HandleFeedbackMessage;
    }

    public override void OnDestroy()
    {
        OnFeedbackMessageReceived -= HandleFeedbackMessage;
    }

    void Update()
    {
        if (!IsServer)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            SupervisorMessageToDisplayEvent(stringTest);
        }
    }

    private void HandleFeedbackMessage(string feedbackMessage)
    {
        SupervisorMessageToDisplay(feedbackMessage);
    }

    private void SupervisorMessageToDisplayEvent(string feedbackMessage)
    {
        OnFeedbackMessageReceived?.Invoke(feedbackMessage);
    }

    public void SupervisorMessageToDisplay(string feedbackMessage)
    {
        SupervisorFeedbackClientRpc(feedbackMessage);
    }

    [ClientRpc]
    private void SupervisorFeedbackClientRpc(string feedbackMessage)
    {
        SupervisorFeedback(feedbackMessage);

    }

    private void SupervisorFeedback(string feedbackMessage)
    {
        supervisorMessageText.text = feedbackMessage;
        StartCoroutine(MoveFeedback());
    }

    private IEnumerator MoveFeedback()
    {
        float startTime = 0f;

        // Move towards the target
        while (startTime < transitionSpeed)
        {
            float t = startTime / transitionSpeed;
            startFeedbackWindowTransform.anchoredPosition = Vector3.Lerp(startpos, finalpos, t);
            startTime += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(ShowElements());
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
            startFeedbackWindowTransform.anchoredPosition = Vector3.Lerp(finalpos, startpos, t);
            startTime += Time.deltaTime;
            yield return null;
        }
    }
}

