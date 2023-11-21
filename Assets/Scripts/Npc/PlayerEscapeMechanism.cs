using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerEscapeMechanism : MonoBehaviour
{
    public UnityEvent OnCanEscape;
    public UnityEvent OnCanEscapeMaxCoins;
    public UnityEvent OnCanNotEscape;
    public int EscapeMushrooms = 7;

    public void Start()
    {
    }

    public void CheckEscape()
    {
        // Max Coins
        if (Stats.Instance.MushroomsCollected >= Stats.Instance.MaxMushrooms)
        {
            OnCanEscapeMaxCoins.Invoke();
            OnCanEscape.RemoveAllListeners();
            return;
        }

        // Enough Coins
        if (Stats.Instance.MushroomsCollected >= EscapeMushrooms)
        {
            OnCanEscape.Invoke();
            OnCanEscape.RemoveAllListeners();
            return;
        }
        
        // Not Enough Coins
        OnCanNotEscape.Invoke();
    }
}
