using UnityEngine;

public class UFO : MonoBehaviour
{
    public float hoverHeight = 0.5f;
    public float hoverSpeed = 2.0f;
    public float rotationSpeed = 50.0f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Hovering up and down
        float hover = Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
        transform.position = startPosition + Vector3.up * hover;

        // Rotating around its axis
        //transform.Rotate(Vector3.down, rotationSpeed * Time.deltaTime);
    }
}
