using UnityEngine;

[CreateAssetMenu(fileName = "SoundData", menuName = "Scriptable Objects/SoundData")]
public class SoundData:  ScriptableObject
{
    public AudioClip audioClip;

    public SoundName soundName;
    [Range(0f, 1f)]
    public float volume;

    public bool loop;

    public bool mute;

    public enum SoundName {
        button_click,
        size_up,
        weapon_hit,
        weapon_throw,
        count_down,
        dead_1,
        dead_2,
        end_win,
        end_lose
    }
}
