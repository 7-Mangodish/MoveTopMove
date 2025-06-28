using System;
using System.Collections.Generic;
using System.Globalization;

using UnityEngine;
using static System.Guid;
public class DataManager : MonoBehaviour
{
    private static DataManager instance;
    public static DataManager Instance { get => instance; }

    public static string PLAYER_ID;
    public bool isInit = false;

    [Header("-----PlayerPersonalData------")]
    public PlayerData playerData;
    [Header("-----WEAPON-----")]
    public WeaponObjects weaponObjects;
    public Material[] listMaterial;
    [Header("-----SKIN-----")]
    public HatObjects hatObjects;
    public PantObjects pantObjects;
    public ArmorObjects armorObjects;
    public SetObjects setObjects;
    public ConfigPlayerSkinShop configPlayerSkinShop;
    [Header("-----Coin-----")]
    public int cheatPlayerCoin;
    public int winPlayerCoin;
    public int losePlayerCoin;
    public bool isDoubleAward = false;

    private void OnApplicationQuit() {
        SaveData();
    }
    private void OnDisable() {
        SaveData();
    }

    public void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject);
    }

    //Ham Load Data moi khi vao game
    public void LoadData() {
        if (PlayerPrefs.HasKey(GameVariable.PLAYER_PERSONAL_DATA)) {
            string json = PlayerPrefs.GetString(GameVariable.PLAYER_PERSONAL_DATA);
            playerData = JsonUtility.FromJson<PlayerData>(json);
            isInit = true;
            UpdateWeapon();
            UpdateDataSkinShop();
            SaveData();
            return;
        }
        //Truong hop khong co data
        InitData();
        SaveData();
    }

    // Ham duoc goi de them Data khi vao game lan dau
    public void InitData() {
        playerData = new PlayerData();
        // Duyet qua tung vu khi trong WeaponObject
        foreach (Weapon weapon in weaponObjects.listWeapon) {
            WeaponData newWeaponData = new WeaponData(weapon.id);
            if (weapon.isBoom) newWeaponData.isBoom = true;
            playerData.listWeaponData.Add(newWeaponData);
        }
        playerData.listWeaponData[0].isLock = false;
        //Hat
        playerData.listHatSkinData = new List<SkinData>();
        for(int i=0; i< configPlayerSkinShop.listHatSkinShop.Length; i++) {
            SkinData newHatData = new(0, E_SkinType.NotBuy);
            playerData.listHatSkinData.Add(newHatData);
        }
        //Pant
        playerData.listPantSkinData = new List<SkinData>();
        for (int i = 0; i < configPlayerSkinShop.listPantSkinShop.Length; i++) {
            SkinData newPantData = new(0, E_SkinType.NotBuy);
            playerData.listPantSkinData.Add(newPantData);
        }
        // Armor
        playerData.listArmorSkinData = new List<SkinData>();
        for (int i = 0; i < configPlayerSkinShop.listArmorSkinShop.Length; i++) {
            SkinData newArmorData = new(0, E_SkinType.NotBuy);
            playerData.listArmorSkinData.Add(newArmorData);
        }
        //Set
        playerData.listSetSkinData = new List<SkinData>();
        for (int i = 0; i < configPlayerSkinShop.listSetSkinShop.Length; i++) {
            SkinData newSetData = new(0, E_SkinType.NotBuy);
            playerData.listSetSkinData.Add(newSetData);
        }
        isInit = true;
    }

    //Ham luu data
    private void SaveData() {
        if (!isInit)
            return;
        PlayerPrefs.SetString(GameVariable.PLAYER_PERSONAL_DATA,JsonUtility.ToJson(playerData));
        PlayerPrefs.Save();
        //
        //FirebaseManager.Instance.SavePlayerData(playerPersonalData.PLAYER_ID, playerPersonalData);
    }

    // Ham duoc goi de kiem tra xem lieu co them moi hay xoa skin trong shop hay khong
    private void UpdateDataSkinShop() {
        foreach (S_SkinShopInfor skinInfor in configPlayerSkinShop.listHatSkinShop) {
            if (skinInfor.id >= playerData.listHatSkinData.Count) {
                SkinData newData = new SkinData(0, E_SkinType.NotBuy);
                playerData.listHatSkinData.Add(newData);
            }
        }
        foreach (S_SkinShopInfor skinInfor in configPlayerSkinShop.listPantSkinShop) {
            if (skinInfor.id >= playerData.listPantSkinData.Count) {
                SkinData newData = new SkinData(0, E_SkinType.NotBuy);
                playerData.listPantSkinData.Add(newData);
            }
        }
        foreach (S_SkinShopInfor skinInfor in configPlayerSkinShop.listArmorSkinShop) {
            if (skinInfor.id >= playerData.listArmorSkinData.Count) {
                SkinData newData = new SkinData( 0, E_SkinType.NotBuy);
                playerData.listArmorSkinData.Add(newData);
            }
        }
        foreach (S_SkinShopInfor skinInfor in configPlayerSkinShop.listSetSkinShop) {
            if (skinInfor.id >= playerData.listSetSkinData.Count) {
                SkinData newData = new SkinData(0, E_SkinType.NotBuy);
                playerData.listSetSkinData.Add(newData);
            }
        }
    }

    public void UpdatePlayerCoin(int coin) {
        playerData.coin += coin;
        SaveData();
    }

    #region ----------------WEAPON_DATA------------

    public void UpdateWeapon() {
        // Xoa weaponData neu xoa weapon tren weaponObjects
        for (int i = playerData.listWeaponData.Count - 1; i >= 0; i--) {
            bool isDelete = true;
            foreach (Weapon weapon in weaponObjects.listWeapon) {
                if (playerData.listWeaponData[i].weaponId == weapon.id) { isDelete = false; continue; }
            }
            if (isDelete) playerData.listWeaponData.RemoveAt(i);
        }
        //Them moi weaponData neu them moi vao ScriptableObject
        foreach (Weapon weapon in weaponObjects.listWeapon) {
            bool isNew = true;
            foreach (WeaponData weaponData in playerData.listWeaponData) {
                if (weaponData.weaponId == weapon.id) { isNew = false; continue; }
            }
            if (isNew) {
                WeaponData newWeaponData = new WeaponData(weapon.id);
                if (weapon.isBoom) newWeaponData.isBoom = true;
                playerData.listWeaponData.Add(newWeaponData);
            }
        }
    }
    public WeaponData GetWeaponData(int id) {
        foreach (WeaponData data in playerData.listWeaponData) {
            if (data.weaponId == id)
                return data;
        }
        // Truong hop vu khi su dung da bi xoa data tren SC
        Debug.LogWarning("weaponData da bi xoa, tra ve vu khi dau tien");
        WeaponData weaponData = playerData.listWeaponData[0];
        playerData.currentWeaponId = weaponData.weaponId;
        if (weaponData.isLock) weaponData.isLock = false;
        return weaponData;
    }
    public Material[] GetWeaponMaterial(int weaponId) {
        WeaponData weaponData = GetWeaponData(weaponId);
        Material[] materialTemp = new Material[3];
        if (weaponData.skinIndex == 0) {
            for (int i = 0; i < 3; i++) {
                materialTemp[i] = listMaterial[weaponData.weaponMaterials[i]];
            }
        }
        else
            materialTemp = weaponObjects.GetListMaterials(weaponId, weaponData.skinIndex);
        return materialTemp;
    }
    #endregion

    #region --------------SKILL_DATA---------------
    public SkillData GetSkillData() {
        if (!PlayerPrefs.HasKey(GameVariable.PLAYER_SKILL)) {
            SkillData skillData = new SkillData();
            string json = JsonUtility.ToJson(skillData);
            PlayerPrefs.SetString(GameVariable.PLAYER_SKILL, json);
        }
        string jsonData = PlayerPrefs.GetString(GameVariable.PLAYER_SKILL);
        SkillData data = JsonUtility.FromJson<SkillData>(jsonData);
        return data;
    }

    public void SetSkillData(SkillData data) {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(GameVariable.PLAYER_SKILL, json);
    }
    #endregion

    #region ------------ZOMBIE_MODE_DATA-----------
    //public int GetZombieDayVictory() {
    //    if (!PlayerPrefs.HasKey(GameVariable.ZOMBIE_DAY_VICTORY))
    //        PlayerPrefs.SetInt(GameVariable.ZOMBIE_DAY_VICTORY, 0);

    //    return PlayerPrefs.GetInt(GameVariable.ZOMBIE_DAY_VICTORY);
    //}

    //public void SetZombieDayVictory(int currentDay) {
    //    PlayerPrefs.SetInt("ZombieDayVictory", currentDay);
    //}
    #endregion
}
