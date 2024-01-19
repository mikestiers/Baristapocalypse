using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName ="MessScriptableObjects", menuName ="ScriptableObjects/Mess")]
public class MessSO : ScriptableObject
{
    public GameObject prefab;
    public string messName;
    public string messTag;
    public int dirtyness;
}
