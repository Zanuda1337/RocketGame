using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string Name;

    public AudioClip clip;

    public AudioMixerGroup Group;

    [Range(0f, 1f)]
    public float Volume;
    [Range(0f, 3f)]
    public float Pitch;
    public bool Loop;

    [HideInInspector]
    public AudioSource Source;
}
