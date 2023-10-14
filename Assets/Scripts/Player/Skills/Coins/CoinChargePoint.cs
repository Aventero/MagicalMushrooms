using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Outline))]
public class CoinChargePoint : MonoBehaviour
{
    [SerializeField] private int maxChargeValue = 100;
    private int uiCharge = 0;
    private int actualCharge = 0;
    public UnityEvent OnFullyCharged;
    public bool IsGonnaBeFull = false;

    private Renderer rend;
    private MaterialPropertyBlock propBlock;
    private Color initialColor;
    private Outline outline;

    private void Awake()
    {
        // Outline
        outline = GetComponent<Outline>();
        if (outline == null)
            outline = gameObject.AddComponent<Outline>();
        outline.OutlineColor = Color.white;
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.OutlineWidth = 1;

        // Color & Material block
        rend = GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock();
        rend.GetPropertyBlock(propBlock);
        initialColor = rend.material.GetColor("_BaseColor");

        // Color Set to "Off"
        propBlock.SetColor("_BaseColor", new Color(0.3f, 0.3f, 0.3f, 1f)); ;
        rend.SetPropertyBlock(propBlock);
    }

    public void ActualCharge(int chargeAmount)
    {
        actualCharge += chargeAmount;

        // Restore the initial color
        Color lerpColor = Color.Lerp(Color.grey, initialColor, (float)actualCharge / maxChargeValue);
        propBlock.SetColor("_BaseColor", lerpColor);
        rend.SetPropertyBlock(propBlock);

        // Only called once when full!
        if (actualCharge >= maxChargeValue)
        {
            actualCharge = maxChargeValue;
            OnFullyCharged?.Invoke();
        }
    }

    public void UICharge(int chargeAmount)
    {
        uiCharge += chargeAmount;

        // Do nothing when already full
        if (uiCharge >= maxChargeValue)
        {
            IsGonnaBeFull = true;
            return;
        }
    }

    public void OutlineToGreen()
    {
        outline.OutlineColor = Color.green;
    }

    public void ResetOutlineWidth()
    {
        outline.OutlineWidth = 1f;
    }

    public void SetOutlineWidth(float width)
    {
        outline.OutlineWidth = width;
    }

    public float GetCurrentChargeValue()
    {
        return actualCharge;
    }

    public float GetMaxChargeValue()
    {
        return maxChargeValue;
    }
}
