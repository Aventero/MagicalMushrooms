using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CoinCharger : MonoBehaviour
{
    [SerializeField] private float rayDistance = 3f; // The distance the ray should travel
    private float maxChargeCooldown = 0.05f;    
    private float currentChargeCooldown = 0f;
    public string ToolTipText = "Charge";
    public CoinChargePoint chargePoint;
    public Transform vacuumCenter;   // The point towards which coins will spawn
    public bool MouseHeld { get; private set; }

    [Header("Flying Object Properties")]
    public GameObject magicCoinPrefab; // Prefab of the object you want to spawn and make it fly
    public float spawnForce = 5f;         // Initial force applied when spawning the object
    public float OutlineWidth = 2f;

    // Function to spawn and start the flight of the object
    public void ChargeObject()
    {
        GameObject obj = Instantiate(magicCoinPrefab, vacuumCenter.position, Quaternion.identity);
        Coin coin = obj.GetComponent<Coin>();

        // Calculate a new target jiggle position around the initial position
        Vector3 jiggleDirection = GenerateRandomDirection(vacuumCenter.forward);
        coin.SetChargePoint(chargePoint);
        coin.FlyAndCharge(chargePoint.transform, 0.1f, jiggleDirection, new Vector3(0.001f, 0.001f, 0.001f));
    }

    public Vector3 GenerateRandomDirection(Vector3 targetDirection, float maxAngleDeviation = 90f)
    {
        targetDirection.Normalize();
        Quaternion randomRotation = Quaternion.AngleAxis(Random.Range(0, maxAngleDeviation), Random.onUnitSphere);
        Vector3 randomDirection = randomRotation * targetDirection;
        return randomDirection.normalized;
    }

    public void Input(InputAction.CallbackContext callback)
    {
        if (callback.started)
        {
            MouseHeld = true;
        }
        else if (callback.canceled)
        {
            MouseHeld = false;
        }
    }

    private void FixedUpdate()
    {
        ToggleCharger();

        if (chargePoint == null)
            return;


        if (MouseHeld)
            Charge();

        if (chargePoint.GetCurrentChargeValue() == chargePoint.GetMaxChargeValue())
        {
            UIManager.Instance.ShowChargeTooltip("Charged!");
        }
        else
        {
            UIManager.Instance.ShowChargeTooltip("Charging " + chargePoint.GetCurrentChargeValue().ToString() + " / " + chargePoint.GetMaxChargeValue().ToString());
        }
    }

    private void ToggleCharger()
    {
        CoinChargePoint target = FindClosestTargetWithScript<CoinChargePoint>();

        if (target != null)
        {
            if (chargePoint == null)
            {
                SetCharger(target);
            }
            else if (chargePoint != target)
            {

                LoseCharger();
                SetCharger(target);
            }
        }
        else if (chargePoint != null)
        {
            LoseCharger();
        }
    }

    private void SetCharger(CoinChargePoint chargePoint)
    {
        if (chargePoint != null)
        {
            this.chargePoint = chargePoint;
            UIManager.Instance.ShowChargeTooltip("Charge: " + chargePoint.GetCurrentChargeValue().ToString() + " / " + chargePoint.GetMaxChargeValue().ToString(), MouseSide.LeftClick);
            chargePoint.SetOutlineWidth(OutlineWidth);
        }
    }

    private void LoseCharger()
    {
        chargePoint.ResetOutlineWidth();
        chargePoint = null;

        if (UIManager.Instance.GetActiveToolTipType() == ToolTipType.Charge)
            UIManager.Instance.HideTooltip();
    }


    public void Charge()
    {
        if (chargePoint == null)
            return;

        currentChargeCooldown += Time.deltaTime;
        
        // Only charge every 0.1 seconds, so its not instant
        if (currentChargeCooldown >= maxChargeCooldown)
        {
            if (Stats.Instance.CoinsCollected <= 0)
            {
                Stats.Instance.MissingCoinsEffect();
                return;
            }

            if (chargePoint.IsGonnaBeFull)
                return;

            ChargeObject();
            Stats.Instance.DecreaseCoinsCollected(1);
            currentChargeCooldown = 0f;
        }
    }



    private T FindClosestTargetWithScript<T>() where T : MonoBehaviour
    {
        // Shoot raycast from center of screen and get the scripts with the T script
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit[] hits = Physics.RaycastAll(ray, rayDistance);

        T closestScript = null;
        float minDistance = float.MaxValue; // Start with the maximum value so any distance will be less than this

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.TryGetComponent<T>(out var script))
            {
                if (hit.distance < minDistance)
                {
                    minDistance = hit.distance;
                    closestScript = script;
                }
            }
        }

        return closestScript;
    }
}
