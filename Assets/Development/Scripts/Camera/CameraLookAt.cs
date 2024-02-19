using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraLookAt : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public GameObject targetObject1;
    public GameObject targetObject2;
    public CameraMove cameraMoveScript;

    void Start()
    {
       
        if (virtualCamera == null)
            virtualCamera = GetComponent<CinemachineVirtualCamera>();

        cameraMoveScript = FindObjectOfType<CameraMove>();
    }

    void Update()
    {
       
        if (cameraMoveScript != null)
        {
            // Check the isinzone bool from the CameraMove script
            if (cameraMoveScript.cameralookatzone == true)
            {
                
                virtualCamera.LookAt = targetObject2.transform;
            }
            else
            {
                
                virtualCamera.LookAt = targetObject1.transform;
            }
        }
    }
}