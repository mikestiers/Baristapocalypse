using Unity.Netcode;
using UnityEngine;

public interface IParticleParent
{
    public Transform GetParticleTransform();
    public void SetParticle(Ingredient ingredient);
    public Ingredient GetParticle();
    public void ClearParticle();
    public bool HasParticle();
    public NetworkObject GetNetworkObject();
}

