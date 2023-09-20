using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChanger : MonoBehaviour
{
    public MeshRenderer[] rippleRenderers;
    public Material[] rippleMaterials;

    void Awake()
    {
        // Get All renderers 
        MeshRenderer[] allRenderers = (MeshRenderer[])FindObjectsOfType(typeof(MeshRenderer));

        rippleMaterials = new Material[allRenderers.Length];
        rippleRenderers = new MeshRenderer[allRenderers.Length];
        int rippleMaterialCount = 0;
        for (int i = 0; i < allRenderers.Length; i++)
        {
            Material mat = allRenderers[i].material;

            // Add to Ripple Array if it has the Ripple shader
            if (mat.shader.name == "My_Shaders/Ripple")
            {
                rippleMaterials[rippleMaterialCount] = mat;
                rippleRenderers[rippleMaterialCount] = allRenderers[i];
                rippleMaterialCount++;
            }
        }
        Array.Resize(ref rippleMaterials, rippleMaterialCount);
        Array.Resize(ref rippleRenderers, rippleMaterialCount);
    }
}
