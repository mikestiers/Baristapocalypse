using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColorChoice : MonoBehaviour
{
    public SkinnedMeshRenderer baseMeshRenderer;
    public MeshRenderer RingMeshRenderer;

    public Material playerMaterial;
    public Material ringMaterial;
    public string intersectionColorPropertyName = "_Intersection_color";
    public Color intersectionColor;

    // Start is called before the first frame update
    private void Awake()
    {
        playerMaterial = new Material(baseMeshRenderer.material);
        ringMaterial = new Material(RingMeshRenderer.material);
        
        baseMeshRenderer.material = playerMaterial;

        intersectionColor = playerMaterial.color;
        
        baseMeshRenderer.material = playerMaterial; 
        RingMeshRenderer.material.SetColor(intersectionColorPropertyName, intersectionColor); 
    }

public void SetPlayerColor(Color color)
{
    // playerMaterial.color = color;
    ringMaterial.color = color;
    RingMeshRenderer.material.SetColor(intersectionColorPropertyName, color); 
}
}
