using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class NoRotation : MonoBehaviour
{
    void Update()
    {
        transform.rotation = quaternion.Euler(0,0,0);
    }
}
