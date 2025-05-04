
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponObjects", menuName = "Scriptable Objects/WeaponObjects")]
public class WeaponObjects : ScriptableObject
{
    public Weapon[] listWeapon;

    public Mesh GetMeshWeapon(int weaponId, int weaponSkinIndex) {
        Weapon weapon = GetWeaponById(weaponId);
        GameObject skinWeapon = weapon.GetWeaponSkin(weaponSkinIndex);
        return skinWeapon.GetComponent<MeshFilter>().sharedMesh;
    }

    public Material[] GetListMaterials(int weaponId, int weaponSkinIndex) {
        Weapon weapon = GetWeaponById(weaponId);
        GameObject weaponObject = weapon.GetWeaponSkin(weaponSkinIndex);
        Material[] materials = weaponObject.GetComponent<MeshRenderer>().sharedMaterials;
        return materials;
    }

    public Weapon GetWeaponById(int id) {
        Weapon weapon = null;
        foreach (Weapon obj in listWeapon) {
            if (obj.id == id) { weapon = obj; break; }
        }
        if(weapon == null) {
            Debug.LogWarning("Vu khi voi id " + id + "da bi xoa tren SC");
            return null;
        }
        return weapon;
    }

}
