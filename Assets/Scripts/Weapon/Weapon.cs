using UnityEngine;

[System.Serializable]
public class Weapon 
{

    //[SerializeField] private GameObject weaponObject;
    [SerializeField] private GameObject[] weaponSkins;
    public enum WeaponAttribute {
        None,
        Range, 
        Damage  
    }

    public WeaponAttribute attribute;
    public int index;

    //public GameObject GetWeapon() {
    //    return weaponObject;
    //}

    public GameObject GetWeaponSkin(int index) {
        return weaponSkins[index];
    }
}
