using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData {
    public List<Vector3> visitedCheckpointPositions;
    public Vector3 lastCheckpointPos;
    public Quaternion playerCheckpointRotation;

    public int coins;

    public List<ChargePointData> activatedCoinChargers;
}