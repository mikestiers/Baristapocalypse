using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RandomEventEffects : NetworkBehaviour
{
    [SerializeField] private GameObject gravityLights;
    [SerializeField] private FullScreenEffectController screenFX;

    public void TurnOnOffEventEffect(bool trueOrFalse)
    {
        TurnOnOffEventEffectServerRpc(trueOrFalse);
    }

    [ServerRpc(RequireOwnership = false)]
    private void TurnOnOffEventEffectServerRpc(bool trueOrFalse)
    {
        TurnOnOffEventEffectClientRpc(trueOrFalse);
    }

    [ClientRpc]
    private void TurnOnOffEventEffectClientRpc(bool trueOrFalse)
    {
        gravityLights.SetActive(trueOrFalse);
        screenFX.ToggleGravityEffect(trueOrFalse);
    }

    
}
