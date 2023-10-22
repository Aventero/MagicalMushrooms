using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour, IUIMenu
{
    private readonly string loadSave = "LoadSave";

    public GameObject ContinueButton;
    public GameObject OptionsMenu;
    public GameObject MainMenuObject;

    public void Start()
    {
        Time.timeScale = 1.0f;
        StateManager.Instance.UnlockMouse();

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
        StateManager.Instance.LockMouse();

        if (PlayerPrefs.HasKey("LastSavedScene"))
            SceneLoader.Instance.LoadScene(PlayerPrefs.GetString("LastSavedScene"));
    }

    public void NewGame()
    {
        SetLoadSavePref(false);
        StateManager.Instance.LockMouse();
        SceneLoader.Instance.LoadScene("CutsceneOutside");
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