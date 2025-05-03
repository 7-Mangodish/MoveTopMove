using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkinShopController : MonoBehaviour {
    private static SkinShopController instance;
    public static SkinShopController Instance { get { return instance; } }


    [SerializeField] private Material playerMaterial;

    [Header("-----Top Panel-----")]
    [SerializeField] private Button[] listShopButton;
    [SerializeField] private Image[] listShopIcon;
    [SerializeField] private GameObject[] listShopPanel;

    [Header("-----HatButton-----")]
    [SerializeField] private HatObjects hatObjects;
    [SerializeField] private Button[] listHatButtons;
    [SerializeField] private Transform hatHolderTransform;
    private int hatIndexSelected;

    [Header("-----PantButton-----")]
    [SerializeField] private PantObjects pantObjects;
    [SerializeField] private Button[] listPantButtons;
    [SerializeField] private GameObject characterPant;
    private SkinnedMeshRenderer pantSkin;
    private int pantIndexSelected;

    [Header("-----ArmorButton-----")]
    [SerializeField] private ArmorObjects armorObjects;
    [SerializeField] private Button[] listArmorButtons;
    [SerializeField] private Transform armorHolderTransform;
    private int armorIndexSelected;

    [Header("-----SetButton-----")]
    [SerializeField] private SetObjects setObjects;
    [SerializeField] private Button[] listSetButtons;
    [SerializeField] private Transform wingHolderTransform;
    [SerializeField] private Transform tailHolderTransform;
    [SerializeField] private SkinnedMeshRenderer playerMesh;
    private int setIndexSelected;
    private bool isSet;

    [Header("Button")]
    [SerializeField] private Button selectButton;
    [SerializeField] private GameObject purchaseSystemButton;
    [SerializeField] private Button purchaseByAdButton;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private Button equipedButton;
    [SerializeField] private GameObject borderImagePref;
    private GameObject currentBorderImage;
    [SerializeField] private GameObject equippedImagePref;
    private GameObject currentEquippedImage;
    [SerializeField] private Button exitSkinButton;


    [Header("Text")]
    [SerializeField] private TextMeshProUGUI purchaseText;
    [SerializeField] private TextMeshProUGUI skinInfor;
    private enum TypeSkin {
        hat,
        pant,
        armor,
        set
    }
    private TypeSkin type;
    private SkinData data;

    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    public void SetUpSkinShop() {
        data = DataManager.Instance.GetSkinData();

        SetUpShopButton();
        SetUpExitButton();

        SetUpListHatButton();
        SetUpListPantButton();
        SetUpListArmorButton();
        SetUpListSetButton();

        SetUpPurchaseButton();
        SetUpPurchaseByAdButton();
        SetUpSelecButton();

        pantSkin = characterPant.GetComponent<SkinnedMeshRenderer>();

        //Debug.Log("Hat: " + data.hatIndex + ", Pant: " +
        //    data.pantIndex + ", Armor: " + data.armorIndex + ", Set: " + data.setIndex + ", isSet:" + data.isSet);
        hatIndexSelected = data.hatIndex;
        pantIndexSelected = data.pantIndex;
        armorIndexSelected = data.armorIndex;
        setIndexSelected = data.setIndex;

        if (data.isSet) {
            listShopButton[3].onClick.Invoke();
        }
        else {
            listShopButton[0].onClick.Invoke();
        }

        MaxManager.Instance.OnPlayerReceiveAward += SkinShop_OnPlayerReceiveAward;
    }

    private void OnDisable() {
        if (MaxManager.Instance != null)
            MaxManager.Instance.OnPlayerReceiveAward -= SkinShop_OnPlayerReceiveAward;
        else
            Debug.LogWarning("Max is null");

    }

    void SetUpSelecButton() {
        selectButton.onClick.AddListener(() => {
            //data = GetSkinData();
            data = DataManager.Instance.GetSkinData();

            switch (type) {
                case TypeSkin.hat: {
                        data.hatIndex = hatIndexSelected;
                        SetUpEquippedImage(listHatButtons[hatIndexSelected].transform);
                        break;
                    }
                case TypeSkin.pant: {
                        data.pantIndex = pantIndexSelected;
                        SetUpEquippedImage(listPantButtons[pantIndexSelected].transform);
                        break;
                    }
                case TypeSkin.armor: {
                        data.armorIndex = armorIndexSelected;
                        SetUpEquippedImage(listArmorButtons[armorIndexSelected].transform);
                        break;
                    }
                case TypeSkin.set: {
                        data.setIndex = setIndexSelected;
                        SetUpEquippedImage(listSetButtons[setIndexSelected].transform);
                        break;
                    }
            }
            //SaveSkinData(data);
            DataManager.Instance.SaveSkinData(data);
            selectButton.gameObject.SetActive(false);
            equipedButton.gameObject.SetActive(true);
        });
    }
    void SetUpPurchaseButton() {
        purchaseButton.onClick.AddListener(() => {
            AudioManager.Instance.PlaySoundClickButton();

            if (!PurchaseSkin())
                Debug.Log("Khong Du Tien");
            else {
                purchaseSystemButton.gameObject.SetActive(false);
                equipedButton.gameObject.SetActive(true);
                switch (type) {
                    case TypeSkin.hat: {
                            SetUpEquippedImage(listHatButtons[hatIndexSelected].transform);
                            break;
                        }
                    case TypeSkin.pant: {
                            SetUpEquippedImage(listPantButtons[pantIndexSelected].transform);
                            break;
                        }
                    case TypeSkin.armor: {
                            SetUpEquippedImage(listArmorButtons[armorIndexSelected].transform);
                            break;
                        }
                    case TypeSkin.set: {
                            SetUpEquippedImage(listSetButtons[setIndexSelected].transform);
                            break;
                        }
                }
            }

        });
    }
    void HandlerPurchaseByAdButton() {
        Transform child = null;
        //data = GetSkinData();

        purchaseSystemButton.gameObject.SetActive(false);
        equipedButton.gameObject.SetActive(true);
        switch (type) {
            case TypeSkin.hat: {

                    child = FindChildWithTag(listHatButtons[hatIndexSelected].transform);
                    //hatObjects.listHats[hatIndexSelected].isLock = false;
                    data.isUnLockHat[hatIndexSelected] = 1;

                    SetUpEquippedImage(listHatButtons[hatIndexSelected].transform);
                    data.hatIndex = hatIndexSelected;
                    break;
                }
            case TypeSkin.pant: {

                    child = FindChildWithTag(listPantButtons[pantIndexSelected].transform);
                    SetUpEquippedImage(listPantButtons[pantIndexSelected].transform);

                    data.isUnLockPant[pantIndexSelected] = 1;
                    data.pantIndex = pantIndexSelected;
                    break;
                }
            case TypeSkin.armor: {

                    child = FindChildWithTag(listArmorButtons[armorIndexSelected].transform);
                    SetUpEquippedImage(listArmorButtons[armorIndexSelected].transform);

                    data.isUnLockArmor[armorIndexSelected] = 1;
                    data.armorIndex = armorIndexSelected;
                    break;
                }
            case TypeSkin.set: {

                    child = FindChildWithTag(listSetButtons[setIndexSelected].transform);
                    SetUpEquippedImage(listSetButtons[setIndexSelected].transform);

                    data.isUnLockSet[setIndexSelected] = 1;
                    data.setIndex = setIndexSelected;
                    break;
                }
        }

        if (child != null) {
            child.gameObject.SetActive(false);
        }
        DataManager.Instance.SaveSkinData(data);
    }
    void SetUpPurchaseByAdButton() {
        purchaseByAdButton.onClick.AddListener(() => {
            if(MaxManager.Instance!= null) {
                MaxManager.Instance.ShowRewardAd();
                MaxManager.Instance.SetTypeReward(MaxManager.TypeReward.playerSkin);
            }

        });
    }
    void SetUpListHatButton() {
        if (data == null)
            Debug.Log("data is null");
        for (int i = 0; i < listHatButtons.Length; i++) {
            int ind = i;
            
            if (data.isUnLockHat[ind] == 1) {
                Transform child = FindChildWithTag(listHatButtons[ind].transform);
                if (child != null) {
                    child.gameObject.SetActive(false);
                }
            }

            listHatButtons[i].onClick.AddListener(() => {
                AudioManager.Instance.PlaySoundClickButton();

                hatIndexSelected = ind;

                skinInfor.text = "+" + hatObjects.listHats[ind].index.ToString() + " Range";
                purchaseText.text = hatObjects.listHats[ind].cost.ToString();

                SetUpBorderImage(listHatButtons[ind].transform);
                SetupPurchaseAndSelectButton(data.isUnLockHat[ind]);
                if (selectButton.IsActive())
                    SetUpSelectAndEquipedButton(type, ind);

                hatObjects.SetCharacterHat(ind, hatHolderTransform);
            });
        }
    }
    void SetUpListPantButton() {
        for (int i = 0; i < listPantButtons.Length; i++) {
            int ind = i;

            if (data.isUnLockPant[ind] == 1) {
                Transform child = FindChildWithTag(listPantButtons[ind].transform);
                if (child != null) {
                    child.gameObject.SetActive(false);
                }
            }

            listPantButtons[i].onClick.AddListener(() => {
                AudioManager.Instance.PlaySoundClickButton();

                pantIndexSelected = ind;

                skinInfor.text = "+" + pantObjects.listPants[ind].index.ToString() + " Range";
                purchaseText.text = pantObjects.listPants[ind].cost.ToString();

                SetUpBorderImage(listPantButtons[ind].transform) ;
                SetupPurchaseAndSelectButton(data.isUnLockPant[ind]);
                if (selectButton.IsActive())
                    SetUpSelectAndEquipedButton(type, ind);

                pantObjects.SetPantMaterial(ind, pantSkin);

            });
        }
    }
    void SetUpListArmorButton() {
        for (int i = 0; i < listArmorButtons.Length; i++) {
            int ind = i;

            if (data.isUnLockArmor[ind] == 1) {
                Transform child = FindChildWithTag(listArmorButtons[ind].transform);
                if (child != null) {
                    child.gameObject.SetActive(false);
                }
            }

            listArmorButtons[i].onClick.AddListener(() => {
                AudioManager.Instance.PlaySoundClickButton();

                armorIndexSelected = ind;

                skinInfor.text = "+" + armorObjects.listArmor[ind].index.ToString() + " Hp";
                purchaseText.text = armorObjects.listArmor[ind].cost.ToString();

                SetUpBorderImage(listArmorButtons[ind].transform) ;
                SetupPurchaseAndSelectButton(data.isUnLockArmor[ind]);

                if (selectButton.IsActive())
                    SetUpSelectAndEquipedButton(type, ind);

                armorObjects.SetCharacterArmor(ind, armorHolderTransform);
            });
        }
    }
    void SetUpListSetButton() {
        for (int i = 0; i < listSetButtons.Length; i++) {
            int ind = i;

            if (data.isUnLockSet[ind] == 1) {
                //Debug.Log("UnLock" + ind);
                Transform child = FindChildWithTag(listSetButtons[ind].transform);
                if (child != null)
                {
                    child.gameObject.SetActive(false);
                }
            }         
            listSetButtons[i].onClick.AddListener(() => {
                AudioManager.Instance.PlaySoundClickButton();

                //Debug.Log("Set: " + ind);
                setIndexSelected = ind;

                skinInfor.text = "+" + setObjects.listSets[ind].index.ToString() + " Hp";
                purchaseText.text = setObjects.listSets[ind].cost.ToString();

                SetUpBorderImage(listSetButtons[ind].transform) ;
                SetupPurchaseAndSelectButton(data.isUnLockSet[ind]);
                if (selectButton.IsActive())
                    SetUpSelectAndEquipedButton(type, ind);

                setObjects.SetCharacterHatSet(ind, hatHolderTransform);
                setObjects.SetCharacterArmorSet(ind, armorHolderTransform);
                setObjects.SetCharacterWingSet(ind, wingHolderTransform);
                setObjects.SetCharacterTailSet(ind, tailHolderTransform);
                setObjects.SetCharacterMaterialSet(ind, playerMesh);


            });
        }
    }
    void SetUpShopButton() {
        for (int i = 0; i < listShopButton.Length; i++) {
            int ind = i;
            listShopButton[i].onClick.AddListener(() => {
                AudioManager.Instance.PlaySoundClickButton();
                equipedButton.gameObject.SetActive(false);
                purchaseSystemButton.gameObject.SetActive(false);

                switch (ind) {
                    case 0: {
                            type = TypeSkin.hat;
                            if (data == null)
                                Debug.LogWarning("data null");
                            if(data.hatIndex >= 0)
                                equipedButton.gameObject.SetActive(true);
                            break;
                        }
                    case 1: {
                            type = TypeSkin.pant;
                            if (data.pantIndex >= 0)
                                equipedButton.gameObject.SetActive(true);
                            break;
                        }
                    case 2: {
                            type = TypeSkin.armor;
                            if (data.armorIndex >= 0)
                                equipedButton.gameObject.SetActive(true);
                            break;
                        }
                    case 3: {
                            type = TypeSkin.set;
                            if (data.setIndex >= 0)
                                equipedButton.gameObject.SetActive(true);
                            break;
                        }
                }
                LoadSkin(type);

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
            });
        }
    }
    void SetUpSelectAndEquipedButton(TypeSkin type, int ind) {
        //SkinData skinData = GetSkinData();
        data = DataManager.Instance.GetSkinData();
        bool res = false;
        switch (type) {
            case TypeSkin.hat: {
                    if (data.hatIndex == ind)
                        res = true;
                    break;
                }
            case TypeSkin.pant: {
                    if (data.pantIndex == ind)
                        res = true;
                    break;
                }
            case TypeSkin.armor: {
                    if (data.armorIndex == ind)
                        res = true;
                    break;
                }
            case TypeSkin.set: {
                    if (data.setIndex == ind)
                        res = true;
                    break;
                }
        }

        if (res) {
            equipedButton.gameObject.SetActive(true);
            selectButton.gameObject.SetActive(false);
        }
        else {
            equipedButton.gameObject.SetActive(false);
            selectButton.gameObject.SetActive(true);
        }
    }
    void SetupPurchaseAndSelectButton(int isLock) {
        equipedButton.gameObject.SetActive(false);
        if (isLock == 0) {
            purchaseSystemButton.gameObject.SetActive(true);
            selectButton.gameObject.SetActive(false);
        }
        else {
            purchaseSystemButton.gameObject.SetActive(false);
            selectButton.gameObject.SetActive(true);
        }
    }
    void SetUpBorderImage(Transform parentTranform) {
        if (currentBorderImage != null)
            Destroy(currentBorderImage);
        currentBorderImage = Instantiate(borderImagePref, parentTranform);
    }
    void SetUpEquippedImage(Transform parentTransform) {
        if (currentEquippedImage != null){
            Destroy(currentEquippedImage);
        }
        currentEquippedImage = Instantiate(equippedImagePref, parentTransform);
    }
    void SetUpExitButton() {
        exitSkinButton.onClick.AddListener(() => {
                AudioManager.Instance.PlaySoundClickButton();

            if (HomePageController.Instance != null)
                HomePageController.Instance.ExitSkinShop();
            else
                Debug.LogError("HomePage is null");
            LoadSkin(type);
        });
    }
    void LoadSkin(TypeSkin typeSkin) {
        //data = GetSkinData();
        data = DataManager.Instance.GetSkinData();
        if (typeSkin == TypeSkin.set) {
            hatObjects.DestroyHat();
            characterPant.gameObject.SetActive(false);
            armorObjects.DestroyArmor();

            setObjects.SetCharacterHatSet(data.setIndex, hatHolderTransform);
            setObjects.SetCharacterArmorSet(data.setIndex, armorHolderTransform);
            setObjects.SetCharacterWingSet(data.setIndex, wingHolderTransform);
            setObjects.SetCharacterTailSet(data.setIndex, tailHolderTransform);
            setObjects.SetCharacterMaterialSet(data.setIndex, playerMesh);
            data.isSet = true;
        }
        else {
            setObjects.DestroySet();
            if (!characterPant.gameObject.activeSelf)
                characterPant.gameObject.SetActive(true);
            playerMesh.material = playerMaterial;
            hatObjects.SetCharacterHat(data.hatIndex, hatHolderTransform);
            pantObjects.SetPantMaterial(data.pantIndex, pantSkin);
            armorObjects.SetCharacterArmor(data.armorIndex, armorHolderTransform);

            data.isSet = false;
        }

        if (currentBorderImage != null)
            Destroy(currentBorderImage);
        switch (typeSkin) {
            case TypeSkin.hat: {
                    if (data.hatIndex >= 0) {
                        SetUpBorderImage(listHatButtons[data.hatIndex].transform);
                        SetUpEquippedImage(listHatButtons[data.hatIndex].transform);
                    }
                    break;
                }
            case TypeSkin.pant: {
                    if (data.pantIndex >= 0) {
                        SetUpBorderImage(listPantButtons[data.pantIndex].transform);
                        SetUpEquippedImage(listPantButtons[data.pantIndex].transform);
                    }
                    break;
                }
            case TypeSkin.armor: {
                    if (data.armorIndex >= 0) {
                        SetUpBorderImage(listArmorButtons[data.armorIndex].transform);
                        SetUpEquippedImage(listArmorButtons[data.armorIndex].transform);
                    }
                    break;
                }
            case TypeSkin.set: {
                    if (data.setIndex >= 0) {
                        SetUpBorderImage(listSetButtons[data.setIndex].transform);
                        SetUpEquippedImage(listSetButtons[data.setIndex].transform);
                    }
                    break;
                }
        }

    }
    Transform FindChildWithTag(Transform parentTransform) {
        foreach(Transform trans in parentTransform) {
            if (trans.gameObject.CompareTag("LockImage")) {
                return trans;
            }
        }
        return null;
    }

    bool PurchaseSkin() {
        Transform child = null;
        //data = GetSkinData();
        switch (type) {
            case TypeSkin.hat: {
                    if (!CoinManager.Instance.PurchaseItem(hatObjects.listHats[hatIndexSelected].cost))
                        return false;

                    child = FindChildWithTag(listHatButtons[hatIndexSelected].transform);
                    //hatObjects.listHats[hatIndexSelected].isLock = false;

                    SetUpEquippedImage(listHatButtons[hatIndexSelected].transform);

                    data.isUnLockHat[hatIndexSelected] = 1;
                    data.hatIndex = hatIndexSelected;
                    break;
                }
            case TypeSkin.pant: {
                    if (!CoinManager.Instance.PurchaseItem(pantObjects.listPants[pantIndexSelected].cost))
                        return false;

                    child = FindChildWithTag(listPantButtons[pantIndexSelected].transform);
                    SetUpEquippedImage(listPantButtons[pantIndexSelected].transform);

                    data.isUnLockPant[pantIndexSelected] = 1;
                    data.pantIndex = pantIndexSelected;
                    break;
                }
            case TypeSkin.armor: {
                    if (!CoinManager.Instance.PurchaseItem(armorObjects.listArmor[armorIndexSelected].cost))
                        return false;

                    child = FindChildWithTag(listArmorButtons[armorIndexSelected].transform);
                    SetUpEquippedImage(listArmorButtons[armorIndexSelected].transform);

                    data.isUnLockArmor[armorIndexSelected] = 1;
                    data.armorIndex = armorIndexSelected;
                    break;
                }
            case TypeSkin.set: {
                    if (!CoinManager.Instance.PurchaseItem(setObjects.listSets[setIndexSelected].cost))
                        return false;

                    child = FindChildWithTag(listSetButtons[setIndexSelected].transform);
                    SetUpEquippedImage(listSetButtons[setIndexSelected].transform);

                    data.isUnLockSet[setIndexSelected] = 1;
                    data.setIndex = setIndexSelected;
                    break;
                }
        }

        if (child != null) {
            child.gameObject.SetActive(false);
        }
        DataManager.Instance.SaveSkinData(data);
        HomePageController.Instance.SetCoinText();
        return true;
    }

    private void SkinShop_OnPlayerReceiveAward(object sender, MaxManager.TypeReward t) {
        if(t == MaxManager.TypeReward.playerSkin) {
            HandlerPurchaseByAdButton();
            FirebaseManager.Instance.HandlerClickAdEvent(FirebaseManager.TypeEvent.clickSkinShopAd, FirebaseManager.TypeAd.reward);
        }
    }
}
