using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ripple : MonoBehaviour
{
    private MaterialChanger materialChanger;
    public float RippleSpeed = 10;

    void Start()
    {
        materialChanger = GetComponent<MaterialChanger>();

        for (int i = 0; i < materialChanger.rippleRenderers.Length; i++)
        {
            Renderer renderer = materialChanger.rippleRenderers[i];
            Material mat = Instantiate(renderer.material);
            renderer.material = mat;
            materialChanger.rippleMaterials[i] = mat;
            
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < materialChanger.rippleRenderers.Length; i++)
        {
            if (materialChanger.rippleMaterials[i] != null)
            {
                Debug.Log("Destroying: " + materialChanger.rippleMaterials[i]);
                Destroy(materialChanger.rippleMaterials[i]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CastClickRay();
        }
    }

    private void CastClickRay()
    {
        var camera = Camera.main;
        var mousePosition = Input.mousePosition;

        // The XY coordiantes are in screen space, while the Z coordinate is in view space
        var ray = camera.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, camera.nearClipPlane));

        // If ray hits collider, and its attached to this gameobject
        if (Physics.Raycast(ray, out var hit) && hit.collider.gameObject.tag == "Ripple")
        {
            // Chose random color
            Color rippleColor = Color.HSVToRGB(Random.value, 1, 1);
            foreach (Material material in materialChanger.rippleMaterials)
            {
                StartRipple(hit.point, material, rippleColor);
            }
        }
    }

    private void StartRipple(Vector3 center, Material material, Color rippleColor)
    {
        material.SetFloat("_RippleSpeed", RippleSpeed);
        material.SetVector("_RippleCenter", center);

        // The Time.timeSinceLevelLoad value is the same as the Time nodein shader graph
        material.SetFloat("_RippleStartTime", Time.time);
        material.SetColor("_BaseColor", Color.white);
        material.SetColor("_RippleColor", rippleColor);
    }
}
