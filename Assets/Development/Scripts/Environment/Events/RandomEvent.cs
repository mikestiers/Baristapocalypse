using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RandomEvent : NetworkBehaviour
{
    [field: SerializeField] public RandomEventSO RandomEventSO { get; private set; }


}
