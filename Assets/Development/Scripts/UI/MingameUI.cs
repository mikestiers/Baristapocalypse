using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MingameUI : MonoBehaviour
{
    [Header("Arrows")]
    [SerializeField] private Image[] arrows = new Image [4];

    [Header("Timer")]
    [SerializeField] private Slider timeSlider;
    private float progressTime = 0f;
    private float completedTime = 30f;

    [Header("QTEvent")]
    private bool attempted = false;
    private bool succeeded;
    private bool QTRunning = false;
    [SerializeField] private float QTUptime = 2f;
    [SerializeField] private float QTDowntime = 3f;

    [SerializeField] private GameObject timeAddText;
    [SerializeField] private Animator anim;

    void Start()
    {
        if (progressTime != 0f)
        {
            progressTime = 0f;
        }
    }

    void Update()
    {
        timeSlider.value = progressTime;

        progressTime += Time.deltaTime;

        if (progressTime >= completedTime)
        {
            EndMinigame();
        }
        else
        {
            // Minigame logic
            // TODO - add D-Pad inputs
            if (Input.GetKeyDown(KeyCode.W) ||
                Input.GetKeyDown(KeyCode.S) ||
                Input.GetKeyDown(KeyCode.A) ||
                Input.GetKeyDown(KeyCode.D) ||
                Input.GetKeyDown(KeyCode.UpArrow) ||
                Input.GetKeyDown(KeyCode.DownArrow) ||
                Input.GetKeyDown(KeyCode.LeftArrow) ||
                Input.GetKeyDown(KeyCode.RightArrow))
            {
                attempted = true;
            }

            if (!QTRunning)
            {
                QTRunning = true;
                StartCoroutine(DisplayQuickTimeButtons());
            }
        }
    }

    private void EndMinigame()
    {
        progressTime = 0f;
        //Hide();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator DisplayQuickTimeButtons()
    {
        int arrowSelection = Random.Range(0, arrows.Length);
        arrows[arrowSelection].gameObject.SetActive(true);

        // Wait for x amount of seconds while also checking if the player attempts the quick time event
        float elapsedTime = 0f;
        while (elapsedTime < QTUptime)
        {
            if (attempted)
            {
                elapsedTime = QTUptime;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (!attempted)
        {
            anim.SetTrigger("Timeup");
            yield return new WaitForSeconds(1f);
        }
        else
        {
            QuickTimeVerification(arrowSelection);
            yield return StartCoroutine(QuickTimeResults(arrowSelection));
        }

        arrows[arrowSelection].gameObject.SetActive(false);
        yield return new WaitForSeconds((QTDowntime - 1f));
        if (timeAddText.activeInHierarchy)
        {
            timeAddText.SetActive(false);
        }
        QTRunning = false;
    }

    private void QuickTimeVerification(int selection)
    {
        // TODO - add D-Pad input verification
        switch (selection)
        {
            case 1:
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    succeeded = true;
                }
                else
                {
                    succeeded = false;
                }
                break;
            case 2:
                if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                {
                    succeeded = true;
                }
                else
                {
                    succeeded = false;
                }
                break;
            case 3:
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    succeeded = true;
                }
                else
                {
                    succeeded = false;
                }
                break;
            case 4:
                if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    succeeded = true;
                }
                else
                {
                    succeeded = false;
                }
                break;
            default:
                succeeded = false;
                break;
        }
    }

    private IEnumerator QuickTimeResults(int activeArrow)
    {
        if (succeeded)
        {
            anim.SetTrigger("Succeeded");
            timeAddText.SetActive(true);
            progressTime += 5f;
            yield return new WaitForSeconds(1f);
        }
        else if (!succeeded)
        {
            anim.SetTrigger("Failed");
            yield return new WaitForSeconds(1f);
        }

        attempted = false;
    }
}
