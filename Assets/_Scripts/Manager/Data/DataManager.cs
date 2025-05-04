using System;
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

    public void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnApplicationQuit() {
        Debug.Log("AppQuit");
        SaveData(); 
    }
    //private void OnApplicationPause(bool pause) { SaveData(); }

    public void LoadData() {
        if (PlayerPrefs.HasKey(GameVariable.PLAYER_PERSONAL_DATA)) {
            string json = PlayerPrefs.GetString(GameVariable.PLAYER_PERSONAL_DATA);
            playerData = JsonUtility.FromJson<PlayerData>(json);
            isInit = true;
            //
            UpdateWeapon();
            UpdateHat();
            return;
        }
        //
        playerData = new PlayerData(
            hatObjects.listHats.Count,
            pantObjects.listPants.Length,
            armorObjects.listArmor.Length,
            setObjects.listSets.Length
            );
        // Duyet qua tung vu khi trong WeaponObject
        foreach (Weapon weapon in weaponObjects.listWeapon) {
            WeaponData newWeaponData = new WeaponData(weapon.id);
            if (weapon.isBoom) newWeaponData.isBoom = true;
            playerData.listWeaponData.Add(newWeaponData);
        }
        playerData.listWeaponData[0].isLock = false;
        // Duyet Tung Hat trong HatObject
        foreach(Hat hat in hatObjects.listHats) {
            HatData newHatData = new HatData(hat.id);
            playerData.listHatData.Add(newHatData);
        }
        //
        PlayerPrefs.SetString(GameVariable.PLAYER_PERSONAL_DATA, JsonUtility.ToJson(playerData));
        isInit = true;
    }

    private void SaveData() {
        if (!isInit)
            return;
        PlayerPrefs.SetString(GameVariable.PLAYER_PERSONAL_DATA,JsonUtility.ToJson(playerData));
        PlayerPrefs.Save();
        //
        //FirebaseManager.Instance.SavePlayerData(playerPersonalData.PLAYER_ID, playerPersonalData);
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

    #region ----------------SKIN_DATA--------------
    public void UpdateHat() {
        for(int i= playerData.listHatData.Count-1; i>=0; i--) {
            bool isDelete = true;
            foreach(Hat hat in hatObjects.listHats) {
                if(hat.id == playerData.listHatData[i].hatId) { isDelete = false; break; }
            }
            if (isDelete){ playerData.listHatData.RemoveAt(i); }
        }
        foreach(Hat hat in hatObjects.listHats) {
            bool isNew = true;
            foreach(HatData hatData in playerData.listHatData) {
                if (hatData.hatId == hat.id) {
                    isNew = false; break;
                }
            }
            if (isNew) {
                HatData newHatData = new HatData(hat.id);
                playerData.listHatData.Add(newHatData);
            }
        }
    }
    public SkinData GetSkinData() {
        if (!PlayerPrefs.HasKey(GameVariable.PLAYER_SKIN)) {
            SkinData skinData = new SkinData(
                hatObjects.listHats.Count,
                pantObjects.listPants.Length,
                armorObjects.listArmor.Length,
                setObjects.listSets.Length
            );
            skinData.isSet = false;
            skinData.hatIndex = -1;
            skinData.pantIndex = -1;
            skinData.armorIndex = -1;
            skinData.setIndex = -1;
            SaveSkinData(skinData);
            return skinData;
        }

        string json = PlayerPrefs.GetString(GameVariable.PLAYER_SKIN);
        SkinData data = JsonUtility.FromJson<SkinData>(json);
        return data;
    }
    public void SaveSkinData(SkinData data) {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(GameVariable.PLAYER_SKIN, json);
        //Debug.Log("Save Skin: " + data.hatIndex + " " + data.pantIndex + " " + data.armorIndex + " " + data.setIndex);
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
    public int GetZombieDayVictory() {
        if (!PlayerPrefs.HasKey(GameVariable.ZOMBIE_DAY_VICTORY))
            PlayerPrefs.SetInt(GameVariable.ZOMBIE_DAY_VICTORY, 0);

        return PlayerPrefs.GetInt(GameVariable.ZOMBIE_DAY_VICTORY);
    }

    public void SetZombieDayVictory(int currentDay) {
        PlayerPrefs.SetInt("ZombieDayVictory", currentDay);
    }
    #endregion
}
