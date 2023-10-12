using System;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour, UIMenu
{
    private readonly string loadSave = "LoadSave";

    public GameObject ContinueButton;
    public GameObject OptionsMenu;
    public GameObject MainMenuObject;

    public void Awake()
    {
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.None;

        MainMenuObject.SetActive(true);
        OptionsMenu.SetActive(false);

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

    public void Options()
    {
        MainMenuObject.SetActive(false);
        OptionsMenu.SetActive(true);
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

    public void TurnOnMenu()
    {
        MainMenuObject.SetActive(true);
    }
}
