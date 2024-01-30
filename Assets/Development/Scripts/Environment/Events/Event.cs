using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Event : NetworkBehaviour
{
    [field: SerializeField] public EventSO EventSO { get; private set; }


}
