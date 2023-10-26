using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CoinVacuum : MonoBehaviour
{
    [Header("Vacuum Settings")]
    public Transform vacuumOrigin;   // The point towards which coins will be dragged
    public float vacuumSearchRadius = 5f;  // The effective radius of the vacuum
    public float vacuumForce = 5f;   // The force/speed at which coins are pulled towards the vacuum center
    private HashSet<Coin> activeCoins = new();
    public bool MouseHeld { get; private set; }
    [Range(0, 90)] public float angle = 45f; // Start angle for the vacuum zone (relative to forward direction)

    private GlassSlurpSpin glassSlurpSpin;
    private PlayerSkillManager skillManager;

    private void Start()
    {
        glassSlurpSpin = GetComponentInChildren<GlassSlurpSpin>();
        skillManager = GameObject.FindObjectOfType<PlayerSkillManager>();
    }

    private void Update()
    {
        // currently not holding rmb
        if (!MouseHeld)
        {
            //if (skillManager.AreSkillsLocked())
            //    skillManager.UnlockSkills();

            List<Coin> coinsToRemove = new List<Coin>();

            // Unjiggle coins
            foreach (Coin coin in activeCoins)
            {
                coin.UnJiggle();
                if (coin.InInitialPosition)
                    coinsToRemove.Add(coin);
            }

            foreach (Coin coin in coinsToRemove)
            {
                activeCoins.Remove(coin);
            }
        }
        else
        {
            Execute();

            if(!skillManager.AreSkillsLocked())
                skillManager.LockSkills();
        }
    }

    public void Input(InputAction.CallbackContext callback)
    {
        if (skillManager.HasActiveSkill())
            return;

        if (callback.started)
        {
            StateManager.Instance.StartSlurpingEvent.Invoke();
            glassSlurpSpin.StartAnimating();
            MouseHeld = true;
        }
        else if(callback.canceled)
        {
            StateManager.Instance.EndSlurpingEvent.Invoke();
            glassSlurpSpin.StopAnimating();
            MouseHeld = false;
        }
    }

    public void Execute()
    {
        // Find coins within the vacuum radius at the time of execution
        Collider[] hitColliders = Physics.OverlapSphere(vacuumOrigin.position, vacuumSearchRadius);
        foreach (Collider hitCollider in hitColliders)
        {
            // Slurp
            Coin coin = hitCollider.GetComponent<Coin>();
            if (coin == null)
                continue;

            Vector3 toCoin = coin.transform.position - Camera.main.transform.position;
            float angleToCoin = Vector3.Angle(Camera.main.transform.forward, toCoin);
            if (angleToCoin <= angle && toCoin.magnitude <= vacuumSearchRadius)
                activeCoins.Add(coin);
        }

        foreach (Coin coin in activeCoins)
        {
            coin.Jiggle(vacuumOrigin, vacuumForce);
        }
    }
}
