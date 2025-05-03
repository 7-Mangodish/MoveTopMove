using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public List<SoundController> listSoundCtrl;

    public List<SoundData> listSoundData;
    public SoundController clickSoundCtrl;
    public bool isChangeTempVolume;
    public void Awake() {
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
    }

    public void PlaySoundClickButton() {
        clickSoundCtrl.PlaySound(SoundData.SoundName.button_click);
    }

    public void UpdateVolumn() {
        for(int i=0; i<listSoundCtrl.Count; i++) {
            if (listSoundCtrl[i] != null) listSoundCtrl[i].UpdateSoundVolumn();
            else listSoundCtrl.RemoveAt(i);
        }
    }


    public SoundData GetSoundData(SoundData.SoundName name) {
        foreach(SoundData soundData in listSoundData) {
            if (soundData.soundName == name)
                return soundData;
        }
        Debug.Log("Khong ton tai sound: " + name);
        return listSoundData[0];
    }

    public void AddSoundController(SoundController soundCtrl) {
        listSoundCtrl.Add(soundCtrl);
    }
}
