using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityStation : MonoBehaviour
{
    [SerializeField] public GameObject gravityField;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            gravityField.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            gravityField.SetActive(false);
        }
    }
}
