using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissiveControl : MonoBehaviour
{
    public Material emissiveMaterial;
    private bool isEmissiveOn = false;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isEmissiveOn = !isEmissiveOn;
            SetEmissive(isEmissiveOn);
        }
    }

    public void SetEmissive(bool isActive)
    {
        if (emissiveMaterial == null) return;

        // Assuming "_EmissionColor" is the emissive property name
        Color emissiveColor = isActive ? Color.white : Color.black;
        emissiveMaterial.SetColor("_EmissionColor", emissiveColor);

        // Enable or disable emission based on the state
        emissiveMaterial.EnableKeyword("_EMISSION");
        emissiveMaterial.globalIlluminationFlags = isActive ? MaterialGlobalIlluminationFlags.RealtimeEmissive : MaterialGlobalIlluminationFlags.None;

        // This is necessary to apply the changes
        emissiveMaterial.SetFloat("_EmissionScaleUI", isActive ? 1.0f : 0.0f);
    }
}
