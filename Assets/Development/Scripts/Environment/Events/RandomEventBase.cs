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
<<<<<<< HEAD
    protected PlayerColorChoice fx;

    protected virtual void Start()
    {
        fx = FindObjectOfType<PlayerColorChoice>();
    }

=======
>>>>>>> parent of 582d6a99 (Merge pull request #550 from mikestiers/redoing-cusVFX)
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
