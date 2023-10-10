using System;
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
        Debug.Log("Loading Dings");
        SaveData saveData = JsonUtility.FromJson<SaveData>(ReadFile());

        FindObjectOfType<Stats>().CoinsCollected = saveData.coins;

        CheckpointManager checkpointManager = FindObjectOfType<CheckpointManager>();
        Checkpoint checkpoint = FindCheckpoint(saveData.lastCheckpointPos);
        if (checkpoint == null)
            Debug.LogError("Checkpoint is null");
        else
        {
            checkpoint.SetActivated(true);
            checkpointManager.Checkpoint = checkpoint;
            checkpointManager.RespawnPlayer();
        }        
    }

    private Checkpoint FindCheckpoint(Vector3 checkpointPosition)
    {
        Checkpoint[] checkpoints = GameObject.FindObjectsOfType<Checkpoint>();
        foreach(Checkpoint checkpoint in checkpoints) {
            if (checkpoint.GetRespawnPoint().Equals(checkpointPosition))
                return checkpoint;
        }

        return null;
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
