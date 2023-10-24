using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CoinVacuum : MonoBehaviour
{
    [Header("Vacuum Settings")]
    public Transform vacuumCenter;   // The point towards which coins will be dragged
    public float vacuumSearchRadius = 5f;  // The effective radius of the vacuum
    public float vacuumForce = 5f;   // The force/speed at which coins are pulled towards the vacuum center
    private HashSet<Coin> activeCoins = new();
    public bool MouseHeld { get; private set; }
    [Range(0, 1)] public float slurpRadius = 1f; // Start angle for the vacuum zone (relative to forward direction)

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
            if (skillManager.AreSkillsLocked())
                skillManager.UnlockSkills();

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
        //// Find coins within the vacuum radius at the time of execution
        //Collider[] hitColliders = Physics.OverlapSphere(vacuumCenter.position, vacuumSearchRadius);
        //foreach (Collider hitCollider in hitColliders)
        //{
        //    // Slurp
        //    Coin coin = hitCollider.GetComponent<Coin>();
        //    if (coin != null)
        //    {
        //        Vector3 toCoin = coin.transform.position - Camera.main.transform.position;

        //        // Find the projection (or closest point) of the coin's position onto the line of the camera's forward direction
        //        Vector3 projection = Camera.main.transform.position + Vector3.Dot(toCoin, Camera.main.transform.forward) * Camera.main.transform.forward;

        //        // Compute the distance between the coin and the projection point
        //        float distanceFromLine = Vector3.Distance(projection, coin.transform.position);

        //        // Check if this distance is within the desired limit (cylinder's radius)
        //        if (distanceFromLine <= vacuumSearchRadius)
        //        {
        //            activeCoins.Add(coin);
        //        }
        //    }
        //}

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit[] hits;

        float maxDistance = vacuumSearchRadius; // Length of the cylinder
        float sphereRadius = slurpRadius; // Radius of the cylinder

        hits = Physics.SphereCastAll(ray, sphereRadius, maxDistance);

        foreach (RaycastHit hit in hits)
        {
            Coin coin = hit.collider.GetComponent<Coin>();
            if (coin != null)
            {
                activeCoins.Add(coin);
            }
        }

        foreach (Coin coin in activeCoins)
        {
            coin.Jiggle(vacuumCenter, vacuumForce);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        // The start and end points of the sphere cast
        Vector3 start = Camera.main.transform.position;
        Vector3 end = start + Camera.main.transform.forward * vacuumSearchRadius;

        // Draw the central line of the cylinder
        Gizmos.DrawLine(start, end);

        // Draw circles at the start and end to represent the sphere's path
        Gizmos.DrawWireSphere(start, slurpRadius);
        Gizmos.DrawWireSphere(end, slurpRadius);
    }
}
