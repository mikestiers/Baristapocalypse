using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SplineCameraScript : MonoBehaviour
{
    [SerializeField] private GameObject SplineCameraPos;
    [SerializeField] private GameObject MainCamera;
    [SerializeField] public Transform Player;
    [SerializeField] float smoothTime = .5f;
    [SerializeField] Vector3 offset;

    private Vector3 velocity;
    [SerializeField] private Camera cam;
    private bool hasListShrunk = false;
    private void Start()
    {
        MainCamera.gameObject.transform.position = SplineCameraPos.transform.position;
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        if(Player == null) 
        {
            Player = FindObjectOfType<PlayerController>().gameObject.transform;
        }
    }
    private void LateUpdate()
    {
        if(Player == null) { return; }

        Move();
        
    }

    void Move()
    {
        Vector3 centerPoint = Player.position;             //GetCenterPoint();

        Vector3 newPosition = centerPoint + offset;

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }

 
}
