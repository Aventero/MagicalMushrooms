using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CoinChargePoint : MonoBehaviour
{
    [SerializeField] private int maxChargeValue = 100;  
    private int uiCharge = 0;
    private int actualCharge = 0;
    public UnityEvent OnFullyCharged;
    public bool IsGonnaBeFull = false;

    public void ActualCharge(int chargeAmount)
    {
        actualCharge += chargeAmount;

        // Only called once when full!
        if (actualCharge >= maxChargeValue)
        {
            actualCharge = maxChargeValue;
            OnFullyCharged?.Invoke();
            Debug.Log("FULLY CHARGED!");
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

    public void ChangeOutline()
    {
        GetComponent<Outline>().OutlineColor = Color.green;
    }

    public float GetCurrentChargeValue()
    {
        return uiCharge;
    }

    public float GetMaxChargeValue()
    {
        return maxChargeValue;
    }
}
