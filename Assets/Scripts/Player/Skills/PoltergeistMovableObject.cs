using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoltergeistMovableObject : MonoBehaviour
{
    [HideInInspector]
    public Material HighlightMaterial;
    [HideInInspector]
    public Material FocusMaterial;
    [HideInInspector]
    public float HighlightDistance;

    private Outline outline;
    private Material objectMaterial;
    private MeshRenderer meshRenderer;
    private GameObject player;
    private bool highlighting = false;
    private bool isFocused = false;

    void Start()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;

        meshRenderer = GetComponent<MeshRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");

        objectMaterial = meshRenderer.material;
    }

    public void TurnOnHighlighting()
    {
        highlighting = true;
    }

    public void TurnOffHighlighting()
    {
        highlighting = false;
        isFocused = false;
        outline.enabled = false;
        meshRenderer.material = objectMaterial;
    }

    public void ShowFocus()
    {
        isFocused = true;
        meshRenderer.material = FocusMaterial;
        outline.enabled = true;
    }

    public void HideFocus()
    {
        meshRenderer.material = objectMaterial;
        isFocused = false;
        outline.enabled = false;
    }

    private void Update()
    {
        if (!highlighting || isFocused)
            return;

        if (Vector3.Distance(this.transform.position, player.transform.position) <= HighlightDistance)
            meshRenderer.material = HighlightMaterial;
        else
            meshRenderer.material = objectMaterial;
    }
}
