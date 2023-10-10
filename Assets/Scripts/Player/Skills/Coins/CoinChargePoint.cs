using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CoinChargePoint : MonoBehaviour
{
    [SerializeField] private int maxChargeValue = 100;  
    private int currentChargeValue = 0;
    public UnityEvent OnFullyCharged;
    public bool IsFullyCharged = false;

    public void Charge(int chargeAmount)
    {
        // Do nothing when already full
        if (currentChargeValue >= maxChargeValue)
            return;

        currentChargeValue += chargeAmount;

        // Only called once when full!
        if (currentChargeValue >= maxChargeValue)
        {
            currentChargeValue = maxChargeValue;
            OnFullyCharged?.Invoke();
            IsFullyCharged = true;
        }
    }

    public void ChangeOutline()
    {
        GetComponent<Outline>().OutlineColor = Color.green;
    }

    public float GetCurrentChargeValue()
    {
        return currentChargeValue;
    }

    public float GetMaxChargeValue()
    {
        return maxChargeValue;
    }
}
