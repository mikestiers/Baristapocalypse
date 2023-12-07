using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class PhysicsManager : MonoBehaviour
{
    [SerializeField] private float floatForce = 10f;
    [SerializeField] private LayerMask gravityLayer;
    [SerializeField] private float maxFloatDistance = 1f;
    [SerializeField] private float minFloat = 1.0f;
    [SerializeField] Vector3 startingLocation;
    [SerializeField] List<GameObject> stations;
    bool isFloating;

    private void Start()
    {
        foreach (GameObject station in stations)
        {
            station.GetComponent<Collider>().isTrigger = false;
            station.GetComponent<Rigidbody>().useGravity = true;
            station.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotation;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log($"Is floating {other.gameObject.name}");
        if ((gravityLayer.value & 1 << other.gameObject.layer) != 0)
        {
            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
            if (!isFloating)
            {
                startingLocation = other.transform.position;
                isFloating = true;
            }
            if (other.transform.position.y >= startingLocation.y + maxFloatDistance)
            {
                Debug.Log("Down");
                rb.AddForce(Vector3.down * floatForce, ForceMode.Force);
            }
            else
            {
                Debug.Log("Up");
                rb.AddForce(Vector3.up * floatForce, ForceMode.Force);
            }
        }
    }
}
