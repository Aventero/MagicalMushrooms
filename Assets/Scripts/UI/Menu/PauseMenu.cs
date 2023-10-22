using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour, IUIMenu
{
    [Header("Volume")]
    public Volume GamePostVolume;
    public VolumeProfile volumeProfile;

    [Header("Gameobjects")]
    public GameObject Menu;
    public GameObject OptionsMenu;
    public GameObject SkipCutsceneButton;
    public GameObject SaveButton;

    private bool activeMenu = false;
    private VolumeProfile standardProfile;

    private void Awake()
    {
        standardProfile = GamePostVolume.profile;
    }

    public void ShowMenu()
    {
        if (activeMenu)
        {
            Resume();
            return;
        }
        else
        {
            activeMenu = true;
        }

        Menu.SetActive(true);
        GamePostVolume.profile = volumeProfile;
        OptionsMenu.SetActive(false);
        StateManager.Instance.PauseGameEvent.Invoke();
        Debug.Log("Pause?");

        if (!UIManager.Instance.IsCutscene)
            SkipCutsceneButton.SetActive(false);
        else
            SaveButton.SetActive(false);
    }

    public void Resume()
    {
        activeMenu = false;
        GamePostVolume.profile = standardProfile;
        StateManager.Instance.ResumeGameEvent.Invoke();
        UIManager.Instance.SetOverlayVisibility(true);
        gameObject.SetActive(false);
    }

    public void Save()
    {
        FindObjectOfType<SaveManager>().SaveGame();
    }

    public void Options()
    {
        Menu.SetActive(false);
        OptionsMenu.SetActive(true);
    }

    public void TurnOnMenu()
    {
        Menu.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        SceneLoader.Instance.LoadScene("MainMenu");
    }

    public void SkipCutscene()
    {
        SceneLoader.Instance.LoadScene(UIManager.Instance.SkippingCutsceneName);
    }
}
