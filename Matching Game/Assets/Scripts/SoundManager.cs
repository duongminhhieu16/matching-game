using System.Collections.Generic;
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
    [SerializeField] private List<AudioClip> clips;
    public static SoundManager Instance;
    private AudioSource Source;
    
    [SerializeField] private AudioMixer _MasterMixer;

    private void Awake()
    {
        Instance = this;
        Source = GetComponent<AudioSource>();
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