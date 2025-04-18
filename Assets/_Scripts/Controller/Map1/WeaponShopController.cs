using System.Data.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponShopController : MonoBehaviour
{
    private static WeaponShopController instance;
    public static WeaponShopController Instance { get => instance; }

    [SerializeField] private WeaponObjects weaponObjects;
    [SerializeField] private GameObject weaponDisplay;
    private int weaponIndexSelected;

    [Header("WeaponSkin")]
    [SerializeField] private GameObject[] listWeaponSkins;
    [SerializeField] Button[] listWeaponSkinButtons = new Button[5];
    [SerializeField] private Image[] listBorderImage = new Image[5];
    [SerializeField] private Button nextOptionsButton;
    [SerializeField] private Button previousOptionsButton;
    private int weaponSkinIndexSelected;

    [Header("Button")]
    [SerializeField] private Button selectButton;
    [SerializeField] private Button equipedButton;

    [SerializeField] private Button purchaseWeaponButton;
    [SerializeField] private TextMeshProUGUI purchaseWeaponText;
    [SerializeField] private TextMeshProUGUI weaponName;
    [SerializeField] private Button purchaseWeaponByAdButton;
    [SerializeField] private TextMeshProUGUI purchaseWeaponByAdText;
    [SerializeField] private TextMeshProUGUI weaponAttribute;

    [SerializeField] private Button purchaseSkinButton;
    [SerializeField] private TextMeshProUGUI purchaseSkinText;
    [SerializeField] private Button purchaseSkinByAdButton;

    [Header("Color")]
    [SerializeField] Button[] listColorButtons = new Button[18];
    [SerializeField] Button[] listWeaponPartButtons = new Button[3];
    [SerializeField] Button[] listPartTargetButton = new Button[3];
    private int partSelected;
    int materialCount;

    private WeaponData data;
    //public event EventHandler OnUserChangeWeapon;

    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        nextOptionsButton.onClick.AddListener(() =>{
            SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

            weaponIndexSelected += 1;
            if (weaponIndexSelected >= weaponObjects.listWeapon.Length)
                weaponIndexSelected = 0;
            PlayerPrefs.SetInt("CurWeapon", weaponIndexSelected);
            LoadWeapon();
        });

        previousOptionsButton.onClick.AddListener(() => {
            SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

            weaponIndexSelected -= 1;
            if (weaponIndexSelected < 0)
                weaponIndexSelected = weaponObjects.listWeapon.Length-1;
            PlayerPrefs.SetInt("CurWeapon", weaponIndexSelected);
            LoadWeapon();
        });

        selectButton.onClick.AddListener(SetUpSelectButton);
        purchaseSkinButton.onClick.AddListener(SetUpPurchaseSkinButton);
        purchaseSkinByAdButton.onClick.AddListener(SetUpPurchaseSkinButtonByAd);
        purchaseWeaponButton.onClick.AddListener(SetUpPurchaseWeaponButton);
        purchaseWeaponByAdButton.onClick.AddListener(SetUpPurchaseWeaponByAdButton);

        //Skin
        for(int i=0; i<listWeaponSkinButtons.Length; i++) {
            int ind = i;
            listWeaponSkinButtons[i].onClick.AddListener(() => {
                SelectSkin(ind);
            });
        }
        // Weapon Part
        for(int i=0; i < listWeaponPartButtons.Length; i++) {
            int ind = i;
            listWeaponPartButtons[i].onClick.AddListener(() => {
                partSelected = ind;
                SetUpTargetPartButton();
            });
        }

        //Material
        for(int i=0; i<listColorButtons.Length; i++) {
            int ind = i;
            listColorButtons[i].onClick.AddListener(() => {
                SelectMaterial(ind);
            });
        }
    }

    private void Start() {
        //LoadWeapon();
        MaxManager.Instance.OnPlayerReceiveAward += WeaponShop_OnPlayerReceiveAward;
    }



    public void LoadWeapon() {
        //Lay vu khi hien tai dang duoc chon

        int weaponIndexSaved = PlayerPrefs.GetInt("CurWeapon");
        weaponIndexSelected = weaponIndexSaved;

        materialCount = weaponObjects.GetListMaterials(weaponIndexSaved, 0).Length;

        data = DataManager.Instance.GetWeaponData(weaponIndexSaved);

        //Debug.Log("Start, vu khi: " + weaponIndexSaved + " " + "skin: " +
        //    data.skinIndex + ", " + data.showMaterial());

        // SetUp cac skin cua vu khi
        int skinIndexSaved = data.skinIndex;
        weaponSkinIndexSelected = skinIndexSaved;

        for (int i=0; i< listWeaponSkinButtons.Length; i++) {
            if (i == skinIndexSaved)
                listBorderImage[i].gameObject.SetActive(true);
            else
                listBorderImage[i].gameObject.SetActive(false);
            SetUpSkin(weaponIndexSaved, i);
        }

        //SetUp material  va scale cho WeaponDisplay
        Mesh skinMesh = weaponObjects.GetMeshWeapon(weaponIndexSaved, skinIndexSaved);
        Material[] skinMaterial = weaponObjects.GetListMaterials(weaponIndexSaved, skinIndexSaved);

        SetMeshAndMaterial(weaponDisplay, skinMesh, skinMaterial);

        float scale = 3 * weaponObjects.listWeapon[weaponIndexSelected].scale;
        weaponDisplay.transform.localScale = new Vector3(scale, scale, scale);  

        // SetUp Name va thuoc tinh cua bu khi
        weaponName.text = weaponObjects.listWeapon[weaponIndexSaved].name;
        weaponAttribute.text = "+" + weaponObjects.listWeapon[weaponIndexSaved].index + " " +
            weaponObjects.listWeapon[weaponIndexSaved].attribute;

        //Kiem tra xemvu khi da duoc mua hay chua
        if (data.isLock) {
            purchaseWeaponText.text = weaponObjects.listWeapon[weaponIndexSaved].cost.ToString();
            purchaseWeaponButton.gameObject.SetActive(true);
            purchaseWeaponByAdButton.gameObject.SetActive(true);

            purchaseWeaponByAdText.text = 
                data.adQuantity.ToString() + "/2";

            TurnOffUI();
            return;

        }
        else {
            listWeaponSkinButtons[skinIndexSaved].onClick.Invoke();

            TurnOnUI();
        }

        SetUpLockSkinIcon();

        // Hien thi mau neu skin da luu = 0
        SetUpColorBoard(skinIndexSaved);
        //Debug.Log(PlayerController.Instance.name);
        DataManager.Instance.SaveWeaponData(weaponIndexSelected, data);

    }

    void SetUpLockSkinIcon() {
        for (int i = 0; i < listWeaponSkinButtons.Length; i++) {
            Transform lockIcon = null;
            foreach (Transform child in listWeaponSkinButtons[i].transform) {
                if (child.gameObject.CompareTag("LockImage")) {
                    lockIcon = child; break;
                }

            }
            purchaseSkinText.text = weaponObjects.listWeapon[weaponIndexSelected].weaponSkinCost[i].ToString();
            if (data.isLockSkin[i] == false) {
                if (lockIcon != null)
                    lockIcon.gameObject.SetActive(false);

            }
            else {
                if (lockIcon != null)
                    lockIcon.gameObject.SetActive(true);
                else {
                    Debug.Log("skin " + i + "null");
                }
            }
        }
    }

    void SetUpSkin(int weaponIndex, int skinIndex) {
        
        // Lay scale
        float scale = weaponObjects.listWeapon[weaponIndex].scale;
        listWeaponSkins[skinIndex].gameObject.transform.localScale = new Vector3(scale, scale, scale);

        Mesh skinMesh = weaponObjects.GetMeshWeapon(weaponIndex, skinIndex);
        Material[] skinMaterials =new Material[materialCount];
        if(skinIndex !=0 )
           skinMaterials = weaponObjects.GetListMaterials(weaponIndex, skinIndex);
        else {
            for (int i = 0; i < materialCount; i++) {
                int materialIndex = data.weaponMaterials[i];
                Material savedMaterial = listColorButtons[materialIndex].GetComponent<Image>().material;
                skinMaterials[i] = savedMaterial;
            }
        }
        SetMeshAndMaterial(listWeaponSkins[skinIndex], skinMesh, skinMaterials);
    }

    void SetUpTargetPartButton() {
        for (int j = 0; j < materialCount; j++) {
            if (j == partSelected)
                listPartTargetButton[j].gameObject.SetActive(true);
            else
                listPartTargetButton[j].gameObject.SetActive(false);
        }
    }
    void SetUpColorBoard(int skinIndex) {
        if (skinIndex == 0) {
            Debug.Log(data.showMaterial());

            SetUpTargetPartButton();
            data = DataManager.Instance.GetWeaponData(weaponIndexSelected);
            int[] materialIndex = data.weaponMaterials; 
            for (int i=0; i< 3; i++) {
                if(i < materialCount) {
                    listWeaponPartButtons[i].gameObject.SetActive(true);
                    listWeaponPartButtons[i].gameObject.GetComponent<Image>().color =
                        listColorButtons[materialIndex[i]].GetComponent<Image>().material.color;
                }
                else {
                    listWeaponPartButtons[i].gameObject.SetActive(false);
                }
            }
            foreach (Button btn in listColorButtons) {
                btn.gameObject.SetActive(true);
            }
        }
        else {
            foreach (Button btn in listWeaponPartButtons) {
                btn.gameObject.SetActive(false);
            }
            foreach (Button btn in listColorButtons) {
                btn.gameObject.SetActive(false);
            }
        }
    }
    void SetMeshAndMaterial(GameObject obj, Mesh mesh, Material[] materials) {
        obj.GetComponent<MeshFilter>().mesh = mesh;
        obj.GetComponent<MeshRenderer>().sharedMaterials = materials;
    }

    void SelectSkin(int skinIndex) {
        weaponSkinIndexSelected = skinIndex;

        SetUpColorBoard(skinIndex);

        for(int i=0; i<listBorderImage.Length; i++) {
            if (skinIndex == i)
                listBorderImage[i].gameObject.SetActive(true);
            else
                listBorderImage[i].gameObject.SetActive(false);
        }

        //Lay mesh va material cua skin
        Mesh skinMesh = listWeaponSkins[skinIndex].GetComponent<MeshFilter>().mesh;
        Material[] skinMaterials = listWeaponSkins[skinIndex].GetComponent<MeshRenderer>().sharedMaterials;
        SetMeshAndMaterial(weaponDisplay, skinMesh, skinMaterials);

        // Doi transform cua Select Button
        SetPurchaseSkinAndSelect(skinIndex);
        if (selectButton.IsActive()) {
            SetSelectAndEquipButton(skinIndex);
        }

        if (skinIndex != 0) {
            selectButton.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, -500, -2);
            equipedButton.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, -500, -2);
        }
        else {
            selectButton.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, -700, -2);
            equipedButton.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, -700, -2);
        }

    }

    void SelectMaterial(int materialIndex) {
        Material materialSelected = listColorButtons[materialIndex].GetComponent<Image>().material;

        SetWeaponDisplay(partSelected, materialSelected);

        Mesh skinMesh = listWeaponSkins[0].GetComponent<MeshFilter>().mesh;
        Material[] skinMaterials = listWeaponSkins[0].GetComponent <MeshRenderer>().sharedMaterials;
        skinMaterials[partSelected] = materialSelected;
        SetMeshAndMaterial(listWeaponSkins[0], skinMesh, skinMaterials);

        //weaponObjects.SetWeaponPartMaterial(weaponIndexSelected,
        //    0, partSelected, materialSelected);

        // Luu Data
        data = DataManager.Instance.GetWeaponData(weaponIndexSelected);
        data.weaponMaterials[partSelected] = materialIndex;

        DataManager.Instance.SaveWeaponData(weaponIndexSelected, data);

    }

    void SetWeaponDisplay(int weaponPart, Material material) {
        // hien thi mau tren phan duoc chon
        MeshRenderer renderer = weaponDisplay.GetComponent<MeshRenderer>();
        Material[] weaponDisplayMaterials = renderer.sharedMaterials;
        weaponDisplayMaterials[weaponPart] = material;
        renderer.sharedMaterials = weaponDisplayMaterials;

        //hien thi mau tren button duoc chon
        listWeaponPartButtons[weaponPart].GetComponent<Image>().color = material.color;

    }

    void SetPurchaseSkinAndSelect(int skinIndex) {
        equipedButton.gameObject.SetActive(false);

        if (data.isLockSkin[skinIndex]) {
            purchaseSkinButton.gameObject.SetActive(true);
            purchaseSkinText.text = weaponObjects.listWeapon[weaponIndexSelected].weaponSkinCost[skinIndex].ToString();
            purchaseSkinByAdButton.gameObject.SetActive(true);
            selectButton.gameObject.SetActive(false);
        }
        else {
            purchaseSkinButton.gameObject.SetActive(false);
            purchaseSkinByAdButton.gameObject.SetActive(false);
            selectButton.gameObject.SetActive(true);
        }
    }
    void SetSelectAndEquipButton(int skinIndex) {
        //data = WeaponDataManager.Instance.GetWeaponData(weaponIndexSelected);
        //Debug.Log(skinIndex + " " + data.skinIndex);
        if (data.skinIndex == skinIndex) {
            equipedButton.gameObject.SetActive(true);
            selectButton.gameObject.SetActive(false);
        }
        else {
            selectButton.gameObject.SetActive(true);
            equipedButton.gameObject.SetActive(false);
        }
    }

    #region ----------PURCHASE_WEAPON-----------
    void SetUpPurchaseWeaponButton() {
        SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

        int playerCoin = PlayerPrefs.GetInt("PlayerCoin");
        if (playerCoin > weaponObjects.listWeapon[weaponIndexSelected].cost)
            playerCoin -= weaponObjects.listWeapon[weaponIndexSelected].cost;
        else {
            Debug.Log("Thieu tien roi b ei");
            return;
        }
        PlayerPrefs.SetInt("PlayerCoin", playerCoin);
        HomePageController.Instance.SetCoinText();

        purchaseWeaponButton.gameObject.SetActive(false);
        purchaseWeaponByAdButton.gameObject.SetActive(false);
        data.isLock = false;

        equipedButton.gameObject.SetActive(true);
        equipedButton.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, -500, -2);

        DataManager.Instance.SaveWeaponData(weaponIndexSelected, data);
        TurnOnUI();
        SetUpLockSkinIcon();

    }

    void SetUpPurchaseWeaponByAdButton() {
        if(MaxManager.Instance != null) {
            MaxManager.Instance.ShowRewardAd();
            MaxManager.Instance.SetTypeReward(MaxManager.TypeReward.weapon);
        }

    }

    void HandlerPurchaseWeaponByAd() {
        data.adQuantity += 1;
        purchaseWeaponByAdText.text = data.adQuantity.ToString() + "/2";

        if (data.adQuantity == 2) {
            purchaseWeaponButton.gameObject.SetActive(false);
            purchaseWeaponByAdButton.gameObject.SetActive(false);
            purchaseSkinByAdButton.gameObject.SetActive(false);

            data.isLock = false;

            
            equipedButton.gameObject.SetActive(true);
            equipedButton.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, -500, -2);

            DataManager.Instance.SaveWeaponData(weaponIndexSelected, data);
            TurnOnUI();
        }
        SetUpLockSkinIcon();


    }
    #endregion

    #region PurchaseWeaponSkin
    void SetUpPurchaseSkinButton() {
        SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);


        if (!CoinManager.Instance.PurchaseItem(weaponObjects.listWeapon[weaponIndexSelected].weaponSkinCost[weaponSkinIndexSelected])) {
            Debug.Log("ko du tien");
            return;
        }
         
        HomePageController.Instance.SetCoinText();

        //weaponObjects.listWeapon[weaponIndexSelected].weaponSkinCost[weaponSkinIndexSelected] = 0;
        data.isLockSkin[weaponSkinIndexSelected] = false;

        purchaseSkinButton.gameObject.SetActive(false);
        purchaseSkinByAdButton.gameObject.SetActive(false);
        equipedButton.gameObject.SetActive(true);

        Transform lockIcon = null;
        foreach(Transform child in listWeaponSkinButtons[weaponSkinIndexSelected].transform) {
            if (child.gameObject.CompareTag("LockImage")) {
                lockIcon = child; break;
            }
        }
        if(lockIcon != null) 
            lockIcon.gameObject.SetActive(false);

        //data = WeaponDataManager.Instance.GetWeaponData(weaponIndexSelected);
        data.skinIndex = weaponSkinIndexSelected;
        DataManager.Instance.SaveWeaponData(weaponIndexSelected, data);
    }
    void SetUpPurchaseSkinButtonByAd() {
        if(MaxManager.Instance != null) {
            MaxManager.Instance.ShowRewardAd();
            MaxManager.Instance.SetTypeReward(MaxManager.TypeReward.weaponSkin);
        }

    }
    void HandlerPurchaseSkinByAd() {
        data.isLockSkin[weaponSkinIndexSelected] = false;

        purchaseSkinButton.gameObject.SetActive(false);
        purchaseSkinByAdButton.gameObject.SetActive(false);
        equipedButton.gameObject.SetActive(true);

        Transform lockIcon = null;
        foreach (Transform child in listWeaponSkinButtons[weaponSkinIndexSelected].transform) {
            if (child.gameObject.CompareTag("LockImage")) {
                lockIcon = child; break;
            }

        }
        if (lockIcon != null)
            lockIcon.gameObject.SetActive(false);

        //data = WeaponDataManager.Instance.GetWeaponData(weaponIndexSelected);
        data.skinIndex = weaponSkinIndexSelected;
        DataManager.Instance.SaveWeaponData(weaponIndexSelected, data);
    }

    #endregion
    void SetUpSelectButton() {
        SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

        data = DataManager.Instance.GetWeaponData(weaponIndexSelected);
        data.skinIndex = weaponSkinIndexSelected;
        DataManager.Instance.SaveWeaponData(weaponIndexSelected, data);
        selectButton.gameObject.SetActive(false);
        equipedButton.gameObject.SetActive(true);
    }

    void TurnOnUI() {
        purchaseWeaponButton.gameObject.SetActive(false);
        purchaseWeaponByAdButton.gameObject.SetActive(false);
        foreach (Button btn in listWeaponSkinButtons)
            btn.gameObject.SetActive(true);
        //foreach (Button btn in listWeaponPartButtons)
        //    btn.gameObject.SetActive(true);
        //foreach (Button btn in listColorButtons)
        //    btn.gameObject.SetActive(true);
    }

    void TurnOffUI() {
        purchaseSkinButton.gameObject.SetActive(false);
        selectButton.gameObject.SetActive(false);
        equipedButton.gameObject.SetActive(false);
        foreach (Button btn in listWeaponSkinButtons)
            btn.gameObject.SetActive(false);
        foreach (Button btn in listWeaponPartButtons)
            btn.gameObject.SetActive(false);
        foreach (Button btn in listColorButtons)
            btn.gameObject.SetActive(false);
    }

    private void WeaponShop_OnPlayerReceiveAward(object sender, MaxManager.TypeReward t) {
        if(t == MaxManager.TypeReward.weapon) {
            HandlerPurchaseWeaponByAd();
        }
        if(t == MaxManager.TypeReward.weaponSkin) {
            HandlerPurchaseSkinByAd();

        }

    }
}
