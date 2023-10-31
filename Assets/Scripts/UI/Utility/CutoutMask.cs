using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CutoutMask : Image
{
    new IEnumerator Start()
    {
        yield return null;
        OnDisable();
        yield return null;
        OnEnable();
    }

    public override Material materialForRendering
    {
        get
        {
            Material material = new(base.materialForRendering);
            material.SetInt("_StencilComp", (int)CompareFunction.NotEqual); 
            return material;
        }
    }
}
