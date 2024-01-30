using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class RandomEventSO : ScriptableObject
{
    public GameObject randomObjectEvent;
    public string eventName;
    public float chanceOfOccurrence;
}
