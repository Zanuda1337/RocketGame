using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    public Sound[] Sounds;

    public static AudioManager instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        foreach (Sound sound in Sounds)
        {
            sound.Source = gameObject.AddComponent<AudioSource>();

            sound.Source.clip = sound.clip;
            sound.Source.volume = sound.Volume;
            sound.Source.pitch = sound.Pitch;
            sound.Source.loop = sound.Loop;
            sound.Source.outputAudioMixerGroup = sound.Group;
        }

        //if (s.mixerGroup == null) { s.source.outputAudioMixerGroup = mixerGroup; } else { s.source.outputAudioMixerGroup = s.mixerGroup; }
    }
    public void Play(string name)
    {
        Sound s = Array.Find(Sounds, sound => sound.Name == name);
        if (s==null)
        {
            Debug.Log($"Can't find sound with name \"{name}\"");
            return;
        }
        if (!s.Source.isPlaying) s.Source.Play();
        s.Source.volume = s.Volume;
        s.Source.pitch = s.Pitch;
    }

    public void Stop(string name, float fadeTime = 0f)
    {
        StartCoroutine(FadeOut(name, fadeTime));
    }

    public void SetPitch(string name, float pitch)
    {
        Sound s = Array.Find(Sounds, item => item.Name == name);
        if (s == null)
        {
            Debug.Log($"Can't find sound with name \"{name}\"");
            return;
        }

        s.Source.pitch = pitch;
        s.Pitch = s.Source.pitch;
    }

    public void SetVolume(string name, float volume)
    {
        Sound s = Array.Find(Sounds, item => item.Name == name);
        if (s == null)
        {
            Debug.Log($"Can't find sound with name \"{name}\"");
            return;
        }

        s.Source.volume = volume;
        s.Volume = s.Source.volume;
    }
    private IEnumerator FadeOut(string name, float fadeTime)
    {
        Sound s = Array.Find(Sounds, item => item.Name == name);
        if (s == null)
        {
            Debug.Log($"Can't find sound with name \"{name}\"");
            yield return null;
        }
        while (s.Source.volume > 0.0f)
        {
            s.Source.volume -= Time.unscaledDeltaTime / fadeTime;
            yield return null;
        }
        if (s.Source.isPlaying) s.Source.Stop();
        s.Source.volume = 1f;
    }
    public IEnumerator SmoothPitchDown(string name, float pitch, float time = 0f)
    {
        Sound s = Array.Find(Sounds, item => item.Name == name);
        if (s == null)
        {
            Debug.Log($"Can't find sound with name \"{name}\"");
            yield return null;
        }
        float currentPitch = s.Source.pitch;

        while (s.Source.pitch > pitch)
        {
                s.Source.pitch -= Time.unscaledDeltaTime / time;
                s.Pitch = s.Source.pitch;
                yield return null;
        }
    }
    public IEnumerator SmoothPitchUp(string name, float pitch, float time = 0f)
    {
        Sound s = Array.Find(Sounds, item => item.Name == name);
        if (s == null)
        {
            Debug.Log($"Can't find sound with name \"{name}\"");
            yield return null;
        }
        float currentPitch = s.Source.pitch;

        while (s.Source.pitch < pitch)
        {
            s.Source.pitch += Time.unscaledDeltaTime / time;
            s.Pitch = s.Source.pitch;
            yield return null;
        }
    }

    public IEnumerator SmoothVolumeUp(string name, float volume = 1f, float time = 0f)
    {
        Sound s = Array.Find(Sounds, item => item.Name == name);
        if (s == null)
        {
            Debug.Log($"Can't find sound with name \"{name}\"");
            yield return null;
        }
        float currentVolume = s.Source.volume;

        while (s.Source.volume < volume)
        {
            s.Source.volume += Time.unscaledDeltaTime / time;
            s.Volume = s.Source.volume;
            yield return null;
        }
    }

    public IEnumerator SmoothVolumeDown(string name, float volume = 0f, float time = 0f)
    {
        Sound s = Array.Find(Sounds, item => item.Name == name);
        if (s == null)
        {
            Debug.Log($"Can't find sound with name \"{name}\"");
            yield return null;
        }
        float currentVolume = s.Source.volume;

        while (s.Source.volume > volume)
        {
            s.Source.volume -= Time.unscaledDeltaTime / time;
            s.Volume = s.Source.volume;
            yield return null;
        }
    }

}
