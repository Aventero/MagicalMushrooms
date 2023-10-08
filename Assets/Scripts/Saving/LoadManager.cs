using System;
using System.IO;
using UnityEngine;

public class LoadManager : MonoBehaviour
{
    private readonly string saveFile = "Savedata.json";
    private readonly string loadSave = "LoadSave";

    private GameObject player;
    private GameObject witch;

    public void Start()
    {
        if (Convert.ToBoolean(PlayerPrefs.GetInt(loadSave)))
            LoadSave();
    }

    public void LoadSave()
    {
        Debug.Log("Loading Dings");
        SaveData saveData = JsonUtility.FromJson<SaveData>(ReadFile());

        player = GameObject.FindWithTag("Player");
        CharacterController charController = player.GetComponent<CharacterController>();
        charController.enabled = false;
        player.transform.SetPositionAndRotation(saveData.playerPos, saveData.playerRotation);
        charController.enabled = true;

        witch = GameObject.FindWithTag("Witch");
        witch.transform.position = saveData.witchPos;

        FindObjectOfType<Stats>().CoinsCollected = saveData.coins;
        FindObjectOfType<CheckpointManager>().Checkpoint = saveData.lastCheckpoint;
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
