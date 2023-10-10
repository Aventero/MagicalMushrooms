using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CoinCharger : MonoBehaviour
{
    [SerializeField] private float rayDistance = 3f; // The distance the ray should travel
    private float maxChargeCooldown = 0.02f;    
    private float currentChargeCooldown = 0f;
    public OverlayMenu OverlayMenu;
    public string ToolTipText = "Charge";
    public CoinChargePoint chargePoint;
    public Transform vacuumCenter;   // The point towards which coins will spawn

    [Header("Flying Object Properties")]
    public GameObject magicCoinPrefab; // Prefab of the object you want to spawn and make it fly
    public float spawnForce = 5f;         // Initial force applied when spawning the object

    // Function to spawn and start the flight of the object
    public void SpawnAndFlyObject()
    {
        GameObject obj = Instantiate(magicCoinPrefab, vacuumCenter.position, Quaternion.identity);
        Coin coin = obj.GetComponent<Coin>();

        // Calculate a new target jiggle position around the initial position
        Vector3 jiggleDirection = GenerateRandomDirection(vacuumCenter.forward);
        coin.ChargeRoutine(chargePoint.transform, 0.1f, jiggleDirection, new Vector3(0.001f, 0.001f, 0.001f));
    }

    public Vector3 GenerateRandomDirection(Vector3 targetDirection, float maxAngleDeviation = 90f)
    {
        targetDirection.Normalize();
        Quaternion randomRotation = Quaternion.AngleAxis(Random.Range(0, maxAngleDeviation), Random.onUnitSphere);
        Vector3 randomDirection = randomRotation * targetDirection;
        return randomDirection.normalized;
    }

    private void FixedUpdate()
    {
        List<CoinChargePoint> targets = FindAllTargetsWithScript<CoinChargePoint>();

        if (targets.Count > 0 && chargePoint == null)
            SetCharger(targets[0]);
        else if (targets.Count == 0 && chargePoint != null)
            LoseCharger();
    }

    private void SetCharger(CoinChargePoint chargePoint)
    {
        if (chargePoint != null)
        {
            this.chargePoint = chargePoint;
            OverlayMenu.ShowTooltip("Charge: " + chargePoint.GetCurrentChargeValue().ToString() + " / " + chargePoint.GetMaxChargeValue().ToString(), MouseSide.RightClick);
        }
    }

    private void LoseCharger()
    {
        chargePoint = null;
        OverlayMenu.HideTooltip();
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
                return;
            chargePoint.Charge(1);
            SpawnAndFlyObject();
            Stats.Instance.DecreaseCoinsCollected(1);
            currentChargeCooldown = 0f;
            OverlayMenu.UpdateCurrentTooltip("Charge: " + chargePoint.GetCurrentChargeValue().ToString() + " / " + chargePoint.GetMaxChargeValue().ToString());
        }
    }



    private List<T> FindAllTargetsWithScript<T>() where T : MonoBehaviour
    {
        // Shoot raycast from center of screen and get the scripts with the T script
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit[] hits = Physics.RaycastAll(ray, rayDistance);
        List<T> foundScripts = new();
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.TryGetComponent<T>(out var script))
                foundScripts.Add(script);
        }

        return foundScripts;
    }
}
