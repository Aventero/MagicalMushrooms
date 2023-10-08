using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private readonly string filename = "Savedata.json";

    private GameObject player;
    private GameObject witch;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        witch = GameObject.FindWithTag("Witch");
    }

    public void SaveGame()
    {
        Debug.Log("Saving Game!");
        string json = JsonUtility.ToJson(SetupSaveData());
        WriteToFile(json);
    }

    private SaveData SetupSaveData()
    {
        SaveData data = new();
        data.playerPos = player.transform.position;
        data.coins = player.GetComponent<Stats>().CoinsCollected;
        data.lastCheckpoint = GameObject.FindObjectOfType<CheckpointManager>().Checkpoint.transform.position;

        data.witchPos = witch.transform.position;

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
