using System;
using System.Collections.Generic;
using UnityEngine;
using static System.Guid;

public class PlayerData
{
    public string PLAYER_ID;

    public string playerName;
    public string deviceModel;
    public int zone;
    public float musicVolume = 1;
    public float soundVolume = 1;
    public bool isVibrantion = true;
    public int zombieDayVictory;

    public SkinData skinData;
    public SkillData skillData;

    public int currentWeaponId;
    public bool isWeaponBoom;
    public List<WeaponData> listWeaponData = new List<WeaponData>();

    public PlayerData(int hatCount, int pantCount, int armorCount, int setCount) {
        this.PLAYER_ID = Guid.NewGuid().ToString();
        playerName = "You";
        this.deviceModel = SystemInfo.deviceModel;
        zone = 1;
        skinData = new SkinData(hatCount, pantCount, armorCount, setCount);
        skillData = new SkillData();
    }


}
