using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CoinChargePoint : MonoBehaviour
{
    [SerializeField] private int maxChargeValue = 100;  
    private int currentChargeValue = 0;
    public UnityEvent OnFullyCharged;

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
        }
    }

    public void FULL()
    {
        Debug.Log("IS FULL!");
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
