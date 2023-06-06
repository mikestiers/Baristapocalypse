using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    public Text timer;
    private float timeRemaining = 10f;
    private int score;
    public UnityEvent LoseEvent;
    public UnityEvent WinEvent;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timeRemaining -= Time.deltaTime;
        timer.text = timeRemaining.ToString("n2");

        if (timeRemaining <= 0)
        {
            LoseEvent.Invoke();
            Debug.LogError("lose");
        }

        if (score == 100)
        {
            WinEvent.Invoke();
            Debug.LogError("win");
        }
    }


}
