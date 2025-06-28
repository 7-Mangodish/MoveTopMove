using System;
using System.Collections.Generic;
using UnityEngine;
using static System.Guid;

public class PlayerData
{
    public bool isNew = true;
    public string PLAYER_ID;
    public string playerName;
    public string deviceModel;
    public int zone;
    public float musicVolume = 1;
    public float soundVolume = 1;
    public bool isVibrantion = true;
    //Weapon
    public int currentWeaponId;
    public List<WeaponData> listWeaponData = new List<WeaponData>();
    //Skin
    public int currentHatId;
    public int currentPantId;
    public int currentArmorId;
    public int currentSetId;
    public List<SkinData> listHatSkinData;
    public List<SkinData> listPantSkinData;
    public List<SkinData> listArmorSkinData;
    public List<SkinData> listSetSkinData;
    public bool isSet;
    public int coin;
    //ZombieMode
    public SkillData skillData;
    public int zombieDayVictory;
    public PlayerData() {
        this.PLAYER_ID = Guid.NewGuid().ToString();
        playerName = "You";
        this.deviceModel = SystemInfo.deviceModel;
        zone = 1;
        skillData = new SkillData();
        currentHatId = currentPantId = currentArmorId = currentSetId = 0;
        isSet = false;
        zombieDayVictory = 0;
    }
}

public class HatData {
    public int hatId;
    public bool isLock = true;
    public HatData(int id) {
        hatId = id;
    }
}

[Serializable]
public class SkinData {
    public int adsWatch;
    public E_SkinType skinType;

    public SkinData(int adsWatch, E_SkinType skinType) {
        this.adsWatch = adsWatch;
        this.skinType = skinType;
    }
}

public enum E_SkinType {
    Use,
    Buy,
    NotBuy

}
