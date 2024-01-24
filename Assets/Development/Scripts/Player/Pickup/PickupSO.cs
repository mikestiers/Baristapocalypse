using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu (fileName ="PickupScriptableObjects", menuName ="ScriptableObjects/Pickups")]
public class PickupSO : ScriptableObject
{
    public GameObject prefab;
    public string objectName;
    public string objectTag;
}
