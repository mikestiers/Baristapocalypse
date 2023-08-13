using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinigameTimingBarUI : MonoBehaviour
{
    [SerializeField] private GameObject hasMinigameTimingGameObject;
    [SerializeField] private Image fillBar;
    [SerializeField] private Image sweetSpot;
    [SerializeField] private Image timingMarker;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    private IHasMinigameTiming hasMinigameTiming;

    private void Start()
    {
        hasMinigameTiming = hasMinigameTimingGameObject.GetComponent<IHasMinigameTiming>();
        if (hasMinigameTiming != null)
        {
            hasMinigameTiming.OnMinigameTimingStarted += HasMinigame_OnMinigameTimingStarted;
            Hide();
        }

        Vector2 newPosition = startPoint.position;
        timingMarker.rectTransform.localPosition = newPosition;
    }

    private void HasMinigame_OnMinigameTimingStarted(object sender, IHasMinigameTiming.OnMinigameTimingEventArgs e)
    {
        fillBar.fillAmount = e.minigameTimingNormalized;
        if (e.minigameTimingNormalized == 0f || e.minigameTimingNormalized >= 1f)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
