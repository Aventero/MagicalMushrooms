using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class MagicLiquid : MonoBehaviour
{
    private MeshRenderer rend;
    private MaterialPropertyBlock propBlock;
    MeshFilter meshFilter;
    private Color originalColor;

    [Header("Liquid")]
    private Vector3 lastPos;
    private Vector3 velocity;
    private Vector3 lastRot;
    private Vector3 angularVelocity;

    public float MaxWobble = 0.03f;
    public float WobbleSpeed = 1f;
    public float Recovery = 1f;

    private float wobbleAmountX;
    private float wobbleAmountZ;
    private float wobbleAmountToAddX;
    private float wobbleAmountToAddZ;
    private float wobbleSine;
    private float wobbleTime = 0.5f;

    void Awake()
    {
        rend = GetComponent<MeshRenderer>();
        propBlock = new MaterialPropertyBlock();
        meshFilter = GetComponent<MeshFilter>();
    }

    private void Start()
    {

    }

    private void Update()
    {
        Vector3 boundsSize = meshFilter.mesh.bounds.size;
        rend.GetPropertyBlock(propBlock);
        propBlock.SetVector("_ObjectBounds", boundsSize);
        propBlock.SetFloat("_Height", Stats.Instance.GetNormalizedCoins());
        rend.SetPropertyBlock(propBlock);

        if (Stats.Instance.CoinsCollected <= 1 || Stats.Instance.CoinsCollected >= Stats.Instance.MaxCoins)
            return;

        if (Time.timeScale <= 0.7f)
            return;

        WobbleLiquid();
    }

    private void WobbleLiquid()
    {
        wobbleTime += Time.deltaTime;

        // Decrease wobble over time
        wobbleAmountToAddX = Mathf.Lerp(wobbleAmountToAddX, 0, Time.deltaTime * Recovery);
        wobbleAmountToAddZ = Mathf.Lerp(wobbleAmountToAddZ, 0, Time.deltaTime * Recovery);

        // Make a sine wave of the decreasing wobble
        wobbleSine = 2 * Mathf.PI * WobbleSpeed;
        wobbleAmountX = wobbleAmountToAddX * Mathf.Sin(wobbleSine * wobbleTime);
        wobbleAmountZ = wobbleAmountToAddZ * Mathf.Sin(wobbleSine * wobbleTime);

        // Send it to the shader using the MaterialPropertyBlock
        rend.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_WobbleX", wobbleAmountX);
        propBlock.SetFloat("_WobbleZ", wobbleAmountZ);
        rend.SetPropertyBlock(propBlock);

        // Calculate velocity
        velocity = (lastPos - transform.position) / Time.deltaTime;
        angularVelocity = transform.rotation.eulerAngles - lastRot;

        // Add clamped velocity to wobble
        wobbleAmountToAddX += Mathf.Clamp((velocity.x + (angularVelocity.z * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);
        wobbleAmountToAddZ += Mathf.Clamp((velocity.z + (angularVelocity.x * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);

        // Store the last position and rotation
        lastPos = transform.position;
        lastRot = transform.rotation.eulerAngles;
    }

    public void MagicReachedOrigin()
    {
        Vector3 boundsSize = meshFilter.mesh.bounds.size;
        rend.GetPropertyBlock(propBlock);
        propBlock.SetVector("_ObjectBounds", boundsSize);
        propBlock.SetFloat("_Height", Stats.Instance.GetNormalizedCoins());
        rend.SetPropertyBlock(propBlock);
        StartCoroutine(GlowEffect());
    }

    private IEnumerator GlowEffect()
    {
        Color targetColor = originalColor * 1.5f; // Increase the brightness
        float duration = 0.05f; // Duration of the effect
        float holdDuration = 0.05f; // Duration to hold the full glow
        float progress = 0;

        while (progress < 1)
        {
            Color currentColor = Color.Lerp(originalColor, targetColor, progress);
            propBlock.SetColor("_BaseColor", currentColor);
            rend.SetPropertyBlock(propBlock);
            progress += Time.deltaTime / duration;
            yield return null;
        }

        propBlock.SetColor("_BaseColor", targetColor);
        rend.SetPropertyBlock(propBlock);
        yield return new WaitForSeconds(holdDuration);

        // Resetting the emission
        progress = 0;
        while (progress < 1)
        {
            Color currentColor = Color.Lerp(targetColor, originalColor, progress);
            propBlock.SetColor("_BaseColor", currentColor);
            rend.SetPropertyBlock(propBlock);
            progress += Time.deltaTime / duration;
            yield return null;
        }

        propBlock.SetColor("_BaseColor", originalColor);
        rend.SetPropertyBlock(propBlock);
    }
}