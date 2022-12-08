using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static int MaxHealth = 3;
    [HideInInspector] public int CurrentHealth = MaxHealth;

    private void Start()
    {
        CurrentHealth = MaxHealth;

        StateManager.Instance.DealDamageEvent += this.DealDamage;
    }

    public void DealDamage(int damage)
    {
        // Deal Damage
        CurrentHealth -= damage;

        // Health will always be within 0 and MaxHealth
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth); 

        // Update UI
        UIManager.Instance.UpdateHealthIcons(CurrentHealth);
    }
}