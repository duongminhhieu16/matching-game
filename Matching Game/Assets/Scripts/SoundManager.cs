﻿using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum SoundType
{
    TypeSelect,
    TypeMove,
    TypePop,
    TypeGameOver
};

public class SoundManager : MonoBehaviour
{
    public List<AudioClip> clips;
    public static SoundManager Instance;
    private AudioSource Source;
    public Slider sliderSound;
    
    [SerializeField] private AudioMixer _MasterMixer;

    private void Awake()
    {
        Instance = this;
        Source = GetComponent<AudioSource>();
        float sliderSoundValue = PlayerPrefs.GetFloat("music");
        sliderSound.value = sliderSoundValue;
    }
    private void Start()
    {
        PlayerPrefs.SetFloat("music", 0);
        sliderSound.value = PlayerPrefs.GetFloat("music", 0);
    }
    public void SetActive()
    {
        PlayerPrefs.SetInt("active", 1);
    }

    public void SetMusicVolume(Slider volume)
    {
        int active = PlayerPrefs.GetInt("active");
        if (active == 1)
        {
            volume.value = PlayerPrefs.GetFloat("music");
            PlayerPrefs.SetInt("active", 0);
        }
        else
        {
            PlayerPrefs.SetFloat("music", volume.value);
            _MasterMixer.SetFloat("music", volume.value);
        }
        DontDestroyOnLoad(_MasterMixer);
    }

    public void PlaySound(SoundType clipType)
    {
        Source.PlayOneShot(clips[(int)clipType]);
    }
}