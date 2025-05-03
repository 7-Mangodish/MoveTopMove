using UnityEngine;


public class SoundController : MonoBehaviour
{
    private AudioSource audioSource;
    private SoundData soundData;
    public bool isMusic;
    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }


    public void PlaySound(SoundData.SoundName soundName) {
        if (AudioManager.Instance == null) return;
        //if (AudioManager.Instance == null || !AudioManager.Ins.CanPlaySound(soundName)) return;
        soundData = AudioManager.Instance.GetSoundData(soundName);
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (soundData != null && soundData.audioClip != null) {
            if (DataManager.Instance != null)
                audioSource.volume = soundData.volume * (isMusic ? DataManager.Instance.playerData.musicVolume : DataManager.Instance.playerData.soundVolume);
            else
                audioSource.volume = soundData.volume;
            audioSource.clip = soundData.audioClip;
            audioSource.loop = soundData.loop;
            audioSource.Play();
        }
        AudioManager.Instance.AddSoundController(this);
    }

    public void UpdateSoundVolumn() {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        audioSource.volume = soundData.volume * (isMusic ? DataManager.Instance.playerData.musicVolume : DataManager.Instance.playerData.soundVolume);
        if (isMusic && AudioManager.Instance.isChangeTempVolume) {
            audioSource.volume = audioSource.volume / 2;
        }
        
    }

}
