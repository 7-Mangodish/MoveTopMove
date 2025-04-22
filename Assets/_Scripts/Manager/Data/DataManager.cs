using System;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private static DataManager instance;
    public static DataManager Instance { get => instance; }

    public static string PLAYER_ID;
    public bool isInit = false;


    [Header("-----WEAPON-----")]
    public WeaponObjects weaponObjects;
    public Material[] listMaterial;
    public class OnUserChangeWeaponArg : EventArgs {
        public int skinIndex;
        public Material[] materials;
    }
    public event EventHandler<OnUserChangeWeaponArg> OnUserChangeWeapon;

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
        SaveData();
    }

    /*
     Lay Player ID, Neu khong co, tao moi thong qua Firebase va luu duoi PlayerPref
     */
    public async void InitDataManager() {
        if (!PlayerPrefs.HasKey(GameVariable.PLAYER_ID)) {
            string deviceModel = SystemInfo.deviceModel;

            PlayerPersonalData playerData = new PlayerPersonalData(deviceModel);
            PLAYER_ID = await FirebaseManager.Instance.CreateNewPlayer(playerData);

            PlayerPrefs.SetString(GameVariable.PLAYER_ID, PLAYER_ID);
        }
        else
            PLAYER_ID = PlayerPrefs.GetString(GameVariable.PLAYER_ID);
        isInit = true;
    }
    
    // Luu Data khi Out App
    private void SaveData() {
        PlayerPersonalData data = GetPlayerPersonalData();
        FirebaseManager.Instance.SavePlayerData(PLAYER_ID, data);
    }

    #region -----------------PLAYER_DATA---------------
    public PlayerPersonalData GetPlayerPersonalData() {
        if (!PlayerPrefs.HasKey(GameVariable.PLAYER_PERSONAL_DATA))
        {
            string deviceModel = SystemInfo.deviceModel;
            PlayerPersonalData newData = new PlayerPersonalData(deviceModel);
            SavePlayerPersonalData(newData);
            
        }
        string json = PlayerPrefs.GetString(GameVariable.PLAYER_PERSONAL_DATA);
        PlayerPersonalData data = JsonUtility.FromJson<PlayerPersonalData>(json);
        return data;
    }
    public void SavePlayerPersonalData(PlayerPersonalData data) {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(GameVariable.PLAYER_PERSONAL_DATA, json);
    }
    #endregion

    #region ----------------WEAPON_DATA------------
    public WeaponData GetWeaponData(int weaponIndex) {
        if (!PlayerPrefs.HasKey(weaponIndex.ToString())) {
            WeaponData weaponData = new WeaponData();
            if (weaponIndex == 0)
                weaponData.isLock = false;
            SaveWeaponData(weaponIndex, weaponData);
            return weaponData;
        }

        string json = PlayerPrefs.GetString(weaponIndex.ToString());
        WeaponData data = JsonUtility.FromJson<WeaponData>(json);
        return data;
    }

    public void SaveWeaponData(int weaponIndex, WeaponData data) {
        string json = JsonUtility.ToJson(data);
        //Debug.Log("Save Weapon: " + weaponIndex + ", skin: " + data.skinIndex + ", " + data.showMaterial());
        PlayerPrefs.SetString(weaponIndex.ToString(), json);

        Material[] materialTemp = new Material[3];
        for (int i = 0; i < 3; i++) {
            materialTemp[i] = listMaterial[data.weaponMaterials[i]];
        }
        OnUserChangeWeapon?.Invoke(this, new OnUserChangeWeaponArg {
            skinIndex = data.skinIndex,
            materials = materialTemp,

        });
    }

    public Material[] GetWeaponMaterial(int weaponIndex) {
        WeaponData data = GetWeaponData(weaponIndex);

        Material[] materialTemp = new Material[3];
        if(data.skinIndex == 0) {
            for (int i = 0; i < 3; i++) {
                materialTemp[i] = listMaterial[data.weaponMaterials[i]];
            }
        }
        else
            materialTemp = weaponObjects.GetListMaterials(weaponIndex, data.skinIndex);
        return materialTemp;
    }

    #endregion

    #region ----------------SKIN_DATA--------------
    public SkinData GetSkinData() {
        if (!PlayerPrefs.HasKey(GameVariable.PLAYER_SKIN)) {
            SkinData skinData = new SkinData(
                hatObjects.listHats.Length,
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
