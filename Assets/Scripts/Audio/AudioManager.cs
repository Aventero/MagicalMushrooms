using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup MasterMixer;
    [SerializeField] private AudioMixerGroup MusicMixer;
    [SerializeField] private AudioMixerGroup EffectsMixer;

    public Sound[] sounds;

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        SetupAudioClips();
    }

    private void Start()
    {
        LoadSoundSettings();
        //Play("Music");
    }

    // Start is called before the first frame update
    private void SetupAudioClips()
    {
        foreach (Sound s in sounds)
        {
            s.source = this.gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            if (s.isMusic)
                s.source.outputAudioMixerGroup = MusicMixer;
            else
                s.source.outputAudioMixerGroup = EffectsMixer;

            s.source.volume = s.volume;
            s.source.loop = s.loop;
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s != null)
            s.source.Play();
        else
            Debug.LogError("SoundFile: " + name + " not Found");
    }

    public void StopPlaying(string name)
    {
        string fileName = GetClipName(name);
        AudioSource source = GetAudioSource(fileName);

        source.Stop();
    }

    public bool IsPlaying(string name)
    {
        string fileName = GetClipName(name);
        AudioSource source = GetAudioSource(fileName);

        return source.isPlaying;
    }

    public void SetMasterVolume(float volume)
    {
        SetMixerValue("MasterVolume", volume);
        Debug.Log("Master Volume: " + volume);
    }

    public void SetMusicVolume(float volume)
    {
        SetMixerValue("MusicVolume", volume);
        Debug.Log("Music Volume: " + volume);
    }

    public void SetEffectsVolume(float volume)
    {
        SetMixerValue("EffectVolume", volume);
        Debug.Log("Effects Volume: " + volume);
    }

    private void SetMixerValue(string mixer, float volume)
    {
        // Reason for Log10 * 20 -> https://johnleonardfrench.com/the-right-way-to-make-a-volume-slider-in-unity-using-logarithmic-conversion/
        MasterMixer.audioMixer.SetFloat(mixer, Mathf.Log10(volume) * 20);
    }

    private string GetClipName(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        return s.clip.name;
    }

    private AudioSource GetAudioSource(string fileName)
    {
        AudioSource[] sources = this.gameObject.GetComponentsInChildren<AudioSource>();
        return Array.Find(sources, s => s.clip.name == fileName);
    }

    private void LoadSoundSettings()
    {
        Debug.Log("Loading Sound Settings!");
        // Loading Sound volumes
        float volume;

        volume = GetSavedVolume("MasterVolume");
        SetMasterVolume(volume);

        volume = GetSavedVolume("MusicVolume");
        SetMusicVolume(volume);

        volume = GetSavedVolume("EffectsVolume");
        SetEffectsVolume(volume);
    }

    private float GetSavedVolume(string mixerName)
    {
        // Return saved voluem float
        if (PlayerPrefs.HasKey(mixerName))
            return PlayerPrefs.GetFloat(mixerName);

        // Set new volume float if there is none
        PlayerPrefs.SetFloat(mixerName, 1.0f);
        return 1.0f;
    }
}