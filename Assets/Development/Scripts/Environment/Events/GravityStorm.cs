using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityStorm : RandomEventBase
{
    public GameObject[] objectsToMove;
    public float alpha = 0.1f; // scale of Brownian motion
    public float driftSpeed = 0.1f;
    public float rotationSpeed = 1.0f;
    public float collisionBoxSize = 5.0f;

    private Vector3[] objectVelocities;
    private Rigidbody[] objectRigidbodies;

    private void Start()
    {
        objectVelocities = new Vector3[objectsToMove.Length];
        objectRigidbodies = new Rigidbody[objectsToMove.Length];

        for (int i = 0; i < objectsToMove.Length; i++)
        {
            objectVelocities[i] = GenerateRandomVelocity();
            objectRigidbodies[i] = objectsToMove[i].GetComponent<Rigidbody>();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Mess"))
        {
            for (int i = 0; i < objectsToMove.Length; i++)
            {
                MoveObject(objectsToMove[i].transform, objectRigidbodies[i], i);
            }
        }
    }

    private void MoveObject(Transform objTransform, Rigidbody objRigidbody, int index)
    {
        // Apply Brownian motion to the velocity
        objectVelocities[index] += GenerateRandomVelocity() * Mathf.Sqrt(alpha);

        // Apply drifting motion
        Vector3 drift = objectVelocities[index] * driftSpeed * Time.deltaTime;

        // Apply Brownian force
        objRigidbody.AddForce(drift, ForceMode.VelocityChange);

        float maxSpeed = 0.2f; // cap rb max speed
        if (objRigidbody.velocity.magnitude > maxSpeed)
        {
            objRigidbody.velocity = objRigidbody.velocity.normalized * maxSpeed;
        }

        // Apply torque for rotation
        float torqueX = Random.Range(-1f, 1f) * rotationSpeed * Time.deltaTime;
        float torqueY = Random.Range(-1f, 1f) * rotationSpeed * Time.deltaTime;
        float torqueZ = Random.Range(-1f, 1f) * rotationSpeed * Time.deltaTime;

        objRigidbody.AddTorque(new Vector3(torqueX, torqueY, torqueZ), ForceMode.VelocityChange);

        Debug.Log($"Speed:  {objRigidbody.velocity.magnitude}");
    }

    private Vector3 GenerateRandomVelocity()
    {
        // Box-Muller transform for generating normally distributed random numbers
        float u1 = 1f - Random.value;
        float u2 = 1f - Random.value;
        float z0 = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Cos(2f * Mathf.PI * u2);
        float z1 = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Sin(2f * Mathf.PI * u2);

        float z2 = Random.Range(-1f, 1f);

        return new Vector3(z0, z1, z2);

        //float brownianX = Random.Range(-1f, 1f);
        //float brownianY = Random.Range(-1f, 1f);
        //float brownianZ = Random.Range(-1f, 1f);

        //return new Vector3(brownianX, brownianY, brownianZ).normalized;
    }

    public void HandleCollision(Vector3 collisionNormal)
    {
        // Change direction on collision with other objects
        for (int i = 0; i < objectsToMove.Length; i++)
        {
            objectVelocities[i] = Vector3.Reflect(objectVelocities[i], collisionNormal);
        }
    }

}
