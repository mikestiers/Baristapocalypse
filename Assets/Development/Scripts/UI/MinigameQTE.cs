using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinigameQTE : MonoBehaviour
{
    public delegate void MinigameFinishedAction();
    public static event MinigameFinishedAction MinigameFinished;

    [Header("UIElements")]
    [SerializeField] private GameObject fillDrip;
    [SerializeField] private Slider fillSlider;
    [SerializeField] private RectTransform fillDripRectTransform;
    [SerializeField] private GameObject succeededText;
    [SerializeField] private Image[] arrows = new Image[4];

    // Will be removed and replaced with animations
    [SerializeField] private GameObject failedText;

    [Header("SliderVariables")]
    [SerializeField] private float progressTime;
    [SerializeField] private float completedTime;
    [SerializeField] private float fillDripYMultiplier = 3.077f;

    [Header("QTEVariables")]
    [SerializeField] private int QTEgen;
    [SerializeField] private int waitingForKey;
    [SerializeField] private int correctKey;
    [SerializeField] private int countingDown;

    private void Awake()
    {
        Hide();
    }

    private void Start()
    {
        fillDripRectTransform = fillDrip.GetComponent<RectTransform>();
        completedTime = fillSlider.maxValue;

        if (progressTime != 0)
        {
            progressTime = 0f;
        }
    }

    private void Update()
    {
        // Increases slider value over time
        fillSlider.value = progressTime;
        progressTime += Time.deltaTime;

        // Changes filldrip position to appear on top of the slider
        if (fillDripRectTransform != null)
        {
            fillDripRectTransform.anchoredPosition = new Vector2(fillDripRectTransform.anchoredPosition.x, 0.33f + (fillSlider.value / fillDripYMultiplier));
        }

        if (progressTime >= completedTime)
        {
            EndMinigame();
        }

        // Initiates QTE arrow spawning
        if (waitingForKey == 0)
        {
            QTEgen = Random.Range(0, 4);
            countingDown = 1;
            waitingForKey = 1;
            arrows[QTEgen].gameObject.SetActive(true);
            StartCoroutine(UptimeCountdown(QTEgen));
        }

        // Verifies input against spawned arrow
        if (QTEgen == 0)
        {
            if (Input.anyKeyDown)
            {
                if (Input.GetButtonDown("UpInput"))
                {
                    correctKey = 1;
                }
                else
                {
                    correctKey = 2;
                }

                StartCoroutine(InputSubmitted(QTEgen));
            }
        }
        else if (QTEgen == 1)
        {
            if (Input.anyKeyDown)
            {
                if (Input.GetButtonDown("DownInput"))
                {
                    correctKey = 1;
                }
                else
                {
                    correctKey = 2;
                }

                StartCoroutine(InputSubmitted(QTEgen));
            }
        }
        else if (QTEgen == 2)
        {
            if (Input.anyKeyDown)
            {
                if (Input.GetButtonDown("LeftInput"))
                {
                    correctKey = 1;
                }
                else
                {
                    correctKey = 2;
                }

                StartCoroutine(InputSubmitted(QTEgen));
            }
        }
        else if (QTEgen == 3)
        {
            if (Input.anyKeyDown)
            {
                if (Input.GetButtonDown("RightInput"))
                {
                    correctKey = 1;
                }
                else
                {
                    correctKey = 2;
                }

                StartCoroutine(InputSubmitted(QTEgen));
            }
        }
    }

    public void StartMinigame()
    {
        Show();
    }

    private void EndMinigame()
    {
        progressTime = 0f;
        failedText.SetActive(false);
        succeededText.SetActive(false);
        QTEgen = 5;
        waitingForKey = 0;
        countingDown = 1;
        for (int i = 0; i < 4; i++)
        {
            arrows[i].gameObject.SetActive(false);
        }
        Hide();
        // TODO - Give the player the completed drink
        MinigameFinished();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    // Performs action based on input verification
    IEnumerator InputSubmitted(int originalQTEgen)
    {
        QTEgen = 5;

        if (correctKey == 1)
        {
            countingDown = 2;
            succeededText.SetActive(true);
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.drinkMinigameHit);
            arrows[originalQTEgen].gameObject.SetActive(false);
            progressTime += 4.0f;
            // Here is where the succeeded animation will play
            yield return new WaitForSeconds(1.5f);
            correctKey = 0;
            succeededText.SetActive(false);
            yield return new WaitForSeconds(1.0f);
            waitingForKey = 0;
            countingDown = 1;
        }
        else if (correctKey == 2)
        {
            countingDown = 2;
            // Here is where the fail animation will play
            failedText.SetActive(true);
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.drinkMinigameMiss);
            arrows[originalQTEgen].gameObject.SetActive(false);
            yield return new WaitForSeconds(1.5f);
            correctKey = 0;
            failedText.SetActive(false);
            yield return new WaitForSeconds(1.0f);
            waitingForKey = 0;
            countingDown = 1;
        }
    }

    // Keeps arrow spawned for a set period of time before despawning
    IEnumerator UptimeCountdown(int originalQTEgen)
    {
        yield return new WaitForSeconds(3.0f);
        if (countingDown == 1)
        {
            QTEgen = 5;
            countingDown = 2;
            // Here is where the fail animation will play
            failedText.SetActive(true);
            arrows[originalQTEgen].gameObject.SetActive(false);
            yield return new WaitForSeconds(1.5f);
            correctKey = 0;
            failedText.SetActive(false);
            yield return new WaitForSeconds(1.0f);
            waitingForKey = 0;
            countingDown = 1;
        }
    }
}
