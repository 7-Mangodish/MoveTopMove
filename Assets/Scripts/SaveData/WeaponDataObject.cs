using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDataObject", menuName = "Scriptable Objects/WeaponDataObject")]
public class WeaponDataObject : ScriptableObject
{
    public List <WeaponData> listWeaponDatas = new List<WeaponData>();

    public int GetSkinIndexSelected(int weaponIndex) {
        return listWeaponDatas[weaponIndex].skinIndexSelected;
    }

    public int[] GetMaterialIndexSelected(int weaponIndex) {
        return listWeaponDatas[weaponIndex].materialIndexSelected;
    }

    public void SetSkinIndexSelected(int weaponIndex,  int skinIndex) {
        listWeaponDatas[weaponIndex].skinIndexSelected = skinIndex;
    }

    public void SetMaterialIndexSelected(int weaponIndex, int weaponPartIndex, int materialIndex) {
        listWeaponDatas[weaponIndex].materialIndexSelected[weaponPartIndex] = materialIndex;
    }
}
