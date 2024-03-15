using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class BuldgeControl : MonoBehaviour
{
    public Material buldgeMaterial;
    private bool isBuldgeOn = false;
    [SerializeField] private Color buldgeColor = Color.white;
    [SerializeField] private float emissionIntensity = 1.0f;
    [SerializeField] private float buldgeStrength = 0.007f;

    private void Start()
    {
        if (buldgeMaterial == null)
        {
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                Debug.Log("RENDERER NOT NULL");
                buldgeMaterial = renderer.material;
            }
        }

        SetBuldge(isBuldgeOn);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            SetBuldge(!isBuldgeOn);

        }
    }

    public void SetBuldge(bool isActive)
    {
        Debug.Log("setting buldge");
        if (buldgeMaterial == null) return;
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetColor("_Buldge_Color", isActive ? buldgeColor * emissionIntensity : Color.black);
        propertyBlock.SetFloat("_Buldge_strength", isActive? buldgeStrength : 0.0f);
        GetComponent<Renderer>().SetPropertyBlock(propertyBlock);
    }
}
