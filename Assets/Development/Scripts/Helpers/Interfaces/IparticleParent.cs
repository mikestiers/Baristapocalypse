using Unity.Netcode;
using UnityEngine;

public interface IParticleParent
{
    public Transform GetParticleTransform();
    public void SetParticle(Particle particle);
    public Particle GetParticle();
    public void ClearParticle();
    public NetworkObject GetNetworkObject();
}

