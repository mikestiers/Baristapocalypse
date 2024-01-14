using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameTimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameTimerText;


    private void Update()
    {
        gameTimerText.text = GameManager.Instance.GetGamePlayingTimer().ToString("n2");
    }
}
