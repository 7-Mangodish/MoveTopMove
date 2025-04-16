using UnityEngine;

[System.Serializable]
public class WeaponData
{
    public int skinIndex;
    public int[] weaponMaterials = { 17, 17, 17 };
    public bool isLock = true;
    public bool[] isLockSkin = new bool[5];
    public int adQuantity;

    public WeaponData() {
        skinIndex = 2;
        for (int i = 0; i < 5; i++) {
            if (i < 3)
                isLockSkin[i] = false;
            else
                isLockSkin[i] = true;
        }
    }
    public string showMaterial() {
        return weaponMaterials[0] + " " + weaponMaterials[1] + " " + weaponMaterials[2];
    }
}
