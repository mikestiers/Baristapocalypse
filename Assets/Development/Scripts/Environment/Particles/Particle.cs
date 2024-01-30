using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Particle : NetworkBehaviour
{
    [field: SerializeField] public ParticleSO ParticleSO { get; private set; }

    private IParticleParent particleParent;
    public IngredientFollowTransform followTransform;

    protected virtual void Awake()
    {
        followTransform = GetComponent<IngredientFollowTransform>();
    }

    public ParticleSO GetParticleSO()
    {
        return ParticleSO;
    }

    public void SetParticleParent(IParticleParent particleParent)
    {
        SetParticleParentServerRpc(particleParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetParticleParentServerRpc(NetworkObjectReference particleParentNetworkObjectReference)
    {
        SetParticleParentClientRpc(particleParentNetworkObjectReference);
    }

    [ClientRpc]
    private void SetParticleParentClientRpc(NetworkObjectReference particleParentNetworkObjectReference)
    {
        particleParentNetworkObjectReference.TryGet(out NetworkObject particleParentNetworkObject);
        IParticleParent particleParent = particleParentNetworkObject.GetComponent<IParticleParent>();

        if (this.particleParent != null)
        {
            this.particleParent.ClearParticle();
        }
        this.particleParent = particleParent;
        particleParent.SetParticle(this);

        followTransform.SetTargetTransform(particleParent.GetParticleTransform());
        
    }

    public IParticleParent GetParticleParent()
    {
        return particleParent;
    }

    public static void DestroyParticle(Particle particle)
    {
        BaristapocalypseMultiplayer.Instance.DestroyParticle(particle);
    }

    public static void SpawnParticle(ParticleSO particleSO, IParticleParent particleParent)
    {
        BaristapocalypseMultiplayer.Instance.SpawnParticle(particleSO, particleParent);
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void ClearParticleOnParent()
    {
        particleParent.ClearParticle();
    }
}
