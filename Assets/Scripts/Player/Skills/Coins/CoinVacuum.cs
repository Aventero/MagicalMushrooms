using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CoinVacuum : MonoBehaviour
{
    [Header("Vacuum Settings")]
    public Transform vacuumCenter;   // The point towards which coins will be dragged
    public float vacuumRadius = 5f;  // The effective radius of the vacuum
    public float vacuumForce = 5f;   // The force/speed at which coins are pulled towards the vacuum center
    private HashSet<Coin> activeCoins = new();
    private CoinCharger coinCharger;
    public bool MouseHeld { get; private set; }
    [Range(0, 90)] public float angle = 45f; // Start angle for the vacuum zone (relative to forward direction)

    private void Start()
    {
        coinCharger = GetComponent<CoinCharger>();
    }

    private void Update()
    {
        // currently not holding rmb
        if (!MouseHeld)
        {
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
            coinCharger.Charge();
        }
        
    }

    public void Input(InputAction.CallbackContext callback)
    {
        if (callback.started)
            MouseHeld = true;
        else if(callback.canceled)
            MouseHeld = false;
    }

    public void Execute()
    {
        // Find coins within the vacuum radius at the time of execution
        Collider[] hitColliders = Physics.OverlapSphere(vacuumCenter.position, vacuumRadius);
        foreach (Collider hitCollider in hitColliders)
        {
            Coin coin = hitCollider.GetComponent<Coin>();
            if (coin != null)
            {

                Vector3 toCoin = coin.transform.position - vacuumCenter.position;
                float angleToCoin = Vector3.Angle(vacuumCenter.forward, toCoin);

                // Assuming "angle" is half of the total angle of the cone
                if (angleToCoin <= angle && toCoin.magnitude <= vacuumRadius)
                {
                    activeCoins.Add(coin);
                }
            }
        }

        foreach (Coin coin in activeCoins)
        {
            coin.Jiggle(vacuumCenter, vacuumForce);
        }
    }

    private void OnDrawGizmos()
    {
        if (vacuumCenter)
        {
            // Draw the circle for the base radius
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(vacuumCenter.position, vacuumRadius);

            // Draw lines to represent the angles
            Gizmos.color = Color.red;

            // The direction of the start and end angles in the Y axis
            Vector3 startDirY = Quaternion.Euler(0, -angle, 0) * vacuumCenter.forward.normalized;
            Vector3 endDirY = Quaternion.Euler(0, angle, 0) * vacuumCenter.forward.normalized;

            // The direction of the start and end angles in the X axis
            Vector3 startDirX = Quaternion.Euler(-angle, 0, 0) * vacuumCenter.forward.normalized;
            Vector3 endDirX = Quaternion.Euler(angle, 0, 0) * vacuumCenter.forward.normalized;

            // Draw the Y rotation boundaries
            Gizmos.DrawLine(vacuumCenter.position, vacuumCenter.position + startDirY * vacuumRadius);
            Gizmos.DrawLine(vacuumCenter.position, vacuumCenter.position + endDirY * vacuumRadius);

            // Draw the X rotation boundaries
            Gizmos.DrawLine(vacuumCenter.position, vacuumCenter.position + startDirX * vacuumRadius);
            Gizmos.DrawLine(vacuumCenter.position, vacuumCenter.position + endDirX * vacuumRadius);

            // Draw a series of lines to represent the cone in the XZ and YZ planes
            int segments = 10; // adjust this for more detailed visualization
            for (int i = 1; i <= segments; i++)
            {
                Vector3 dirY = Quaternion.Euler(0, -angle + (2 * angle * i / segments), 0) * vacuumCenter.forward.normalized;
                Vector3 dirX = Quaternion.Euler(-angle + (2 * angle * i / segments), 0, 0) * vacuumCenter.forward.normalized;

                Gizmos.DrawLine(vacuumCenter.position, vacuumCenter.position + dirY * vacuumRadius);
                Gizmos.DrawLine(vacuumCenter.position, vacuumCenter.position + dirX * vacuumRadius);
            }
        }
    }
}
