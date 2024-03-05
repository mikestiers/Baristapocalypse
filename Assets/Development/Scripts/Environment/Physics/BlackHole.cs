using UnityEngine;

public class BlackHole : MonoBehaviour
{
    public float gravitationalPull = 10f; // Adjustable gravitational pull strength
    public float centerForceScale = 3.7f;
    public float rightForceScale = 3.5f;
    private void OnValidate()
    {
        if (centerForceScale <= rightForceScale)
        {
            Debug.LogError("centerForce needs to be greater than rightForce");
            centerForceScale = 3.7f;
            rightForceScale = 3.5f;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.attachedRigidbody && other.gameObject.GetComponent<PlayerController>() == null)
        {
            ApplyBlackHoleEffect(other.attachedRigidbody);
        }
    }

    private void ApplyBlackHoleEffect(Rigidbody targetRigidbody)
    {
        targetRigidbody.useGravity = false;
        Vector3 directionToCenter = transform.position - targetRigidbody.position;
        float distance = Vector3.Distance(transform.position, targetRigidbody.position);

        // Center Force
        Vector3 centerForce = directionToCenter.normalized * (gravitationalPull / distance); // Force decreases with distance

        // Right and Up Force
        targetRigidbody.transform.LookAt(transform.position);
        Vector3 rightForce = (targetRigidbody.transform.up + targetRigidbody.transform.right) * (gravitationalPull / distance);
        targetRigidbody.AddForce((centerForce * centerForceScale) + (rightForce * rightForceScale), ForceMode.Force);

        targetRigidbody.velocity = Vector3.ClampMagnitude(targetRigidbody.velocity, 5.0f);
    }
}
