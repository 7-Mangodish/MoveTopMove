using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkinShopController : MonoBehaviour {
    private static SkinShopController ins;
    public static SkinShopController Ins { get { return ins; } }
 
    [Header("-----Top Panel-----")]
    public Button[] listShopButton;
    public Image[] listShopIcon;
    public GameObject[] listShopPanel;
    public Transform hatPanelTrans;
    public Transform pantPanelTrans;
    public Transform armorPanelTrans;
    public Transform setPanelTrans;
    public bool isOpenedSetPanel;
    //
    [Header("-----SkinButtonUI------")]
    public Dictionary<int, SkinShopButtonUI> dictHatButtonUI = new Dictionary<int, SkinShopButtonUI>();
    public Dictionary<int, SkinShopButtonUI> dictPantButtonUI = new Dictionary<int, SkinShopButtonUI>();
    public Dictionary<int, SkinShopButtonUI> dictArmorButtonUI = new Dictionary<int, SkinShopButtonUI>();
    public Dictionary<int, SkinShopButtonUI> dictSetButtonUI = new Dictionary<int, SkinShopButtonUI>();
    //
    [Header("-----Button-----")]
    public SkinShopButtonUI skinShopButtonPref;
    public Button selectButton;
    public GameObject purchaseSystemButton;
    public Button purchaseByAdButton;
    public Button purchaseButton;
    public Button equipedButton;
    public GameObject borderImagePref;
    public GameObject equippedImagePref;
    public Button exitButton;
    [HideInInspector] public SkinShopButtonUI currentSkinShopButtonUI; // Luu button dang duoc chon
    [HideInInspector] public SkinShopButtonUI equippedSkinShopButtonUI; // Luu button dang dang dung
    [HideInInspector] public GameObject currentBorderImage;
    [HideInInspector] public GameObject currentEquippedImage;
    public int idHatSelect;
    public int idPantSelect;
    public int idArmorSelect;
    public int idSetSelect;
    //
    [Header("-----Text-----")]
    public TextMeshProUGUI skinCostTMP;
    public TextMeshProUGUI adsCostTMP;

    private enum TypeSkin {
        hat,
        pant,
        armor,
        set
    }
    private TypeSkin type;

    private void OnEnable() {
        MaxManager.Instance.OnPlayerReceiveAward += MaxManager_OnPlayerReceiveAward;
    }

    private void OnDisable() {
        if (MaxManager.Instance != null)
            MaxManager.Instance.OnPlayerReceiveAward -= MaxManager_OnPlayerReceiveAward;
        else
            Debug.LogWarning("Max is null");

    }
    public void Awake() {
        if (ins == null) ins = this;
        else Destroy(this.gameObject);
        //
        InitSkinShop();
        SetUpEventShopButton();
        SetUpEventButtonUI();
        SetUpEventPurchaseButton();
        SetUpEventPurchaseByAdButton();
        SetUpEventSelecButton();
        SetUpEventExitButton();
    }

    // Tao cac nut skin va setup cac thuoc tinh cua nut
    public void InitSkinShop() {
        for(int i=0; i< DataManager.Instance.configPlayerSkinShop.listHatSkinShop.Length; i++) {
            SkinShopButtonUI newButton = Instantiate(skinShopButtonPref, hatPanelTrans);
            int id = DataManager.Instance.configPlayerSkinShop.listHatSkinShop[i].id;
            newButton.InitButtonInfor(DataManager.Instance.configPlayerSkinShop.listHatSkinShop[i], DataManager.Instance.playerData.listHatSkinData[id]);
            dictHatButtonUI[newButton.skinShopInfor.id] = newButton;
            if (newButton.skinShopInfor.id == DataManager.Instance.playerData.currentHatId) {
                SetUpEquippedImage(newButton.transform);
                SetUpBorderImage(newButton.transform);
            }
        }
        for (int i = 0; i < DataManager.Instance.configPlayerSkinShop.listPantSkinShop.Length; i++) {
            SkinShopButtonUI newButton = Instantiate(skinShopButtonPref, pantPanelTrans);
            int id = DataManager.Instance.configPlayerSkinShop.listPantSkinShop[i].id;
            newButton.InitButtonInfor(DataManager.Instance.configPlayerSkinShop.listPantSkinShop[i], DataManager.Instance.playerData.listPantSkinData[id]);
            //listPantButtonUI.Add(newButton);
            dictPantButtonUI[newButton.skinShopInfor.id] = newButton;
        }
        for (int i = 0; i < DataManager.Instance.configPlayerSkinShop.listArmorSkinShop.Length; i++) {
            SkinShopButtonUI newButton = Instantiate(skinShopButtonPref, armorPanelTrans);
            int id = DataManager.Instance.configPlayerSkinShop.listArmorSkinShop[i].id;
            newButton.InitButtonInfor(DataManager.Instance.configPlayerSkinShop.listArmorSkinShop[i], DataManager.Instance.playerData.listArmorSkinData[id]);
            //listArmorButtonUI.Add(newButton);
            dictArmorButtonUI[newButton.skinShopInfor.id] = newButton;
        }
        for (int i = 0; i < DataManager.Instance.configPlayerSkinShop.listSetSkinShop.Length; i++) {
            SkinShopButtonUI newButton = Instantiate(skinShopButtonPref, setPanelTrans);
            int id = DataManager.Instance.configPlayerSkinShop.listSetSkinShop[i].id;
            newButton.InitButtonInfor(DataManager.Instance.configPlayerSkinShop.listSetSkinShop[i], DataManager.Instance.playerData.listSetSkinData[id]);
            //listSetButtonUI.Add(newButton);
            dictSetButtonUI[newButton.skinShopInfor.id] = newButton;
        }
    }

    // Duoc goi khi moi lan mo shop skin
    public void SetUpOpenShop() {
        for (int i = 0; i < listShopButton.Length; i++) {
            if (!DataManager.Instance.playerData.isSet) {
                equippedSkinShopButtonUI = dictHatButtonUI[DataManager.Instance.playerData.currentHatId];
                if (i == 0) {
                    listShopPanel[i].SetActive(true);
                    listShopButton[i].GetComponent<Image>().enabled = false;
                    listShopIcon[i].color = Color.white;
                }
                else {
                    listShopPanel[i].SetActive(false);
                    listShopButton[i].GetComponent<Image>().enabled = true;
                    listShopIcon[i].color = Color.gray;
                    listShopPanel[i].gameObject.SetActive(false);
                }
            }
            else {
                equippedSkinShopButtonUI = dictSetButtonUI[DataManager.Instance.playerData.currentSetId];
                if (i == 3) {
                    listShopPanel[i].SetActive(true);
                    listShopButton[i].GetComponent<Image>().enabled = false;
                    listShopIcon[i].color = Color.white;
                }
                else {
                    listShopPanel[i].SetActive(false);
                    listShopButton[i].GetComponent<Image>().enabled = true;
                    listShopIcon[i].color = Color.gray;
                    listShopPanel[i].gameObject.SetActive(false);
                }
            }
        }
        idHatSelect = DataManager.Instance.playerData.currentHatId;
        idPantSelect = DataManager.Instance.playerData.currentPantId;
        idArmorSelect = DataManager.Instance.playerData.currentArmorId;
        idSetSelect = DataManager.Instance.playerData.currentSetId;
        SetUpEquippedImage(equippedSkinShopButtonUI.transform);
        SetUpBorderImage(equippedSkinShopButtonUI.transform);
        if (equippedSkinShopButtonUI.skinShopInfor.id != 0)
            equipedButton.gameObject.SetActive(true);
    }

    void SetUpEventShopButton() {
        for (int i = 0; i < listShopButton.Length; i++) {
            int ind = i;
            listShopButton[i].onClick.AddListener(() => {
                AudioManager.Instance.PlaySoundClickButton();
                switch (ind) {
                    case 0: {
                            type = TypeSkin.hat;
                            currentSkinShopButtonUI = dictHatButtonUI[idHatSelect];
                            equippedSkinShopButtonUI = dictHatButtonUI[DataManager.Instance.playerData.currentHatId];
                            break;
                        }
                    case 1: {
                            type = TypeSkin.pant;
                            currentSkinShopButtonUI = dictPantButtonUI[idPantSelect];
                            equippedSkinShopButtonUI = dictPantButtonUI[DataManager.Instance.playerData.currentPantId];
                            break;
                        }
                    case 2: {
                            type = TypeSkin.armor;
                            currentSkinShopButtonUI = dictArmorButtonUI[idArmorSelect];
                            equippedSkinShopButtonUI = dictArmorButtonUI[DataManager.Instance.playerData.currentArmorId];
                            break;
                        }
                    case 3: {
                            type = TypeSkin.set;
                            currentSkinShopButtonUI = dictSetButtonUI[idSetSelect];
                            equippedSkinShopButtonUI = dictSetButtonUI[DataManager.Instance.playerData.currentSetId];
                            break;
                        }
                }
                //Tat va bat UI
                listShopButton[ind].GetComponent<Image>().enabled = false;
                listShopIcon[ind].color = Color.white;
                listShopPanel[ind].gameObject.SetActive(true);
                for (int j = 0; j < listShopIcon.Length; j++) {
                    if (j != ind) {
                        listShopButton[j].GetComponent<Image>().enabled = true;
                        listShopIcon[j].color = Color.gray;
                        listShopPanel[j].gameObject.SetActive(false);
                    }
                }
                //
                if (type == TypeSkin.set) {
                    PlayerController.Instance.playerSkin.ChangeSkinSet(idSetSelect);
                }
                else {
                    PlayerController.Instance.playerSkin.SetActiveSet(false);
                    PlayerController.Instance.playerSkin.ChangeSkinHat(idHatSelect);
                    PlayerController.Instance.playerSkin.ChangeSkinPant(idPantSelect);
                    PlayerController.Instance.playerSkin.ChangeSkinArmor(idArmorSelect);
                }
                //
                SetUpBorderImage(currentSkinShopButtonUI.transform);
                SetUpEquippedImage(equippedSkinShopButtonUI.transform);
                equipedButton.gameObject.SetActive(false);
                selectButton.gameObject.SetActive(false);
                purchaseSystemButton.SetActive(false);
                if (currentSkinShopButtonUI.skinShopInfor.id == 0)
                    return;
                switch (currentSkinShopButtonUI.skinData.skinType) {
                    case E_SkinType.NotBuy:
                        purchaseSystemButton.SetActive(true);
                        break;
                    case E_SkinType.Buy: //truong hop da mua nhung khong dung
                        selectButton.gameObject.SetActive(true);
                        break;
                    case E_SkinType.Use:
                        equipedButton.gameObject.SetActive(true);
                        break;
                }
            });
        }
    }

    void SetUpEventButtonUI() {
        foreach(SkinShopButtonUI btn in dictHatButtonUI.Values) {
            btn.skinButton.onClick.AddListener(() => {
                AudioManager.Instance.PlaySoundClickButton();
                PlayerController.Instance.playerSkin.ChangeSkinHat(btn.skinShopInfor.id);
                //
                SetUpBorderImage(btn.transform);
                currentSkinShopButtonUI = btn;
                idHatSelect = currentSkinShopButtonUI.skinShopInfor.id;
                // Thay doi UI
                skinCostTMP.text = btn.skinShopInfor.cost.ToString();
                adsCostTMP.text = string.Format("{0}/{1}", btn.skinData.adsWatch, btn.skinShopInfor.adsWatchCost);
                purchaseSystemButton.gameObject.SetActive(false);
                selectButton.gameObject.SetActive(false);
                equipedButton.gameObject.SetActive(false);
                switch (btn.skinData.skinType) {
                    case E_SkinType.NotBuy:
                        purchaseSystemButton.SetActive(true);
                        break;
                    case E_SkinType.Buy: //truong hop da mua nhung khong dung
                        selectButton.gameObject.SetActive(true);
                        break;
                    case E_SkinType.Use:
                        equipedButton.gameObject.SetActive(true);
                        break;
                }
            });
        }
        foreach (SkinShopButtonUI btn in dictPantButtonUI.Values) {
            btn.skinButton.onClick.AddListener(() => {
                AudioManager.Instance.PlaySoundClickButton();
                PlayerController.Instance.playerSkin.ChangeSkinPant(btn.skinShopInfor.id);
                //
                SetUpBorderImage(btn.transform);
                currentSkinShopButtonUI = btn;
                idPantSelect = currentSkinShopButtonUI.skinShopInfor.id;
                // Thay doi UI
                skinCostTMP.text = btn.skinShopInfor.cost.ToString();
                adsCostTMP.text = string.Format("{0}/{1}", btn.skinData.adsWatch, btn.skinShopInfor.adsWatchCost);
                purchaseSystemButton.gameObject.SetActive(false);
                selectButton.gameObject.SetActive(false);
                equipedButton.gameObject.SetActive(false);
                switch (btn.skinData.skinType) {
                    case E_SkinType.NotBuy:
                        purchaseSystemButton.SetActive(true);
                        break;
                    case E_SkinType.Buy: //truong hop da mua nhung khong dung
                        selectButton.gameObject.SetActive(true);
                        break;
                    case E_SkinType.Use:
                        equipedButton.gameObject.SetActive(true);
                        break;
                }
            });
        }
        foreach (SkinShopButtonUI btn in dictArmorButtonUI.Values) {
            btn.skinButton.onClick.AddListener(() => {
                AudioManager.Instance.PlaySoundClickButton();
                PlayerController.Instance.playerSkin.ChangeSkinArmor(btn.skinShopInfor.id);
                //
                SetUpBorderImage(btn.transform);
                currentSkinShopButtonUI = btn;
                idArmorSelect = currentSkinShopButtonUI.skinShopInfor.id;
                // Thay doi UI
                skinCostTMP.text = btn.skinShopInfor.cost.ToString();
                adsCostTMP.text = string.Format("{0}/{1}", btn.skinData.adsWatch, btn.skinShopInfor.adsWatchCost);
                purchaseSystemButton.gameObject.SetActive(false);
                selectButton.gameObject.SetActive(false);
                equipedButton.gameObject.SetActive(false);
                switch (btn.skinData.skinType) {
                    case E_SkinType.NotBuy:
                        purchaseSystemButton.SetActive(true);
                        break;
                    case E_SkinType.Buy: //truong hop da mua nhung khong dung
                        selectButton.gameObject.SetActive(true);
                        break;
                    case E_SkinType.Use:
                        equipedButton.gameObject.SetActive(true);
                        break;
                }
            });
        }
        foreach (SkinShopButtonUI btn in dictSetButtonUI.Values) {
            btn.skinButton.onClick.AddListener(() => {
                AudioManager.Instance.PlaySoundClickButton();
                PlayerController.Instance.playerSkin.ChangeSkinSet(btn.skinShopInfor.id);
                //
                SetUpBorderImage(btn.transform);
                currentSkinShopButtonUI = btn;
                idSetSelect = currentSkinShopButtonUI.skinShopInfor.id;
                // Thay doi UI
                skinCostTMP.text = btn.skinShopInfor.cost.ToString();
                adsCostTMP.text = string.Format("{0}/{1}", btn.skinData.adsWatch, btn.skinShopInfor.adsWatchCost);
                purchaseSystemButton.gameObject.SetActive(false);
                selectButton.gameObject.SetActive(false);
                equipedButton.gameObject.SetActive(false);
                switch (btn.skinData.skinType) {
                    case E_SkinType.NotBuy:
                        purchaseSystemButton.SetActive(true);
                        break;
                    case E_SkinType.Buy: //truong hop da mua nhung khong dung
                        selectButton.gameObject.SetActive(true);
                        break;
                    case E_SkinType.Use:
                        equipedButton.gameObject.SetActive(true);
                        break;
                }
            });
        }
    }
    void SetUpBorderImage(Transform parentTranform) {
        if (currentBorderImage == null)
            currentBorderImage = Instantiate(borderImagePref, parentTranform);
        currentBorderImage.transform.SetParent(parentTranform);
        currentBorderImage.transform.localPosition = Vector3.zero;
    }

    void SetUpEventSelecButton() {
        selectButton.onClick.AddListener(() => {
            switch (type) {
                case TypeSkin.hat:
                    DataManager.Instance.playerData.currentHatId = currentSkinShopButtonUI.skinShopInfor.id;
                    DataManager.Instance.playerData.isSet = false;
                    break;
                case TypeSkin.pant:
                    DataManager.Instance.playerData.currentPantId = currentSkinShopButtonUI.skinShopInfor.id;
                    DataManager.Instance.playerData.isSet = false;
                    break;
                case TypeSkin.armor:
                    DataManager.Instance.playerData.currentArmorId = currentSkinShopButtonUI.skinShopInfor.id;
                    DataManager.Instance.playerData.isSet = false;
                    break;
                case TypeSkin.set:
                    DataManager.Instance.playerData.currentSetId = currentSkinShopButtonUI.skinShopInfor.id;
                    DataManager.Instance.playerData.isSet = true;
                    break;
            }
            selectButton.gameObject.SetActive(false);
            equipedButton.gameObject.SetActive(true);
            SetUpEquippedImage(currentSkinShopButtonUI.transform);
            UpdateSkinType();
            AudioManager.Instance.PlaySoundClickButton();

        });
    }
    void SetUpEventPurchaseButton() {
        purchaseButton.onClick.AddListener(() => {
            AudioManager.Instance.PlaySoundClickButton();
            if (DataManager.Instance.playerData.coin < currentSkinShopButtonUI.skinShopInfor.cost)
                Debug.Log("Khong Du Tien");
            else {
                switch (type) {
                    case TypeSkin.hat:
                        // Cap nhat lai skin dang dung
                        DataManager.Instance.playerData.currentHatId = currentSkinShopButtonUI.skinShopInfor.id;
                        break;
                    case TypeSkin.pant:
                        DataManager.Instance.playerData.currentPantId = currentSkinShopButtonUI.skinShopInfor.id;
                        break;
                    case TypeSkin.armor:
                        DataManager.Instance.playerData.currentArmorId = currentSkinShopButtonUI.skinShopInfor.id;
                        break;
                    case TypeSkin.set:
                        DataManager.Instance.playerData.currentSetId = currentSkinShopButtonUI.skinShopInfor.id;
                        DataManager.Instance.playerData.isSet = true;
                        break;
                }
                purchaseSystemButton.gameObject.SetActive(false);
                equipedButton.gameObject.SetActive(true);
                SetUpEquippedImage(currentSkinShopButtonUI.transform);
                UpdateSkinType();
                DataManager.Instance.UpdatePlayerCoin(-currentSkinShopButtonUI.skinShopInfor.cost);
                HomePageController.Instance.SetCoinText();
            }

        });
    }

    // Ham duoc goi de cap nhat cac Type trong DB va trong cac button khi select 1 skin hoac mua 1 skin
    public void UpdateSkinType() {
        if(equippedSkinShopButtonUI != null) 
            equippedSkinShopButtonUI.ChangeStateButton(E_SkinType.Buy);
        currentSkinShopButtonUI.ChangeStateButton(E_SkinType.Use);
        equippedSkinShopButtonUI = currentSkinShopButtonUI;
    }

    void HandlerPurchaseByAdsButton() {
        currentSkinShopButtonUI.skinData.adsWatch += 1;
        adsCostTMP.text = string.Format("{0}/{1}", currentSkinShopButtonUI.skinData.adsWatch, currentSkinShopButtonUI.skinShopInfor.adsWatchCost);
        if (currentSkinShopButtonUI.skinData.adsWatch >= 2) {
            switch (type) {
                case TypeSkin.hat:
                    // Cap nhat lai skin dang dung
                    DataManager.Instance.playerData.currentHatId = currentSkinShopButtonUI.skinShopInfor.id;
                    break;
                case TypeSkin.pant:
                    DataManager.Instance.playerData.currentPantId = currentSkinShopButtonUI.skinShopInfor.id;
                    break;
                case TypeSkin.armor:
                    DataManager.Instance.playerData.currentArmorId = currentSkinShopButtonUI.skinShopInfor.id;
                    break;
                case TypeSkin.set:
                    DataManager.Instance.playerData.currentSetId = currentSkinShopButtonUI.skinShopInfor.id;
                    DataManager.Instance.playerData.isSet = true;
                    break;
            }
            purchaseSystemButton.gameObject.SetActive(false);
            equipedButton.gameObject.SetActive(true);
            SetUpEquippedImage(currentSkinShopButtonUI.transform);
            SetUpBorderImage(currentSkinShopButtonUI.transform);
            UpdateSkinType();
        }

    }
    void SetUpEventPurchaseByAdButton() {
        purchaseByAdButton.onClick.AddListener(() => {
            if (MaxManager.Instance != null) {
                MaxManager.Instance.ShowRewardAd();
                MaxManager.Instance.SetTypeReward(MaxManager.TypeReward.playerSkin);
            }

        });
    }

    void SetUpEquippedImage(Transform parentTransform) {
        if (currentEquippedImage == null)
           currentEquippedImage = Instantiate(equippedImagePref, parentTransform);
        currentEquippedImage.transform.SetParent(parentTransform);
        currentEquippedImage.transform.localPosition = Vector3.zero;
    }
    void SetUpEventExitButton() {
        exitButton.onClick.AddListener(() => {
            AudioManager.Instance.PlaySoundClickButton();
            if (HomePageController.Instance != null)
                HomePageController.Instance.ExitSkinShop();
            if (DataManager.Instance.playerData.isSet == false) {
                PlayerController.Instance.playerSkin.ChangeSkinHat(DataManager.Instance.playerData.currentHatId);
                PlayerController.Instance.playerSkin.ChangeSkinPant(DataManager.Instance.playerData.currentPantId);
                PlayerController.Instance.playerSkin.ChangeSkinArmor(DataManager.Instance.playerData.currentArmorId);
            }
            else
                PlayerController.Instance.playerSkin.ChangeSkinSet(DataManager.Instance.playerData.currentSetId);
            equipedButton.gameObject.SetActive(false);
            selectButton.gameObject.SetActive(false);
            purchaseSystemButton.SetActive(false);
        });
    }
    private void MaxManager_OnPlayerReceiveAward(object sender, MaxManager.TypeReward t) {
        if (t == MaxManager.TypeReward.playerSkin) {
            HandlerPurchaseByAdsButton();
            FirebaseManager.Instance.HandlerClickAdEvent(FirebaseManager.TypeEvent.clickSkinShopAd, FirebaseManager.TypeAd.reward);
        }
    }

}
