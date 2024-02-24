using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class AISupervisor : NetworkBehaviour
{
    public static AISupervisor Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI supervisorMessageText;
    private string stringTest = "There's no story in video games";

    [SerializeField] private float transitionSpeed;
    [SerializeField] private float popOutReviewTime;
    [SerializeField] private RectTransform startFeedbackWindowTransform;
    [SerializeField] private GameObject continueButton;
    
    [SerializeField] private Vector2 startpos;
    [SerializeField] private Vector2 finalpos;

    // Event to receive Supervisor Message
    public delegate void FeedbackMessageHandler(string feedbackMessage);
    public static event FeedbackMessageHandler OnFeedbackMessageReceived;

    public delegate void TutorialMessageHandler();
    public event TutorialMessageHandler OnTutorialMessageReceived;

    private void Awake()
    {
        Instance = this;
        OnFeedbackMessageReceived += HandleFeedbackMessage;
    }

    public override void OnDestroy()
    {
        OnFeedbackMessageReceived -= HandleFeedbackMessage;
    }

    private void OnEnable()
    {
        InputManager.OnInputChanged += InputUpdated;
    }

    private void OnDisable()
    {
        InputManager.OnInputChanged -= InputUpdated;
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

    private void InputUpdated(InputImagesSO inputImagesSO)
    {
        continueButton.GetComponentInChildren<Image>().sprite = inputImagesSO.interact;
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
        if (TutorialManager.Instance.tutorialEnabled)
            continueButton.SetActive(true);
        else
            continueButton.SetActive(false);
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

        // Pauses the game and allows for the player to read the message and continue when ready
        if (TutorialManager.Instance.tutorialEnabled)
            OnTutorialMessageReceived?.Invoke();

        StartCoroutine(ShowElements());
    }

    private IEnumerator ShowElements()
    {
        // 0f wait time if tutorial is enabled because tutorials prompt to continue
        yield return new WaitForSeconds(TutorialManager.Instance.tutorialEnabled ? 0f : popOutReviewTime);
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

