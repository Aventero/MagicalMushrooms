using UnityEngine;

public class ChargePointManager : MonoBehaviour
{
    private void Awake()
    {
        int numberOfDuplicates = 0;
        CoinChargePoint[] coinchargers = FindObjectsOfType<CoinChargePoint>();

        foreach(CoinChargePoint chargePoint in coinchargers)
        {
            foreach (CoinChargePoint secondChargePoint in coinchargers)
            {
                if (chargePoint == secondChargePoint)
                    continue;

                if (chargePoint.GetID().Equals(secondChargePoint.GetID()))
                {
                    Debug.LogError("Found Duplicate on \"" + chargePoint.name + "\" and \"" + secondChargePoint.name + "\"");
                    numberOfDuplicates++;
                }
            }
        }

        if (numberOfDuplicates > 0)
            Debug.LogError(numberOfDuplicates + "/" + coinchargers.Length + " CoinChargePoints have the same ID. ");
        else
            Debug.Log("No Charge Point Duplicates Found!");
    }
}
