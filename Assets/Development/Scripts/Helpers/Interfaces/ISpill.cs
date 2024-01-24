using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public interface ISpill 
{
    public Transform GetSpillTransform();
    public void SetSpill(Spill spill);
    public void ClearSpill();
    public NetworkObject GetNetworkObject();
}
