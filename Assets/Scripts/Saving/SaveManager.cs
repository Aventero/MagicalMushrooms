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
        SaveData data = new();
        data.playerPos = player.transform.position;
        data.playerRotation = player.transform.rotation;
        data.coins = player.GetComponent<Stats>().CoinsCollected;
        data.lastCheckpointPos = FindObjectOfType<CheckpointManager>().Checkpoint.GetRespawnPoint();

        return data;
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
