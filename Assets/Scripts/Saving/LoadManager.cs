using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadManager : MonoBehaviour
{
    private readonly string saveFile = "Savedata.json";
    private readonly string loadSave = "LoadSave";

    public void Start()
    {
        if (Convert.ToBoolean(PlayerPrefs.GetInt(loadSave)))
            LoadSave();
    }

    public void LoadSave()
    {
        Debug.Log("Loading Save");
        SaveData saveData = JsonUtility.FromJson<SaveData>(ReadFile());

        FindObjectOfType<Stats>().IncreaseCoinsCollected(saveData.coins);
        LoadVisitedCheckpoints(saveData.visitedCheckpointPositions, saveData.lastCheckpointPos, saveData.playerCheckpointRotation);
        LoadActivatedCoinCharger(saveData.activatedCoinChargers);
    }

    private void LoadActivatedCoinCharger(List<ChargePointData> activatedCoinCharger)
    {
        CoinChargePoint[] coinChargers = FindObjectsOfType<CoinChargePoint>();

        foreach(ChargePointData savedChargedPoint in activatedCoinCharger)
        {
            foreach(CoinChargePoint chargePoint in coinChargers)
            {
                if (savedChargedPoint.ChargePointID.Equals(chargePoint.GetID()))
                {
                    chargePoint.ActualCharge(savedChargedPoint.CoinValue);
                }
            }
        }
    }

    private void LoadVisitedCheckpoints(List<Vector3> visitedCheckpoints, Vector3 activeCheckpoint, Quaternion playerRotation)
    {
        CheckpointManager checkpointManager = FindObjectOfType<CheckpointManager>();
        Checkpoint[] checkpoints = GameObject.FindObjectsOfType<Checkpoint>();
        foreach(Checkpoint checkpoint in checkpoints)
        {
            foreach(Vector3 checkpointPos in visitedCheckpoints)
            {
                if (checkpoint.GetRespawnPoint().Equals(checkpointPos))
                {
                    if(checkpoint.GetRespawnPoint().Equals(activeCheckpoint))
                    {
                        checkpoint.SetRotation(playerRotation);
                        checkpointManager.Checkpoint = checkpoint;
                        checkpointManager.RespawnPlayer();
                    }

                    checkpoint.SetActivated(true);
                }
            }
            
        }
    }

    private string ReadFile()
    {
        return File.ReadAllText(FilePath());
    }

    private string FilePath()
    {
        return Application.persistentDataPath + "/" + saveFile;
    }
}
