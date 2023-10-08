using System;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private readonly string loadSave = "LoadSave";

    public GameObject ContinueButton;
    public void Awake()
    {
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.None;

        ContinueButton.GetComponent<Button>().interactable = CheckIfSavesExist();
    }

    private bool CheckIfSavesExist()
    {
        return File.Exists(Application.persistentDataPath + "/" + "Savedata.json");
    }

    public void Continue()
    {
        SetLoadSavePref(true);
        SetCursor();
        SceneManager.LoadScene(1);
    }

    public void NewGame()
    {

        SetLoadSavePref(false);
        SetCursor();
        SceneManager.LoadScene(1);
    }

    private void SetLoadSavePref(bool value)
    {
        PlayerPrefs.SetInt(loadSave, Convert.ToInt32(value));
        PlayerPrefs.Save();
    }

    private void SetCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Quit()
    {
        Debug.Log("Quitting the game! Bye!");
        Application.Quit();
    }
}
