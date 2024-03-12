using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RandomEventEffects : NetworkBehaviour
{
    [SerializeField] private GameObject gravityLights;
    [SerializeField] private FullScreenEffectController screenFX;
    [ServerRpc(RequireOwnership = false)]
    public void TurnOnOffEventEffectServerRpc(bool trueOrFalse)
    {
        TurnOnOffEventEffectClientRpc(trueOrFalse);
    }

    [ClientRpc]
    private void TurnOnOffEventEffectClientRpc(bool trueOrFalse)
    {
        TurnOnOffEventEffect(trueOrFalse);
    }

    private void TurnOnOffEventEffect(bool trueOrFalse)
    {
        gravityLights.SetActive(trueOrFalse);
        screenFX.ToggleGravityEffect(trueOrFalse);
        Debug.LogWarning(trueOrFalse);
    }
}
