using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ConfigPlayerSkinShop", menuName = "Scriptable Objects/ConfigPlayerSkinShop", order = 1)]
public class ConfigPlayerSkinShop : ScriptableObject{
    public S_SkinShopInfor[] listHatSkinShop;
    public S_SkinShopInfor[] listPantSkinShop;
    public S_SkinShopInfor[] listArmorSkinShop;
    public S_SkinShopInfor[] listSetSkinShop;
}

[System.Serializable]
public struct S_SkinShopInfor {
    public int id;
    public int cost;
    public int adsWatchCost;
    public Sprite skinSprite;
}