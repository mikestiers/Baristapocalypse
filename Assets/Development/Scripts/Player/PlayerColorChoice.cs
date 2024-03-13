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

    [SerializeField] private GameObject fireworks;

    [SerializeField] private float FXTime = 3f;
    // Start is called before the first frame update
    private void Awake()
    {
        fireworks.SetActive(false);
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

    public void StartnEndFireworks()
    {
        StartCoroutine(FireworksEffect());
    }

    private IEnumerator FireworksEffect()
    {
        fireworks.SetActive(true);
        yield return new WaitForSeconds(FXTime);
        
        fireworks.SetActive(false);
    }
}
