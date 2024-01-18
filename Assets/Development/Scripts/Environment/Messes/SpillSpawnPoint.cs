using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpillSpawnPoint : MonoBehaviour
{
    private Transform targetTransform;

    public void SetSpawnPointTransform(Transform targetTransform)
    {
        this.targetTransform = targetTransform;
        transform.position = targetTransform.position;
        transform.rotation = targetTransform.rotation;
    }

}
