using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RandomEventEffects : NetworkBehaviour
{
    [SerializeField] private GameObject gravityLights;

    [ServerRpc(RequireOwnership = false)]
    public void TurnOnOffEventEffectServerRpc(bool trueOrFalse)
    {
        TurnOnOffEventEffectClientRpc(trueOrFalse);
    }

    [ClientRpc]
    public void TurnOnOffEventEffectClientRpc(bool trueOrFalse)
    {
        TurnOnOffEventEffect(trueOrFalse);
    }

    public void TurnOnOffEventEffect(bool trueOrFalse)
    {
        gravityLights.SetActive(trueOrFalse);
    }
}
