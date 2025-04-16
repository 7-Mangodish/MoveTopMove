using System;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private static DataManager instance;
    public static DataManager Instance { get => instance; }
    [Header("-----WEAPON-----")]
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
        Debug.Log("Save Weapon: " + weaponIndex + ", skin: " + data.skinIndex + ", " + data.showMaterial());
        PlayerPrefs.SetString(weaponIndex.ToString(), json);

        Material[] materialTemp = new Material[3];
        for (int i = 0; i < 3; i++) {
            materialTemp[i] = listMaterial[data.weaponMaterials[i]];
        }
        OnUserChangeWeapon?.Invoke(this, new OnUserChangeWeaponArg {
            skinIndex = data.skinIndex,
            materials = materialTemp

        });
    }

    #endregion

    #region ----------------SKIN_DATA--------------
    public SkinData GetSkinData() {
        if (!PlayerPrefs.HasKey("PlayerSkin")) {
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

        string json = PlayerPrefs.GetString("PlayerSkin");
        SkinData data = JsonUtility.FromJson<SkinData>(json);
        return data;
    }
    public void SaveSkinData(SkinData data) {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("PlayerSkin", json);
        Debug.Log("Save Skin: " + data.hatIndex + " " + data.pantIndex + " " + data.armorIndex + " " + data.setIndex);
    }

    #endregion
}
