using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public interface IPickupObjectParent 
{
    public Transform GetPickupTransform();
    public void SetPickup(Pickup pickup);
    public void ClearPickup();
    public bool HasPickup();
    public NetworkObject GetNetworkObject();
}
