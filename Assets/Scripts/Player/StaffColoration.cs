using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class StaffColoration : MonoBehaviour
{
    private MeshRenderer rend;
    private MaterialPropertyBlock propBlock;
    public float ballBoundsY;
    MeshFilter meshFilter;

    void Awake()
    {
        rend = GetComponent<MeshRenderer>();
        propBlock = new MaterialPropertyBlock();
        meshFilter = GetComponent<MeshFilter>();
    }

    void Update()
    {
        Vector3 boundsSize = meshFilter.mesh.bounds.size;
        rend.GetPropertyBlock(propBlock);
        propBlock.SetVector("_ObjectBounds", boundsSize);
        propBlock.SetFloat("_Height", Stats.Instance.GetNormalizedCoins());
        rend.SetPropertyBlock(propBlock);
    }
}