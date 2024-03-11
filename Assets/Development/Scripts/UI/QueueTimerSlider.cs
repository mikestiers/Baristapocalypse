using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class QueueTimerSlider : NetworkBehaviour
{
    public float customerLeaveTime;
    public int orderIndex;
    private Slider timerSlider;

    private void Start()
    {
        timerSlider = GetComponent<Slider>();
    }

    private void Update()
    {
        UpdateSlider(OrderManager.Instance.GetOrdersList()[orderIndex].orderTimer);
    }

    public void UpdateSlider(float timer)
    {
        Debug.Log(timer);
        float timerValue = -(customerLeaveTime - timer) / customerLeaveTime;
        UpdateSliderClientRpc(timerValue);
    }

    [ClientRpc]
    private void UpdateSliderClientRpc(float timer)
    {
        timerSlider.value = timer;
    }

}
