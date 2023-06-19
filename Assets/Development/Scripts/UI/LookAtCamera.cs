using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{

    private void LateUpdate()
    {
        Vector3 directionFromCamera = transform.position - Camera.main.transform.position;
        transform.LookAt(transform.position + directionFromCamera);    
    }
}
