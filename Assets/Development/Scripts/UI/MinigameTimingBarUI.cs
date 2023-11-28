using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinigameTimingBarUI : MonoBehaviour
{
    [SerializeField] private GameObject hasMinigameTimingGameObject;
    [SerializeField] private Image backgroundBar;
    [SerializeField] private RectTransform sweetSpot;
    [SerializeField] private Image timingMarker;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    private float backgroundWidth;

    private IHasMinigameTiming hasMinigameTiming;

    private void Start()
    {
        hasMinigameTiming = hasMinigameTimingGameObject.GetComponent<IHasMinigameTiming>();
        if (hasMinigameTiming != null)
        {
            hasMinigameTiming.OnMinigameTimingStarted += HasMinigame_OnMinigameTimingStarted;
            Hide();
        }

        backgroundWidth = backgroundBar.rectTransform.rect.width;
    }

    private void HasMinigame_OnMinigameTimingStarted(object sender, IHasMinigameTiming.OnMinigameTimingEventArgs e)
    {
        sweetSpot.anchoredPosition = new Vector2((e.sweetSpotPosition-0.5f) * backgroundWidth, 0);
        timingMarker.fillAmount = e.minigameTimingNormalized;;
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
