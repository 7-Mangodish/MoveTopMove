
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponObjects", menuName = "Scriptable Objects/WeaponObjects")]
public class WeaponObjects : ScriptableObject
{
    public Weapon[] listWeapon;

    public Mesh GetMeshWeapon(int weaponIndex, int weaponSkinIndex) {
        GameObject weapon = listWeapon[weaponIndex].GetWeaponSkin(weaponSkinIndex);
        return weapon.GetComponent<MeshFilter>().sharedMesh;
    }

    public Material[] GetListMaterials(int weaponIndex, int weaponSkinIndex) {
        GameObject weapon = listWeapon[weaponIndex].GetWeaponSkin(weaponSkinIndex);
        Material[] materials = weapon.GetComponent<MeshRenderer>().sharedMaterials;
        return materials;
    }

    public void  SetWeaponPartMaterial(int weaponIndex, int weaponSkinIndex, int weaponPart, Material material) {
        listWeapon[weaponIndex].SetMaterialWeaponSkin(weaponSkinIndex, weaponPart, material);
    }

    public void TurnOffWeapon(int weaponIndex) {
        listWeapon[weaponIndex].TurnOff();
    }
}
