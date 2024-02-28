using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraStation1 : MonoBehaviour
{

    [SerializeField] private CinemachineVirtualCamera m_Camera;

   
    private int insidePriority = 20;

    
    private int defaultPriority = 0;


    private void Start()
    {
        m_Camera.Priority = defaultPriority;
    }

    public void SwitchCameraOn()
    {
        m_Camera.gameObject.SetActive(true);

        m_Camera.Priority = insidePriority;
    }

    public void SwitchCameraOff()
    {
        m_Camera.gameObject.SetActive(false);

        m_Camera.Priority = defaultPriority;
    }

    //private void OnTriggerEnter(Collider other)
    //{
       
    //    if (other.CompareTag("Player"))
    //    {
    //        m_Camera.gameObject.SetActive(true);

    //        m_Camera.Priority = insidePriority;
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
        
    //    if (other.CompareTag("Player"))
    //    {
    //        m_Camera.gameObject.SetActive(false);

    //        m_Camera.Priority = defaultPriority;
    //    }
    //}
}
