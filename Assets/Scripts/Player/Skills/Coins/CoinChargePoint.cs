using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Outline))]
public class CoinChargePoint : MonoBehaviour
{
    [SerializeField] private int maxChargeValue = 100;
    private int uiCharge = 0;
    private int actualCharge = 0;
    public string FullyChargedText = "Fully Charged.";
    public string ChargingTextBeforeValue = "Currently has ";
    public string ChargingTextAfterValue = " charged.";

    public UnityEvent OnFullyCharged;
    public bool IsGonnaBeFull = false;
    public string CoinChargerID;
    public bool HasToBeSaved = true;

    private Outline outline;
    public bool ShouldEnableOutline = true;
    public bool IsFullyCharged { get; private set; }

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        // Outline
        actualCharge = 0;
        uiCharge = 0;
        IsFullyCharged = false;
        IsGonnaBeFull = false;
        outline = GetComponent<Outline>();
        if (outline == null)
            outline = gameObject.AddComponent<Outline>();
        outline.OutlineColor = Color.white;
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.OutlineWidth = 1;

        if (ShouldEnableOutline) 
            outline.enabled = true;
        else
            outline.enabled = false;

        if (string.IsNullOrEmpty(CoinChargerID))
            Debug.LogError("The CoinChargerID is null for this object: " + gameObject.name);
    }

    public void ActualCharge(int chargeAmount)
    {
        actualCharge += chargeAmount;

        // Only called once when full!
        if (actualCharge >= maxChargeValue)
        {
            AudioManager.Instance.Play("chargepointCharged");
            actualCharge = maxChargeValue;
            outline.enabled = false;
            OnFullyCharged?.Invoke();
            IsFullyCharged = true;

            if (HasToBeSaved)
                FindObjectOfType<SaveManager>().AddCompletedChargePoint(this);
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

    public int GetCurrentChargeValue()
    {
        return actualCharge;
    }

    public int GetUIChargeValue()
    {
        return uiCharge;
    }

    public int GetMaxChargeValue()
    {
        return maxChargeValue;
    }

    public string GetID()
    {
        return CoinChargerID;
    }
}
