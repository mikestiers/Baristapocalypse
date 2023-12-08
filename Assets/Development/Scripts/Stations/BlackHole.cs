using UnityEngine;

public class BlackHole : MonoBehaviour
{
    public float gravitationalPull = 10f; // Adjustable gravitational pull strength
    public float centerForce = 3.7f;
    public float rightForce = 3.5f;
    private void OnValidate()
    {
        if (centerForce <= rightForce)
        {
            Debug.LogError("centerForce needs to be greater than rightForce");
            centerForce = 3.7f;
            rightForce = 3.5f;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.attachedRigidbody)
        {
            ApplyBlackHoleEffect(other.attachedRigidbody);
        }
    }

    private void ApplyBlackHoleEffect(Rigidbody targetRigidbody)
    {
        Debug.Log("In the blackhole");
        targetRigidbody.useGravity = false;
        Vector3 directionToCenter = transform.position - targetRigidbody.position;
        //float distance = directionToCenter.magnitude;
        float distance = Vector3.Distance(transform.position, targetRigidbody.position);

        // Middle Force
        Vector3 gravitationalForce = directionToCenter.normalized * (gravitationalPull / distance); // Force decreases with distance

        // Right and Up Force
        targetRigidbody.transform.LookAt(transform.position);
        Vector3 rightForce = (targetRigidbody.transform.up + targetRigidbody.transform.right) * (gravitationalPull / distance);
        targetRigidbody.AddForce((gravitationalForce * 3.7f) + (rightForce * 3.5f), ForceMode.Force);

        targetRigidbody.velocity = Vector3.ClampMagnitude(targetRigidbody.velocity, 5.0f);
    }
}
