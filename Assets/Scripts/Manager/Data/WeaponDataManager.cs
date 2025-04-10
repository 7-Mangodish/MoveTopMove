using System;
using UnityEngine;

public class WeaponDataManager : MonoBehaviour
{
    private static WeaponDataManager instance;
    public static WeaponDataManager Instance { get => instance; }

    public event EventHandler OnUserChangeWeapon;


    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else
            Destroy(this.gameObject);
    }
    public WeaponData GetWeaponData(int weaponIndex) {
        if (!PlayerPrefs.HasKey(weaponIndex.ToString())){
            WeaponData weaponData = new WeaponData();
            weaponData.skinIndex = 0;
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
        OnUserChangeWeapon?.Invoke(this, EventArgs.Empty);
    }

    [System.Serializable]
    public class WeaponData {
        public int skinIndex;
        public int[] weaponMaterials = new int[3];

        public string showMaterial() {
            return weaponMaterials[0] + " " + weaponMaterials[1] + " " + weaponMaterials[2];
        }
    }
}
