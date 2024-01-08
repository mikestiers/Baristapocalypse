using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class MessSO : ScriptableObject
{
    public GameObject prefab;
    public string messName;
    public string messTag;
    public int dirtyness;
}
