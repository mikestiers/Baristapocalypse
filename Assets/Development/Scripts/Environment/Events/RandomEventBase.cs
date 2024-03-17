using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RandomEventBase : NetworkBehaviour
{
    [field: SerializeField] public RandomEventSO RandomEventSO { get; private set; }
    //private bool isEvent;
    private NetworkVariable<bool> isEvent = new NetworkVariable<bool>(false);
    [SerializeField] protected FullScreenEffectController screenEffect;
    protected PlayerColorChoice fx;

    protected void Start()
    {
        fx = FindObjectOfType<PlayerColorChoice>();
    }

    public RandomEventSO GetRandomEvent()
    {
        return RandomEventSO;
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }

    public void SetEventBool(bool trueOrFalse)
    {
        isEvent.Value = trueOrFalse;
    }

    public void ActivateDeactivateEvent()
    {
        gameObject.SetActive(isEvent.Value);

    }

}
