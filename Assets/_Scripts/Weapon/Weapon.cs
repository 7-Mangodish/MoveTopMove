using JetBrains.Annotations;
using UnityEngine;

[System.Serializable]
public class Weapon 
{
    public int id;
    public GameObject[] weaponSkins;
    public int[] weaponSkinCost = new int[5];
    public float scale;
    public string name;
    public int cost;
    public bool isBoom;
    //[HideInInspector] public static float standardScale = 4000;
    public enum WeaponAttribute {
        None,
        Range, 
        Damage  
    }

    public WeaponAttribute attribute;
    public int index;

    public GameObject GetWeaponSkin(int index) {
        return weaponSkins[index];
    }
}
