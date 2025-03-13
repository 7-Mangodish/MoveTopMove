using UnityEngine;

[System.Serializable]
public class WeaponData
{
    public int skinIndexSelected;
    public int[] materialIndexSelected = new int[3] ;

    public WeaponData(int skinIndexSelected) {
        this.skinIndexSelected = skinIndexSelected;
    }
}
