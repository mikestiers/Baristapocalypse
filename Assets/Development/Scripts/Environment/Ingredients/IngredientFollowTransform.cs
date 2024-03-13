using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientFollowTransform : MonoBehaviour
{
    private Transform targetTransform;

    public void SetTargetTransform(Transform targetTransform)
    {
        this.targetTransform = targetTransform;
    }

    public void AdjustTargetTransform(Vector3 holdPos , Quaternion holdRot)
    {
        targetTransform.localPosition = holdPos;
        targetTransform.localRotation = holdRot;
    }

    private void LateUpdate()
    {
        if (targetTransform == null)
        {
            return;
        }

        transform.position = targetTransform.position;
        transform.rotation = targetTransform.rotation;
    }
}
