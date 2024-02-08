using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraZoomIn : MonoBehaviour
{
    public Transform player;
    public CinemachineVirtualCamera virtualCamera;
    public float minDistance = 5f; 
    public float maxDistance = 10f; 
    public float zoomInFOV = 40f; 

    void Update()
    {
        if (player == null || virtualCamera == null)
            return;

       
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

      
        float normalizedDistance = Mathf.Clamp01((distanceToPlayer - minDistance) / (maxDistance - minDistance));

        float newFOV = Mathf.Lerp(virtualCamera.m_Lens.FieldOfView, zoomInFOV, normalizedDistance);

        virtualCamera.m_Lens.FieldOfView = newFOV;
    }
}
