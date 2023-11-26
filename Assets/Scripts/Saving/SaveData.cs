using System.Collections.Generic;
using UnityEngine;

public class SaveData {
    public Vector3 activeCheckpoint;
    public Quaternion playerCheckpointRotation;

    public int coins;

    public List<ChargePointData> activatedCoinChargers;
}