using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance { get { return instance; } }

    [SerializeField] private Sound[] sfxSounds;

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

    public SoundName soundName;
    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        foreach(Sound sound in sfxSounds) {
            sound.audioSource = this.AddComponent<AudioSource>();

            sound.audioSource.clip = sound.audioClip;
            sound.audioSource.name = sound.name;
            sound.audioSource.volume = sound.volumn;
            sound.audioSource.pitch = sound.pitch;
            sound.audioSource.loop = sound.loop;
            sound.audioSource.mute = sound.mute;
            sound.audioSource.playOnAwake = sound.playOnAwake;
        }
    }

    public void PlaySound(SoundName name) {
        Sound res = null;
        foreach(Sound sound in sfxSounds) {
            if(sound.name == name.ToString()) { 
                res = sound; break; 
            }
        }
        if (res == null)
            Debug.Log(name + " not found");
        else
            res.audioSource.Play();
    }

    public void TurnOffSound() {
        foreach(Sound sound in sfxSounds) {
            sound.audioSource.mute = true;
        }
    }

    public void TurnOnSound() {
        foreach(Sound sound in sfxSounds) {
            sound.audioSource.mute = false;
        }
    }
}
