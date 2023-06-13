using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

//[RequireComponent(typeof(Camera))]
public class CameraManager : MonoBehaviour
{
    [SerializeField] List<Transform> targets;
    [SerializeField] Vector3 offset;
    [SerializeField] float smoothTime = .5f;
    [SerializeField] float minZoom = 40f;
    [SerializeField] float maxZoom = 10f;
    [SerializeField] float zoomLimiter = 50f;

    private Vector3 velocity;
    private CinemachineVirtualCamera cam;

    private void Start()
    {
        cam = GetComponent<CinemachineVirtualCamera>();

    }
    private void LateUpdate()
    {
        if(targets.Count <= 0) return;

        Move();
        Zoom();
    }

    void Zoom()
    {
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatesDistance() / zoomLimiter);
        cam.m_Lens.FieldOfView = Mathf.Lerp(cam.m_Lens.FieldOfView, newZoom, Time.deltaTime);
    }
    void Move() 
    {
        Vector3 centerPoint = GetCenterPoint();

        Vector3 newPosition = centerPoint + offset;

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }

    float GetGreatesDistance() 
    {
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++) 
        {
            bounds.Encapsulate(targets[i].position);
        }
        return bounds.size.x;
    }
    Vector3 GetCenterPoint() 
    { 
        if (targets.Count == 1) 
        {
            return targets[0].position;
        }
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++) 
        {
            bounds.Encapsulate(targets[i].position);
        }
        return bounds.center;
    }
}
