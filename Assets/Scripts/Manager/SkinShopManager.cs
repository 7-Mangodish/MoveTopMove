
using System;
using System.Data;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
using static UnityEngine.InputManagerEntry;

public class SkinShopManager : MonoBehaviour {


    [SerializeField] private Material playerMaterial;

    [Header("Top Panel")]
    [SerializeField] private Button[] listShopButton;
    [SerializeField] private Image[] listShopIcon;
    [SerializeField] private GameObject[] listShopPanel;

    [Header("HatButton")]
    [SerializeField] private HatObjects hatObjects;
    [SerializeField] private Button[] listHatButtons;
    [SerializeField] private Transform hatHolderTransform;
    private int hatIndexSelected;

    [Header("PantButton")]
    [SerializeField] private PantObjects pantObjects;
    [SerializeField] private Button[] listPantButtons;
    [SerializeField] private GameObject characterPant;
    private SkinnedMeshRenderer pantSkin;
    private int pantIndexSelected;

    [Header("ArmorButton")]
    [SerializeField] private ArmorObjects armorObjects;
    [SerializeField] private Button[] listArmorButtons;
    [SerializeField] private Transform armorHolderTransform;
    private int armorIndexSelected;

    [Header("SetButton")]
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

    //private int hatEquippedIndex;
    //private int pantEquippedIndex;
    //private int armorEquippedIndex;
    //private int setEquippedIndex;
    private void Awake() {

        SetUpShopButton();
        SetUpExitButton();

        SetUpListHatButton();
        SetUpListPantButton();
        SetUpListArmorButton();
        SetUpListSetButton();

        SetUpPurchaseButton();
        SetUpSelecButton();

    }

    void Start() {
        pantSkin = characterPant.GetComponent<SkinnedMeshRenderer>();
        data = GetSkinData();
        Debug.Log("Hat: " + data.hatIndex + ", Pant: " +
            data.pantIndex + ", Armor: " + data.armorIndex + ", Set: " + data.setIndex + ", isSet:" + data.isSet);
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
    }

    void SetUpSelecButton() {
        selectButton.onClick.AddListener(() => {
            data = GetSkinData();
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
            SaveSkinData(data);

            selectButton.gameObject.SetActive(false);
            equipedButton.gameObject.SetActive(true);
        });
    }

    void SetUpPurchaseButton() {
        purchaseButton.onClick.AddListener(() => {
            SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

            if (!PurchaseSkin())
                Debug.Log("Khong Du Tien");
            else {
                purchaseSystemButton.gameObject.SetActive(false);
                equipedButton.gameObject.SetActive(true);
            }

        });
    }
    void SetUpListHatButton() {
        for (int i = 0; i < listHatButtons.Length; i++) {
            int ind = i;
            
            if (!hatObjects.listHats[ind].isLock) {
                Transform child = FindChildWithTag(listHatButtons[ind].transform);
                if (child != null) {
                    child.gameObject.SetActive(false);
                }
            }

            listHatButtons[i].onClick.AddListener(() => {
                SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

                hatIndexSelected = ind;

                skinInfor.text = "+" + hatObjects.listHats[ind].index.ToString() + " Range";
                purchaseText.text = hatObjects.listHats[ind].cost.ToString();

                SetUpBorderImage(listHatButtons[ind].transform);
                SetupPurchaseAndSelectButton(hatObjects.listHats[ind].isLock);
                if (selectButton.IsActive())
                    SetUpSelectAndEquipedButton(type, ind);

                hatObjects.SetCharacterHat(ind, hatHolderTransform);
            });
        }
    }
    void SetUpListPantButton() {
        for (int i = 0; i < listPantButtons.Length; i++) {
            int ind = i;

            if (!pantObjects.listPants[ind].isLock) {
                Transform child = FindChildWithTag(listPantButtons[ind].transform);
                if (child != null) {
                    child.gameObject.SetActive(false);
                }
            }

            listPantButtons[i].onClick.AddListener(() => {
                SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

                pantIndexSelected = ind;

                skinInfor.text = "+" + pantObjects.listPants[ind].index.ToString() + " Range";
                purchaseText.text = pantObjects.listPants[ind].cost.ToString();

                SetUpBorderImage(listPantButtons[ind].transform) ;
                SetupPurchaseAndSelectButton(pantObjects.listPants[ind].isLock);
                if (selectButton.IsActive())
                    SetUpSelectAndEquipedButton(type, ind);

                pantObjects.SetPantMaterial(ind, pantSkin);

            });
        }
    }
    void SetUpListArmorButton() {
        for (int i = 0; i < listArmorButtons.Length; i++) {
            int ind = i;

            if (!armorObjects.listArmor[ind].isLock) {
                Transform child = FindChildWithTag(listArmorButtons[ind].transform);
                if (child != null) {
                    child.gameObject.SetActive(false);
                }
            }

            listArmorButtons[i].onClick.AddListener(() => {
                SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

                armorIndexSelected = ind;

                skinInfor.text = "+" + armorObjects.listArmor[ind].index.ToString() + " Hp";
                purchaseText.text = armorObjects.listArmor[ind].cost.ToString();

                SetUpBorderImage(listArmorButtons[ind].transform) ;
                SetupPurchaseAndSelectButton(armorObjects.listArmor[ind].isLock);

                if (selectButton.IsActive())
                    SetUpSelectAndEquipedButton(type, ind);

                armorObjects.SetCharacterArmor(ind, armorHolderTransform);
            });
        }
    }
    void SetUpListSetButton() {
        for (int i = 0; i < listSetButtons.Length; i++) {
            int ind = i;

            if (!setObjects.listSets[ind].isLock) {
                //Debug.Log("UnLock" + ind);
                Transform child = FindChildWithTag(listSetButtons[ind].transform);
                if (child != null)
                {
                    child.gameObject.SetActive(false);
                }
            }         
            listSetButtons[i].onClick.AddListener(() => {
                SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

                //Debug.Log("Set: " + ind);
                setIndexSelected = ind;

                skinInfor.text = "+" + setObjects.listSets[ind].index.ToString() + " Hp";
                purchaseText.text = setObjects.listSets[ind].cost.ToString();

                SetUpBorderImage(listSetButtons[ind].transform) ;
                SetupPurchaseAndSelectButton(setObjects.listSets[ind].isLock);
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
                SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

                switch (ind) {
                    case 0: {
                            type = TypeSkin.hat;
                            break;
                        }
                    case 1: {
                            type = TypeSkin.pant;
                            break;
                        }
                    case 2: {
                            type = TypeSkin.armor;
                            break;
                        }
                    case 3: {
                            type = TypeSkin.set;
                            break;
                        }
                }
                LoadSkin(type);
                //SetUpBackGroundButtonColor();

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
        SkinData skinData = GetSkinData();
        bool res = false;
        switch (type) {
            case TypeSkin.hat: {
                    if (skinData.hatIndex == ind)
                        res = true;
                    break;
                }
            case TypeSkin.pant: {
                    if (skinData.pantIndex == ind)
                        res = true;
                    break;
                }
            case TypeSkin.armor: {
                    if (skinData.armorIndex == ind)
                        res = true;
                    break;
                }
            case TypeSkin.set: {
                    if (skinData.setIndex == ind)
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
    void SetupPurchaseAndSelectButton(bool isLock) {
        equipedButton.gameObject.SetActive(false);
        if (isLock) {
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
            SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

            LoadSkin(type);
        });
    }
    void LoadSkin(TypeSkin typeSkin) {
        data = GetSkinData();
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

        switch (typeSkin) {
            case TypeSkin.hat: {
                    SetUpBorderImage(listHatButtons[data.hatIndex].transform);
                    SetUpEquippedImage(listHatButtons[data.hatIndex].transform);
                    break;
                }
            case TypeSkin.pant: {
                    SetUpBorderImage(listPantButtons[data.pantIndex].transform);
                    SetUpEquippedImage(listPantButtons[data.pantIndex].transform);
                    break;
                }
            case TypeSkin.armor: {
                    SetUpBorderImage(listArmorButtons[data.armorIndex].transform);
                    SetUpEquippedImage(listArmorButtons[data.armorIndex].transform);
                    break;
                }
            case TypeSkin.set: {
                    SetUpBorderImage(listSetButtons[data.setIndex].transform);
                    SetUpEquippedImage(listSetButtons[data.setIndex].transform);
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
        data = GetSkinData();
        int playerCoin = PlayerPrefs.GetInt("PlayerCoin");
        switch (type) {
            case TypeSkin.hat: {
                    if (playerCoin < hatObjects.listHats[hatIndexSelected].cost)
                        return false;
                    child = FindChildWithTag(listHatButtons[hatIndexSelected].transform);
                    hatObjects.listHats[hatIndexSelected].isLock = false;
                    playerCoin -= hatObjects.listHats[hatIndexSelected].cost;
                    break;
                }
            case TypeSkin.pant: {
                    if (playerCoin < pantObjects.listPants[hatIndexSelected].cost)
                        return false;
                    child = FindChildWithTag(listPantButtons[pantIndexSelected].transform);
                    pantObjects.listPants[pantIndexSelected].isLock = false;
                    playerCoin -= pantObjects.listPants[hatIndexSelected].cost;

                    break;
                }
            case TypeSkin.armor: {
                    if (playerCoin < armorObjects.listArmor[hatIndexSelected].cost)
                        return false;

                    child = FindChildWithTag(listArmorButtons[armorIndexSelected].transform);
                    armorObjects.listArmor[armorIndexSelected].isLock = false;
                    playerCoin -= armorObjects.listArmor[hatIndexSelected].cost;
                    break;
                }
            case TypeSkin.set: {
                    if (playerCoin < setObjects.listSets[hatIndexSelected].cost)
                        return false;

                    child = FindChildWithTag(listSetButtons[setIndexSelected].transform);
                    setObjects.listSets[setIndexSelected].isLock = false;
                    playerCoin -= setObjects.listSets[hatIndexSelected].cost;
                    break;
                }
        }

        if (child != null) {
            child.gameObject.SetActive(false);
        }
        PlayerPrefs.SetInt("PlayerCoin", playerCoin);
        SaveSkinData(data);
        HomePageManager.Instance.SetCoinText();
        return true;
    }

    SkinData GetSkinData() {
        string json = PlayerPrefs.GetString("PlayerSkin");
        SkinData data = JsonUtility.FromJson<SkinData>(json);
        return data;
    }
    void SaveSkinData(SkinData data) {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("PlayerSkin", json);
        Debug.Log("Save Skin: " + data.hatIndex + " " + data.pantIndex + " " + data.armorIndex + " " + data.setIndex);
    }
    private class SkinData {
        public int hatIndex;
        public int pantIndex;
        public int armorIndex;
        public int setIndex;
        public bool isSet;
    }
}
