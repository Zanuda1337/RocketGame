using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class SettingsController : MonoBehaviour
{
    [SerializeField] public AudioMixerGroup Mixer;
    [SerializeField] private Toggle _toggleMusic;
    [SerializeField] private Toggle _toggleSounds;
    [SerializeField] private Switcher _quality;
    [SerializeField] private Switcher _resolution;
    [SerializeField] private Switcher _shakeModifier;
    //[SerializeField] private GameObject _postProcessParent;
    [SerializeField] private PostProcessVolume _postProcess;
    private AmbientOcclusion _ambientOcclusion;
    private Bloom _bloom;
    private ColorGrading _colorGrading;
    private DepthOfField _depthOfField;
    private Vector2 _defaultResolution;
    private float _musicVolume;

    public void ToggleMusic()
    {
        if (_toggleMusic.isOn)
        {
            Mixer.audioMixer.SetFloat("MusicVolume", 0);
            PlayerPrefs.SetInt("ToggleMusic", 1);
        }
        else 
        {
            Mixer.audioMixer.SetFloat("MusicVolume", -80);
            PlayerPrefs.SetInt("ToggleMusic", 0);
        }
    }


    public void ToggleSounds()
    {
        if (_toggleSounds.isOn)
        {
            Mixer.audioMixer.SetFloat("SoundsVolume", 0);
            PlayerPrefs.SetInt("ToggleSounds", 1);
        }
        else
        {
            Mixer.audioMixer.SetFloat("SoundsVolume", -80);
            PlayerPrefs.SetInt("ToggleSounds", 0);
        }
    }
    public void SwitchQuality()
    {
        switch(_quality.Value)
        {
            case 0:
                QualitySettings.SetQualityLevel(0);
                SetPostProcessLevel(0);
                PlayerPrefs.SetInt("Quality", 0);
                break;
            case 1:
                QualitySettings.SetQualityLevel(1);
                SetPostProcessLevel(1);
                PlayerPrefs.SetInt("Quality", 1);
                break;
            case 2:
                QualitySettings.SetQualityLevel(2);
                SetPostProcessLevel(2);
                PlayerPrefs.SetInt("Quality", 2);
                break;
            case 3:
                QualitySettings.SetQualityLevel(3);
                SetPostProcessLevel(3);
                PlayerPrefs.SetInt("Quality", 3);
                break;
        }
    }
    public void SwitchResolution()
    {
        switch (_resolution.Value)
        {
            case 0:
                Screen.SetResolution(Convert.ToInt32(_defaultResolution.x * 0.35), Convert.ToInt32(_defaultResolution.y * 0.35), Screen.fullScreenMode);
                PlayerPrefs.SetInt("Resolution", 0);
                break;
            case 1:
                Screen.SetResolution(Convert.ToInt32(_defaultResolution.x * 0.5), Convert.ToInt32(_defaultResolution.y * 0.5), Screen.fullScreenMode);
                PlayerPrefs.SetInt("Resolution", 1);
                break;
            case 2:
                Screen.SetResolution(Convert.ToInt32(_defaultResolution.x * 0.7), Convert.ToInt32(_defaultResolution.y * 0.7), Screen.fullScreenMode);
                PlayerPrefs.SetInt("Resolution", 2);
                break;
            case 3:
                Screen.SetResolution(Convert.ToInt32(_defaultResolution.x * 0.85), Convert.ToInt32(_defaultResolution.y * 0.85), Screen.fullScreenMode);
                PlayerPrefs.SetInt("Resolution", 3);
                break;
            case 4:
                Screen.SetResolution(Convert.ToInt32(_defaultResolution.x), Convert.ToInt32(_defaultResolution.y), Screen.fullScreenMode);
                PlayerPrefs.SetInt("Resolution", 4);
                break;
        }
    }
    public void SwitchShake()
    {
        switch (_shakeModifier.Value)
        {
            case 0:
                if (CameraShake.instance != null) CameraShake.instance.ShakeModifier = 0;
                PlayerPrefs.SetInt("Shake", 0);
                break;
            case 1:
                if (CameraShake.instance != null) CameraShake.instance.ShakeModifier = 0.25f;
                PlayerPrefs.SetInt("Shake", 1);
                break;
            case 2:
                if (CameraShake.instance != null) CameraShake.instance.ShakeModifier = 0.5f;
                PlayerPrefs.SetInt("Shake", 2);
                break;
            case 3:
                if (CameraShake.instance != null) CameraShake.instance.ShakeModifier = 0.75f;
                PlayerPrefs.SetInt("Shake", 3);
                break;
            case 4:
                if (CameraShake.instance != null) CameraShake.instance.ShakeModifier = 1f;
                PlayerPrefs.SetInt("Shake", 4);
                break;
        }
    }
    private void SetPostProcessLevel(int level)
    {
        switch (level)
        {
            case 0:
                //_postProcess.weight = 0;
                _ambientOcclusion.active = false;
                _bloom.active = false;
                _colorGrading.active = true;
                _depthOfField.active = false;
                break;
            case 1:
                _ambientOcclusion.active = false;
                _bloom.active = true;
                _colorGrading.active = true;
                _depthOfField.active = false;
                break;
            case 2:
                _ambientOcclusion.active = true;
                _bloom.active = true;
                _colorGrading.active = true;
                _depthOfField.active = false;
                break;
            case 3:
                _ambientOcclusion.active = true;
                _bloom.active = true;
                _colorGrading.active = true;
                _depthOfField.active = true;
                break;
        }
    }
    public void Start()
    {
        //_postProcess = _postProcessParent.GetComponent<PostProcessVolume>();
        _postProcess.profile.TryGetSettings(out _ambientOcclusion);
        _postProcess.profile.TryGetSettings(out _bloom);
        _postProcess.profile.TryGetSettings(out _colorGrading);
        _postProcess.profile.TryGetSettings(out _depthOfField);
        _defaultResolution = new Vector2(Screen.resolutions[Screen.resolutions.Length - 1].width, Screen.resolutions[Screen.resolutions.Length - 1].height);
        LoadTogglePrefs(_toggleMusic, "ToggleMusic");
        LoadTogglePrefs(_toggleSounds, "ToggleSounds");
        ToggleMusic();
        ToggleSounds();

        LoadSwitcherPrefs(_quality, "Quality", 2);
        LoadSwitcherPrefs(_resolution, "Resolution", 4);
        LoadSwitcherPrefs(_shakeModifier, "Shake", 4);
        SwitchShake();
        SwitchQuality();
        SwitchResolution();
    }

    public void LoadTogglePrefs(Toggle toggle, string argument)
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
    public void LoadSwitcherPrefs(Switcher switcher, string argument, int defaultValue = 2)
    {
        if (PlayerPrefs.HasKey(argument))
        {
            switcher.Value = PlayerPrefs.GetInt(argument, defaultValue);
            switcher.UpdatePosition();
        }
    }
}
