using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GravityStorm : RandomEventBase
{
    public Rigidbody[] objectsToMove;
    public List<Rigidbody> objectsToMoveList = new List<Rigidbody>();
    [SerializeField] private float alpha = 0.1f; // scale of Brownian motion
    [SerializeField] private float driftSpeed = 0.1f;
    [SerializeField] private float rotationSpeed = 1.0f;
    [SerializeField] private float collisionBoxSize = 5.0f;
    [SerializeField] private GameObject gravityButton;
    [SerializeField] private Material gravityButtonMaterial;
    [SerializeField] private LayerMask gravityMask;
    [SerializeField] private GameObject gravityEventLights;
    private Collider eventCollider;
    private Vector3[] objectVelocities;
    private Rigidbody[] objectRigidbodies;

    private void Awake()
    {
        eventCollider = GetComponent<Collider>();
        gravityButton.GetComponent<MeshRenderer>().material = gravityButtonMaterial;
        //isEffectOn.Value = true;
        //TurnOnOffEventEffect(true);
    }

    private void Start()
    {
        FindObjectsToMove();
        InitializeArrays();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Mess"))
        {
            MoveObject();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // apply logic to turn gravity back on in case of the object leaving the collider
        // or we can add colliders around the collision box to prevent the objects to leave
    }

    private void FindObjectsToMove()
    {
       
        Collider[] colliders = Physics.OverlapBox(eventCollider.bounds.center, eventCollider.bounds.extents, Quaternion.identity, gravityMask);
        List<Rigidbody> rigidbodies = new List<Rigidbody>();
        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                rigidbodies.Add(rb);
            }
        }
        objectsToMove = rigidbodies.ToArray();
    }

    private void InitializeArrays()
    {
        objectVelocities = new Vector3[objectsToMove.Length];
        objectRigidbodies = new Rigidbody[objectsToMove.Length];
        for (int i = 0; i < objectsToMove.Length; i++)
        {
            objectRigidbodies[i] = objectsToMove[i];
            objectVelocities[i] = Vector3.zero;
        }
    }

    private void MoveObject()
    {

        for (int i = 0; i < objectsToMove.Length; i++)
        {
            Rigidbody rb = objectsToMove[i];

            if (rb != null)
            {
                // Apply Brownian motion to the velocity
                objectVelocities[i] += GenerateRandomVelocity() * Mathf.Sqrt(alpha);

                // Apply drifting motion
                Vector3 drift = objectVelocities[i] * driftSpeed * Time.deltaTime;

                // Apply Brownian force
                rb.AddForce(drift, ForceMode.VelocityChange);

                // Cap rb max speed
                float maxSpeed = 0.4f;
                if (rb.velocity.magnitude > maxSpeed)
                {
                    rb.velocity = rb.velocity.normalized * maxSpeed;
                }

                // Apply torque for rotation
                float torqueX = Random.Range(-1f, 1f) * rotationSpeed * Time.deltaTime;
                float torqueY = Random.Range(-1f, 1f) * rotationSpeed * Time.deltaTime;
                float torqueZ = Random.Range(-1f, 1f) * rotationSpeed * Time.deltaTime;

                rb.AddTorque(new Vector3(torqueX, torqueY, torqueZ), ForceMode.VelocityChange);

                //Debug.Log($"Speed:  {rb.velocity.magnitude}");
            }
        }
    }

    private Vector3 GenerateRandomVelocity()
    {
        float brownianX = Random.Range(-1f, 1f);
        float brownianY = Random.Range(-1f, 1f);
        float brownianZ = Random.Range(-1f, 1f);

        return new Vector3(brownianX, brownianY, brownianZ).normalized;
    }

    public void ConvertListToArray()
    {
        objectsToMove = objectsToMoveList.ToArray();
        objectVelocities = new Vector3[objectsToMove.Length];
        objectRigidbodies = new Rigidbody[objectsToMove.Length];
    }

    // Change direction on collision with other objects
    public void HandleCollision(Rigidbody collidedObject, Vector3 collisionNormal)
    {
        // Find the index of the collided object in the objectsToMove array
        int index = -1;
        for (int i = 0; i < objectsToMove.Length; i++)
        {
            if (objectsToMove[i] == collidedObject)
            {
                index = i;
                break;
            }
        }

        if (index != -1)
        {
            Vector3 reflectedVelocity = Vector3.Reflect(objectVelocities[index], collisionNormal);
            objectVelocities[index] = reflectedVelocity;
        }
    }

}

