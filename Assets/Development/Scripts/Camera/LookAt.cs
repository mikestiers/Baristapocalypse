using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    [SerializeField] private GameObject CenterPoint;
    [SerializeField] private GameObject LeftPoint;
    [SerializeField] private CinemachineVirtualCamera VirtualCamera;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            VirtualCamera.LookAt = LeftPoint.transform;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            VirtualCamera.LookAt = CenterPoint.transform;
        }
    }

}