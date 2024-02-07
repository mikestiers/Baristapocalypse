using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private GameObject Point1;
    [SerializeField] private GameObject Point2;
    [SerializeField] private GameObject DollyTrack;
 
    public bool cameralookatzone;

    public float moveDuration = 1f;

    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private float startTime;

    private void Start()
    {
        initialPosition = DollyTrack.transform.position;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
           targetPosition = Point2.transform.position;
           startTime = Time.time;
           cameralookatzone = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           targetPosition = Point1.transform.position;
           startTime = Time.time;
           cameralookatzone = false;
        }
    }

    private void Update()
    {
        if (Time.time - startTime < moveDuration)
        {
            float t = (Time.time - startTime) / moveDuration;
            DollyTrack.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
        }
    }

}
