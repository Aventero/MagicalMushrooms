
using UnityEngine;

public class AdjustRenderQueue : MonoBehaviour
{
    public int renderQueue = 3000; // Default is 2000

    void Start()
    {
        GetComponent<Renderer>().material.renderQueue = renderQueue;
    }
}