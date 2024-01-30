using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class EventSO : ScriptableObject
{
    public string eventName;
    public bool isEvent;
    public float chanceOfoccurrence;
}
