using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour, UIMenu
{
    public GameObject OptionsMenu;
    public Volume GamePostVolume;
    public VolumeProfile volumeProfile;
    public GameObject Menu;

    private VolumeProfile standardProfile;

    private bool activeMenu = false;

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

        Debug.Log("Starting Pause menu");
        Menu.SetActive(true);
        GamePostVolume.profile = volumeProfile;
        OptionsMenu.SetActive(false);
        StateManager.Instance.PauseGameEvent.Invoke();
    }

    public void Resume()
    {
        activeMenu = false;
        GamePostVolume.profile = standardProfile;
        StateManager.Instance.ResumeGameEvent.Invoke();
        UIManager.Instance.SetOverlayVisibility(true);
        this.gameObject.SetActive(false);
    }

    public void Save()
    {
        GameObject.FindObjectOfType<SaveManager>().SaveGame();
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
        SceneManager.LoadScene(0);
    }
}
