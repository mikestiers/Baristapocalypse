using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleSync : MonoBehaviour
{
    public static int PosID = Shader.PropertyToID("_PlayerPosition");
    public static int SizeID = Shader.PropertyToID("_Size");

    public Material wallMaterial;
    public Camera Camera;
    public LayerMask LayerMask;

    void Update()
    {
        var dir = Camera.transform.position - transform.position;
        var ray = new Ray(transform.position, dir.normalized);

        if (Physics.Raycast(ray, 3000, LayerMask))
        {
            wallMaterial.SetFloat(SizeID, 1);
        }
        else
            wallMaterial.SetFloat(SizeID, 0);
        
        var view = Camera.WorldToViewportPoint(transform.position);
        wallMaterial.SetVector(PosID, view);
    }
}
