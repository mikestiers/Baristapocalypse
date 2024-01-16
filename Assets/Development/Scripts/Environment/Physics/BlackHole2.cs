using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole2 : MonoBehaviour
{
    public float gravitationalPull = 10f; // Adjustable gravitational pull strength
    public float spiralForce = 5f; // Adjustable spiral force strength
    private SphereCollider sphereCollider;
    private float angle;
    public float rotationSpeed = 2f;
    private float rotationDistance;

    private Vector3 initialPosition;
    private GameObject objectBeingAbsorbed;
    private float timer = 0.0f;
    private float lerpOffset;


    private void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();

    }

    private void Update()
    {
        if (objectBeingAbsorbed != null)
        {
            ApplyBlackHoleEffect(objectBeingAbsorbed);

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        objectBeingAbsorbed = other.gameObject;
        initialPosition = objectBeingAbsorbed.transform.position;
        rotationDistance = Vector3.Distance(initialPosition, transform.position);
    }

    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        Vector3 directionToCenter = transform.position - rb.position;
        float distance = directionToCenter.magnitude;

        // Gravitational pull force
        Vector3 gravitationalForce = directionToCenter.normalized * gravitationalPull / distance; // Force decreases with distance
        rb.AddForce(gravitationalForce, ForceMode.Acceleration);
    }

    private void ApplyBlackHoleEffect(GameObject objectBeingAbsorbed)
    {

        // Update the angle based on time and rotation speed
        angle += rotationSpeed * Time.deltaTime;
        angle %= 360f;

        timer += Time.deltaTime;
        if (timer >= 0.1f)
        {
            lerpOffset += 0.1f;
            timer = 0f; // Reset the timer
        }

        float x = transform.position.x + Mathf.Cos(angle) * (rotationDistance - lerpOffset);
        float y = transform.position.y + Mathf.Sin(angle) * (rotationDistance - lerpOffset);
        float z = transform.position.z ;

        objectBeingAbsorbed.transform.position = new Vector3(x, y, z);

    }
}
