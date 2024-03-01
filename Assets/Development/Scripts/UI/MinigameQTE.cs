using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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

    private Coroutine curUptimeCoroutine;
    private bool wasButtonPressed;
    private string buttonPressedName;

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

    private void OnEnable()
    {
        InputManager.OnAnyGamepadButtonPressed += AnyGamepadButtonPressed;
        buttonPressedName = "";

        if (progressTime != 0)
        {
            progressTime = 0f;
        }
        failedText.SetActive(false);
        succeededText.SetActive(false);
        QTEgen = 5;
        waitingForKey = 0;
        countingDown = 1;
        for (int i = 0; i < 4; i++)
        {
            arrows[i].gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        InputManager.OnAnyGamepadButtonPressed -= AnyGamepadButtonPressed;
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
            curUptimeCoroutine = StartCoroutine(UptimeCountdown(QTEgen));
        }

        // Verifies input against spawned arrow
        if (QTEgen == 0)
        {
            if (Input.anyKeyDown || CheckIfButtonWasPressed())
            {
                if (Input.GetButtonDown("UpInput") || buttonPressedName == "up")
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
            if (Input.anyKeyDown || CheckIfButtonWasPressed())
            {
                if (Input.GetButtonDown("DownInput") || buttonPressedName == "down")
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
            if (Input.anyKeyDown || CheckIfButtonWasPressed())
            {
                if (Input.GetButtonDown("LeftInput") || buttonPressedName == "left")
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
            if (Input.anyKeyDown || CheckIfButtonWasPressed())
            {
                if (Input.GetButtonDown("RightInput") || buttonPressedName == "right")
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
        for (int i = 0; i < 1; i++)
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

    private void AnyGamepadButtonPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            wasButtonPressed = true;
            buttonPressedName = context.control.name;
        }
    }

    public bool CheckIfButtonWasPressed()
    {
        if (wasButtonPressed)
        {
            wasButtonPressed = false; // Reset the state
            return true;
        }
        return false;
    }

    // Performs action based on input verification
    IEnumerator InputSubmitted(int originalQTEgen)
    {
        QTEgen = 5;

        if (correctKey == 1)
        {
            if (curUptimeCoroutine != null)
            {
                StopCoroutine(curUptimeCoroutine);
            }

            countingDown = 2;
            succeededText.SetActive(true);
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.drinkMinigameHit);
            arrows[originalQTEgen].gameObject.SetActive(false);
            progressTime += 1.0f;
            // Here is where the succeeded animation will play
            yield return new WaitForSeconds(0);
            correctKey = 0;
            succeededText.SetActive(false);
            failedText.SetActive(false);
            yield return new WaitForSeconds(0f);
            waitingForKey = 0;
            countingDown = 1;
        }
        else if (correctKey == 2)
        {
            if (curUptimeCoroutine != null)
            {
                StopCoroutine(curUptimeCoroutine);
            }

            countingDown = 2;
            // Here is where the fail animation will play
            failedText.SetActive(true);
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.drinkMinigameMiss);
            arrows[originalQTEgen].gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            correctKey = 0;
            failedText.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            waitingForKey = 0;
            countingDown = 1;
        }
    }

    // Keeps arrow spawned for a set period of time before despawning
    IEnumerator UptimeCountdown(int originalQTEgen)
    {
        yield return new WaitForSeconds(1.0f);
        if (countingDown == 1)
        {
            Debug.Log("UptimeCoroutine NOT stopped");
            QTEgen = 5;
            countingDown = 2;
            // Here is where the fail animation will play
            failedText.SetActive(true);
            arrows[originalQTEgen].gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            correctKey = 0;
            failedText.SetActive(false);
            yield return new WaitForSeconds(0f);
            waitingForKey = 0;
            countingDown = 1;
        }
    }
}
