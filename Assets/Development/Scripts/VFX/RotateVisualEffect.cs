using System;
using UnityEngine;
using UnityEngine.VFX;

public class RotateVisualEffect : MonoBehaviour
{
    public Transform aiHead;
    public Camera mainCamera;
    public VisualEffect visualEffect;
    public float rotationSpeed;

    public void Start()
    {
        mainCamera = FindObjectOfType<Camera>();
    }

    void Update()
    {
        Vector3 directionToCamera = mainCamera.transform.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(directionToCamera);
        transform.rotation = lookRotation;

        Vector3 rotationOffset = Quaternion.Euler(0f, visualEffect.GetFloat("angle"), 0f) * Vector3.forward;
        visualEffect.SetVector3("PositionOffset", aiHead.position + rotationOffset);
        visualEffect.SetFloat("angle", visualEffect.GetFloat("angle") + Time.deltaTime * rotationSpeed);
    }
}

