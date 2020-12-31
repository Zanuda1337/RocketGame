using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup _mixer;
    [SerializeField] private Toggle _toggleMusic;
    [SerializeField] private Toggle _toggleSounds;
    private float _musicVolume;

    public void ToggleMusic()
    {
        if (_toggleMusic.isOn)
        {
            _mixer.audioMixer.SetFloat("MusicVolume", 0);
            PlayerPrefs.SetInt("ToggleMusic", 1);
        }
        else 
        {
            _mixer.audioMixer.SetFloat("MusicVolume", -80);
            PlayerPrefs.SetInt("ToggleMusic", 0);
        }
    }


    public void ToggleSounds()
    {
        if (_toggleSounds.isOn)
        {
            _mixer.audioMixer.SetFloat("SoundsVolume", 0);
            PlayerPrefs.SetInt("ToggleSounds", 1);
        }
        else
        {
            //yield return new WaitForSeconds(1);
            _mixer.audioMixer.SetFloat("SoundsVolume", -80);
            PlayerPrefs.SetInt("ToggleSounds", 0);
        }
    }
    public void Start()
    {
        LoadPrefs(_toggleMusic, "ToggleMusic");
        LoadPrefs(_toggleSounds, "ToggleSounds");
        ToggleMusic();
        ToggleSounds();
    }
    public void LoadPrefs(Toggle toggle, string argument)
    {
        if (PlayerPrefs.HasKey(argument))
        {
            if (PlayerPrefs.GetInt(argument) == 1)
            {
                toggle.isOn = true;
            }
            else
            {
                toggle.isOn = false;
            }
        }
    }
}
