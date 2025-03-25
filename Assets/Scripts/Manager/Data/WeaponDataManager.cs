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
    public SaveData GetWeaponData(int weaponIndex) {
        string json = PlayerPrefs.GetString(weaponIndex.ToString());
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        return data;
    }

    public class SaveData {
        public int skinIndex;
        public int[] weaponMaterials = new int[3];

        public string showMaterial() {
            return weaponMaterials[0] + " " + weaponMaterials[1] + " " + weaponMaterials[2];
        }
    }

    public void SaveWeaponData(int weaponIndex, SaveData data) {
        string json = JsonUtility.ToJson(data);
        Debug.Log("Save Weapon: " + weaponIndex + ", skin: " + data.skinIndex + ", " + data.showMaterial());
        PlayerPrefs.SetString(weaponIndex.ToString(), json);
        OnUserChangeWeapon?.Invoke(this, EventArgs.Empty);
    }
}
