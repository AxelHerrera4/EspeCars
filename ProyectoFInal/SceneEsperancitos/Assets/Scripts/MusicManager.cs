using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Audio")]
    public AudioSource musicSource;

    [Header("Settings")]
    [Range(0f, 1f)] public float defaultVolume = 0.8f;
    private const string VOLUME_KEY = "MUSIC_VOLUME";
    private float lastVolume = 0.8f;
    private const string MUTE_KEY = "MUSIC_MUTE";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource == null) musicSource = GetComponent<AudioSource>();

        if (musicSource == null)
        {
            Debug.LogError("MusicManager: No hay AudioSource en este objeto. Agrega uno.", this);
            enabled = false;
            return;
        }

        float vol = PlayerPrefs.GetFloat(VOLUME_KEY, defaultVolume);
        ApplyVolume(vol);

        bool isMuted = PlayerPrefs.GetInt(MUTE_KEY, 0) == 1;
        musicSource.mute = isMuted;

        if (!musicSource.isPlaying)
            musicSource.Play();
    }


    public void SetVolume(float value01)
    {
        ApplyVolume(value01);
        lastVolume = value01;
        PlayerPrefs.SetFloat(VOLUME_KEY, value01);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        return musicSource != null ? musicSource.volume : defaultVolume;
    }

    private void ApplyVolume(float v)
    {
        if (musicSource != null) musicSource.volume = Mathf.Clamp01(v);
    }

    public void ToggleMute()
    {
        musicSource.mute = !musicSource.mute;
        PlayerPrefs.SetInt(MUTE_KEY, musicSource.mute ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log($"Mute toggled. IsMuted: {musicSource.mute}");
    }

    public bool IsMuted()
    {
        return musicSource.mute;
    }
}