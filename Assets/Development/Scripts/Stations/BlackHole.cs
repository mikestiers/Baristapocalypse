using UnityEngine;

public class BlackHole : MonoBehaviour
{
    public float gravitationalPull = 10f; // Adjustable gravitational pull strength
    public float spiralForce = 5f; // Adjustable spiral force strength
    private SphereCollider sphereCollider;
    private float angle;
    private float rotationSpeed;
    private float rotationDistance;
    private bool isBeingPulled = false;


    private void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
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

        angle += rotationSpeed * Time.deltaTime;
        //if (!isBeingPulled)
        //{
        //    rotationDistance = Vector3.Distance(targetRigidbody.gameObject.transform.position, sphereCollider.center);
        //    isBeingPulled = true;
        //}


        Vector3 directionToCenter = transform.position - targetRigidbody.position;
        float distance = directionToCenter.magnitude;

        // Gravitational pull force
        //Vector3 gravitationalForce = directionToCenter.normalized * gravitationalPull / distance; // Force decreases with distance
        //targetRigidbody.AddForce(gravitationalForce, ForceMode.Acceleration);

        //// Spiral force
        float radius = sphereCollider.radius;

        // Calculate the new position
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;
        //float y = targetRigidbody.position.y; // Keep the original Y position

        // Set the new position relative to the center object
        targetRigidbody.position = new Vector3(x, targetRigidbody.transform.position.y, z);

    }
}
