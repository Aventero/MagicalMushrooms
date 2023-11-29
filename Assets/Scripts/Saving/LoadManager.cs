using System;
using System.Collections;
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
        StartCoroutine(LoadDelayed());
    }

    IEnumerator LoadDelayed()
    {
        yield return new WaitForEndOfFrame();
        Debug.Log("Loading Save");
        Stats.Instance.CoinsCollected = 0;
        SaveData saveData = JsonUtility.FromJson<SaveData>(ReadFile());
        
        Stats.Instance.InitializeCoins(saveData.coins);
        LoadActivatedCoinCharger(saveData.activatedCoinChargers);
        LoadVisitedCheckpoints(saveData.activeCheckpoint, saveData.playerCheckpointRotation);
        LoadCollectedMushrooms(saveData.collectedMushrooms);
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
                    chargePoint.LoadChargePoint(savedChargedPoint.CoinValue);
                }
            }
        }
    }

    private void LoadVisitedCheckpoints(Vector3 activeCheckpoint, Quaternion playerRotation)
    {
        Checkpoint[] checkpoints = GameObject.FindObjectsOfType<Checkpoint>();
        foreach(Checkpoint checkpoint in checkpoints)
        {
            if(checkpoint.GetRespawnPoint().Equals(activeCheckpoint))
            {
                Debug.Log("Found checkpoint");
                checkpoint.SetRotation(playerRotation);
                CheckpointManager.Instance.Checkpoint = checkpoint;
                CheckpointManager.Instance.RespawnPlayer();

                checkpoint.SetActivated(true);
                return;
            }
        }

        Debug.LogError("No checkpoint found");
    }

    private void LoadCollectedMushrooms(List<Vector3> collectedMushrooms)
    {
        MushroomCollectable[] mushroomCollectables = GameObject.FindObjectsOfType<MushroomCollectable>();

        foreach(MushroomCollectable mushroom in mushroomCollectables)
        {
            foreach(Vector3 collectedPosition in collectedMushrooms)
            {
                if (mushroom.transform.position.Equals(collectedPosition))
                {
                    mushroom.Collect();
                    break;
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
