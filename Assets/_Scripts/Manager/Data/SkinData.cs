using UnityEngine;

[System.Serializable]
public class SkinData
{
    public int hatIndex;
    public int pantIndex;
    public int armorIndex;
    public int setIndex;
    public bool isSet;

    public int[] isUnLockHat;
    public int[] isUnLockPant;
    public int[] isUnLockArmor;
    public int[] isUnLockSet;

    public SkinData(int hatCount, int pantCount, int armorCount, int setCount) {
        isUnLockHat = new int[hatCount];
        isUnLockPant = new int[pantCount];
        isUnLockArmor = new int[armorCount];
        isUnLockSet = new int[setCount];

    }
}
