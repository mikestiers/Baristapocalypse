using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class EmissiveControl : NetworkBehaviour
{
    public Material emissiveMaterial;
    private bool isEmissiveOn = false;
    [SerializeField] private float emissionIntensity = 1.0f;

    private void Start()
    {
        if(emissiveMaterial == null)
        {
            Renderer renderer = GetComponent<Renderer>();
            if(renderer != null )
            {
                Debug.Log("RENDERER NOT NULL");
                emissiveMaterial = renderer.material;
            }
        }

        SetEmissive(isEmissiveOn);
    }

    public void SetEmissive(bool isActive)
    {
        if (emissiveMaterial == null) return;
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetColor("_EmissionColor", isActive ? Color.white * emissionIntensity : Color.black);
        propertyBlock.SetFloat("_EmissionScaleUI", isActive ? 1.0f : 0.0f);
        GetComponent<Renderer>().SetPropertyBlock(propertyBlock);
    }
}
