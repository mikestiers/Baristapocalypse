using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraManager : MonoBehaviour
{
    public List<Transform> targets = new List<Transform>();
    [SerializeField] Vector3 offset;
    [SerializeField] float smoothTime = .5f;
    [SerializeField] float minZoom = 50f;
    [SerializeField] float maxZoom = 25f;
    [SerializeField] float zoomLimiter = 50f;

    private Vector3 velocity;
    private Camera cam;

    private bool hasListShrunk = false;

    private void Start()
    {
        cam = GetComponent<Camera>();
            
    }

    private void Update()
    {
        if (hasListShrunk == false)
        {
            // Iterate through the list in reverse to avoid index issues when removing elements.
            targets.RemoveAll(target => !target.gameObject.activeSelf);
            hasListShrunk = true;
        }
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
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime);
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
