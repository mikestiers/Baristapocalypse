using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingManager : Singleton<PingManager>
{
    public GameObject target; // the target where the ping visual prefab will be near
    public GameObject pingIndicator; // The ping visual prefab
    public ParticleSystem pingEffect; // The ping effect

    public void CreatePing(GameObject target)
    {
        if (target != null && pingIndicator != null)
        {
            Bounds bounds = GetBounds(target);
            float targetHeight = bounds.size.y;
            Vector3 instantiatePosition = target.transform.position + new Vector3(0, targetHeight / 2, 0);
            Instantiate(pingIndicator, instantiatePosition, target.transform.rotation, target.transform);
            if (pingEffect != null)
            {
                pingEffect.Play();
            }
        }
    }

    private Bounds GetBounds(GameObject obj)
    {
        Collider collider = obj.GetComponent<Collider>();
        if (collider != null)
        {
            return collider.bounds;
        }

        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds;
        }

        return new Bounds();
    }
}
