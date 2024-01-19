using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColorChoice : MonoBehaviour
{
    public SkinnedMeshRenderer baseMeshRenderer;
    public SkinnedMeshRenderer jointMeshRenderer;

    public Material playerMaterial;
    public Color col;

    // Start is called before the first frame update
    private void Awake()
    {
        playerMaterial = new Material(baseMeshRenderer.material);

        baseMeshRenderer.material = playerMaterial;
        baseMeshRenderer.material = playerMaterial;
    }

    public void SetPlayerColor(Color color)
    {
        playerMaterial.color = color;
    }
}
