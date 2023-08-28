using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Threshold : MonoBehaviour
{
    private float RespawnPosition = 1.49f;

    private float threshold = -18f;

    private void FixedUpdate()
    {
        if (transform.position.y < threshold)
        {
            transform.position = new Vector3(0f, RespawnPosition, 0f);
        }
    }
}


