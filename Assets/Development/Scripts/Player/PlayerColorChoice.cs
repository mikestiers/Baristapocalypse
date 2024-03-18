using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColorChoice : MonoBehaviour
{
    public SkinnedMeshRenderer baseMeshRenderer;
    public MeshRenderer RingMeshRenderer;
    public GameObject playerVisualMaterial;

    public Material playerMaterial;
    public Material ringMaterial;
    public string intersectionColorPropertyName = "_Intersection_color";
    public Color intersectionColor;
<<<<<<< HEAD

   [SerializeField] private GameObject fireworks;
   [SerializeField] private float FXTime;
=======
>>>>>>> parent of 582d6a99 (Merge pull request #550 from mikestiers/redoing-cusVFX)

    // Start is called before the first frame update
    private void Awake()
    {
        playerMaterial = new Material(baseMeshRenderer.material);
        ringMaterial = new Material(RingMeshRenderer.material);
        
        //baseMeshRenderer.material = playerMaterial;
        baseMeshRenderer.material = playerMaterial; 
        RingMeshRenderer.material.SetColor(intersectionColorPropertyName, intersectionColor);
        intersectionColor = playerMaterial.color;
        //intersectionColor = baseMeshRenderer.material.GetColor("_EmissionColor");

    }

    public void SetPlayerColor(Material material)
    {
        baseMeshRenderer.material = material;
    }

    public void SetRingColor(Color color)
    {
        ringMaterial.color = color;
        RingMeshRenderer.material.SetColor(intersectionColorPropertyName, color); 

    }
}
