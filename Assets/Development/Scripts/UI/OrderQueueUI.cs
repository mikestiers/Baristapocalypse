using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class OrderQueueUI : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private Transform queueTimerTemplate;
    private OrderInfo currentOrder;

    private void Awake()
    {
        queueTimerTemplate.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        OrderManager.Instance.OnOrderSpawned += OrderManager_OnOrderSpawned;
        OrderManager.Instance.OnOrderCompleted += OrderManager_OnOrderCompleted;

        UpdateVisual();
    }

    private void OnDisable()
    {
        OrderManager.Instance.OnOrderSpawned -= OrderManager_OnOrderSpawned;
        OrderManager.Instance.OnOrderCompleted -= OrderManager_OnOrderCompleted;

        UpdateVisual();
    }

    private void OrderManager_OnOrderSpawned(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    private void OrderManager_OnOrderCompleted(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        foreach(Transform child in container)
        {
            if (child == queueTimerTemplate) continue;
            Destroy(child.gameObject);
        }

        for (int i = 0; i < OrderManager.Instance.GetOrdersList().Count; i++)
        {
            if (i < 2) continue;
            if (i > 4) break;
            Transform recipeTransform = Instantiate(queueTimerTemplate, container);
            recipeTransform.gameObject.SetActive(true);
            recipeTransform.GetComponent<QueueTimerSlider>().customerLeaveTime = OrderManager.Instance.GetOrdersList()[i].customerLeaveTime;
            recipeTransform.GetComponent<QueueTimerSlider>().orderIndex = i;
        }
    }
}
