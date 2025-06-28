using UnityEngine;
using UnityEngine.UI;

public class SkinShopButtonUI : MonoBehaviour
{
    [Header("______ButtonInfor_____")]
    public Image skinImage;
    public Button skinButton;
    public S_SkinShopInfor skinShopInfor;
    public SkinData skinData;
    [Header("_____UI_____")]
    public GameObject unlockImage;
    public void InitButtonInfor(S_SkinShopInfor infor, SkinData skinData) {
        if (infor.id == 0) {
            this.gameObject.SetActive(false);
            return;
        }
        skinShopInfor = infor;
        skinImage.sprite = infor.skinSprite;
        this.skinData = skinData;
        if (skinData.skinType != E_SkinType.NotBuy)
            UnlockSkinImage();
    }
    public void UnlockSkinImage() {
        unlockImage.SetActive(false);
    }
    public void ChangeStateButton(E_SkinType type) {
        skinData.skinType = type;
        if (type == E_SkinType.Use)
            UnlockSkinImage();
    }
}
