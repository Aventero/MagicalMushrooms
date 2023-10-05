using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject OptionsMenu;
    public VolumeProfile volumeProfile;

    private VolumeProfile standardProfile;
    private Volume volume;

    private bool activeMenu = false;

    private void Start()
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
        volume.profile = volumeProfile;
        OptionsMenu.SetActive(false);
        StateManager.Instance.PauseGameEvent.Invoke();
    }

    public void Resume()
    {
        activeMenu = false;
        volume.profile = standardProfile;
        StateManager.Instance.ResumeGameEvent.Invoke();
        this.gameObject.SetActive(false);
    }

    public void Save()
    {
        Debug.Log("Trying so save");
    }

    public void Options()
    {
        OptionsMenu.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
