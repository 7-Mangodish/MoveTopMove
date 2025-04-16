using UnityEngine;

[System.Serializable]
public class Sound
{
    public AudioClip audioClip;

    public string name;
    [Range(0, 1)]
    public float volumn;
    [Range(0, 3)]
    public float pitch;

    public bool loop;

    public bool playOnAwake;

    public bool mute;
    [HideInInspector]
    public AudioSource audioSource;
}
