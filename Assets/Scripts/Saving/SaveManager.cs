using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    private readonly string filename = "Savedata.json";
    public List<CoinChargePoint> activeCoinChargePoints = new();

    public void Start()
    {
        StateManager.Instance.NewCheckpointEvent.AddListener(SaveGame);
    }

    public void SaveGame()
    {
        Debug.Log("Saving Game!");
        string json = JsonUtility.ToJson(SetupSaveData());
        WriteToFile(json);

        PlayerPrefs.SetString("LastSavedScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
    }

    private SaveData SetupSaveData()
    {
        Checkpoint currentCheckpoint = FindObjectOfType<CheckpointManager>().Checkpoint;

        if (currentCheckpoint != null)
        {
            Debug.Log("Checkpoint: " + currentCheckpoint.GetRespawnPoint());
            Debug.Log("Checkpoint Rotation: " + currentCheckpoint.GetRotation());
            Debug.Log("Coins Collected: " + Stats.Instance.CoinsCollected);
            Debug.Log("Mushrooms Collected: " + Stats.Instance.CollectedMushrooms);
            Debug.Log("Active Checkpoint: " + currentCheckpoint.GetRespawnPoint());
            Debug.Log("Active Coin Chargers: " + GetActivatedChargePoints());
        } 
        else
        {
            Debug.Log("Checkpoint is null");
        }

        return new()
        {
            coins = Stats.Instance.CoinsCollected,
            collectedMushrooms = Stats.Instance.CollectedMushrooms,
            activeCheckpoint = currentCheckpoint.GetRespawnPoint(),
            playerCheckpointRotation = currentCheckpoint.GetRotation(),
            activatedCoinChargers = GetActivatedChargePoints()
        };
    }

    public void AddCompletedChargePoint(CoinChargePoint coinChargePoint)
    {
        activeCoinChargePoints.Add(coinChargePoint);
    }

    private List<ChargePointData> GetActivatedChargePoints()
    {
        List<ChargePointData> activatedCoinCharger = new();

        foreach (CoinChargePoint coinChargePoint in activeCoinChargePoints)
        {
            if( coinChargePoint.GetCurrentChargeValue() > 0)
            {
                activatedCoinCharger.Add(new ChargePointData(){
                    ChargePointID = coinChargePoint.GetID(),
                    CoinValue = coinChargePoint.GetCurrentChargeValue()
                });
            }
        }

        return activatedCoinCharger;
    }

    private void WriteToFile(string json)
    {
        string filePath = Application.persistentDataPath + "/" + filename;
        Debug.Log("FilePath: " + filePath);
        FileStream fileStream = new(filePath, FileMode.Create);

        using(StreamWriter writer = new(fileStream))
        {
            writer.Write(json);
        }
    }
}
