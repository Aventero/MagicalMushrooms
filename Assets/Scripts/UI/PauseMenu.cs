using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject OptionsMenu;
    public VolumeProfile volumeProfile;
    public GameObject Menu;

    private VolumeProfile standardProfile;
    private Volume volume;

    private bool activeMenu = false;

    private void Awake()
    {
        volume = GameObject.FindObjectOfType<Volume>();
        standardProfile = volume.profile;
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
        volume.profile = volumeProfile;
        OptionsMenu.SetActive(false);
        StateManager.Instance.PauseGameEvent.Invoke();
    }

    public void Resume()
    {
        activeMenu = false;
        volume.profile = standardProfile;
        StateManager.Instance.ResumeGameEvent.Invoke();
        UIManager.Instance.SetOverlayVisibility(true);
        this.gameObject.SetActive(false);
    }

    public void Save()
    {
        Debug.Log("Trying so save");
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
