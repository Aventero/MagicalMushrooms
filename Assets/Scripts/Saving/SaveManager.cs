using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    private readonly string filename = "Savedata.json";

    private GameObject player;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
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

        return new()
        {
            coins = Stats.Instance.CoinsCollected,
            collectedMushrooms = Stats.Instance.CollectedMushrooms,
            activeCheckpoint = currentCheckpoint.GetRespawnPoint(),
            playerCheckpointRotation = currentCheckpoint.GetRotation(),
            activatedCoinChargers = GetActivatedMovablePlatforms()
        };
    }

    private List<ChargePointData> GetActivatedMovablePlatforms()
    {
        CoinChargePoint[] coinChargers = FindObjectsOfType<CoinChargePoint>();
        List<ChargePointData> activatedCoinCharger = new();

        foreach (CoinChargePoint coinChargePoint in coinChargers)
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
