using UnityEngine;
[System.Serializable]
public class WeaponData
{
    public int weaponId;
    public int skinIndex;
    public int[] weaponMaterials = { 17, 17, 17 };
    public bool isLock = true;
    public bool[] isLockSkin = new bool[5];
    public int adQuantity;
    public bool isBoom = false;

    public WeaponData(int id) {
        weaponId = id;
        skinIndex = 2;
        for (int i = 0; i < 5; i++) {
            if (i < 3)
                isLockSkin[i] = false;
            else
                isLockSkin[i] = true;
        }
    }
}
