using UnityEngine;

public class DissolveController : MonoBehaviour
{
    private string dissolveShaderPath = "Shaders/Dissolve";
    public Shader dissolveShader;
    public float dissolveSpeed = 0.5f;

    private float dissolveAmount = 0f;
    private Renderer rend;
    private MaterialPropertyBlock propBlock;
    private Shader originalShader;
    private int seed = 0;
    [ColorUsageAttribute(true, true)]
    public Color color = new Color(1f, 1f, 1f);

    void Awake()
    {
        rend = GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock();
        originalShader = rend.material.shader;
        dissolveShader = Resources.Load(dissolveShaderPath) as Shader;
        if (dissolveShader == null)
        {
            Debug.LogError("Failed to load dissolve shader!");
            return;
        }
        seed =  (int)(Random.value * 1000);
        rend.material.shader = dissolveShader;
        rend.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_DissolveAmount", 0);
        propBlock.SetFloat("_DissolveScale", 50f);
        propBlock.SetFloat("_DissolveEdgeWidth", 0.01f);
        propBlock.SetColor("_Color", color);
        propBlock.SetFloat("_Seed", seed);
        rend.SetPropertyBlock(propBlock);
    }

    void Update()
    {
        dissolveAmount += dissolveSpeed * Time.deltaTime;
        dissolveAmount = Mathf.Clamp(dissolveAmount, 0, 1.0f);

        rend.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_DissolveAmount", dissolveAmount);
        rend.SetPropertyBlock(propBlock);

        if (dissolveAmount >= 1.0f)
        {
            RestoreOriginalShader();
        }
    }

    void RestoreOriginalShader()
    {
        rend.material.shader = originalShader;
        gameObject.SetActive(false);
        Destroy(this);
    }
}
