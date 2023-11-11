using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [Header("Menues")]
    [SerializeField] private GameObject DialogMenu;
    [SerializeField] private GameObject Menu;

    [Header("Slider")]
    [SerializeField] private Slider MasterSlider;
    [SerializeField] private Slider MusicSlider;
    [SerializeField] private Slider EffectsSlider;

    [Header("Settings")]
    public TMP_Text ResolutionText;
    public Toggle FullscreenToggle;

    private Resolution[] resolutions;
    private int currentResolutionPos;
    private bool settingsChanged;
    private IUIMenu uiMenu;

    private bool fullscreen;

    private void Awake()
    {
        Menu.SetActive(true);
        DialogMenu.SetActive(false);

        settingsChanged = false;
        uiMenu = this.GetComponentInParent<IUIMenu>();

        SetupSettings();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            BackButton();
    }

    public void PreviousResolution()
    {
        settingsChanged = true;

        if (currentResolutionPos > 0)
            currentResolutionPos--;

        UpdateResolutionText();
    }

    public void NextResolution()
    {
        settingsChanged = true;

        if (currentResolutionPos < resolutions.Length - 1)
            currentResolutionPos++;

        UpdateResolutionText();
    }

    private void UpdateResolutionText()
    {
        Debug.Log("Current ResolutionPos: " + currentResolutionPos);
        Resolution resolution = resolutions[currentResolutionPos];
        ResolutionText.text = resolution.width + " x " + resolution.height;
    }

    public void ToggleFullscreen(bool value)
    {
        settingsChanged = true;
        fullscreen = value;
    }

    public void ChangeMasterVolume(float value)
    {
        Debug.Log("Changed Master Volume Slider!");

        settingsChanged = true;
        AudioManager.Instance.SetMasterVolume(value);
    }

    public void ChangeMusicVolume(float value)
    {
        Debug.Log("Changed Music Volume Slider!");

        settingsChanged = true;
        AudioManager.Instance.SetMusicVolume(value);
    }

    public void ChangeEffectsVolume(float value)
    {
        Debug.Log("Changed Effects Volume Slider!");
        settingsChanged = true;
        AudioManager.Instance.SetEffectsVolume(value);
    }

    public void SaveButton()
    {
        ApplySettings();
        SaveSettings();
    }

    public void BackButton()
    {
        if (settingsChanged)
        {
            DialogMenu.SetActive(true);
            Menu.SetActive(false);
        }
        else
        {
            uiMenu.TurnOnMenu();
            Menu.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    private void ApplySettings()
    {
        settingsChanged = false;
        Resolution resolution = resolutions[currentResolutionPos];
        Screen.SetResolution(resolution.width, resolution.height, fullscreen);
    }

    private void SetupResolution()
    {
        currentResolutionPos = -1; 
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionPos = i;
                break; 
            }
        }

        if (currentResolutionPos == -1)
        {
            currentResolutionPos = 0; 
        }
    }


    private void SaveSettings()
    {
        Debug.Log("Saving Settings");

        // Saving Sound volumes
        PlayerPrefs.SetFloat("MasterVolume", MasterSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", MusicSlider.value);
        PlayerPrefs.SetFloat("EffectsVolume", EffectsSlider.value);

        PlayerPrefs.Save();
    }

    private void SetupSettings()
    {
        Debug.Log("Loading Settings");

        float volume;

        // Setup slider positions
        volume = PlayerPrefs.GetFloat("MasterVolume");
        MasterSlider.value = volume;

        volume = PlayerPrefs.GetFloat("MusicVolume");
        MusicSlider.value = volume;

        volume = PlayerPrefs.GetFloat("EffectsVolume");
        EffectsSlider.value = volume;

        resolutions = Screen.resolutions;
        SetupResolution();
        UpdateResolutionText();

        fullscreen = Screen.fullScreen;
        FullscreenToggle.isOn = fullscreen;

        PlayerPrefs.Save();

        settingsChanged = false;
    }

    public void DialogBack()
    {
        settingsChanged = false;

        DialogMenu.SetActive(false);
        BackButton();
    }

    public void DialogSave()
    {
        Debug.Log("Dialog Save Click");
        SaveButton();
        BackButton();
    }

    private void OnEnable()
    {
        Debug.Log("Starting Settings Menu");

        SetupSettings();
    }

    private void OnDisable()
    {
        Debug.Log("Deactivating Settings Menu");
    }
}
