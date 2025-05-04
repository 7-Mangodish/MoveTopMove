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

    private WeaponData weaponData;
    private Weapon weapon;
    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        nextOptionsButton.onClick.AddListener(() =>{
            AudioManager.Instance.PlaySoundClickButton();

            weaponIndexSelected += 1;
            if (weaponIndexSelected >= weaponObjects.listWeapon.Length)
                weaponIndexSelected = 0;
            int id = weaponObjects.listWeapon[weaponIndexSelected].id;
            Debug.Log("idWeapon: " + id);
            LoadWeapon(id);
        });

        previousOptionsButton.onClick.AddListener(() => {
            AudioManager.Instance.PlaySoundClickButton();

            weaponIndexSelected -= 1;
            if (weaponIndexSelected < 0)
                weaponIndexSelected = weaponObjects.listWeapon.Length-1;
            //DataManager.Instance.playerPersonalData.currentWeaponIndex = weaponIndexSelected;
            //Debug.Log(DataManager.Instance.playerPersonalData.currentWeaponIndex);
            int id = weaponObjects.listWeapon[weaponIndexSelected].id;
            LoadWeapon(id);   
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

    private void OnEnable() {
        MaxManager.Instance.OnPlayerReceiveAward += WeaponShop_OnPlayerReceiveAward;
    }

    private void OnDisable() {
        if (MaxManager.Instance != null)
            MaxManager.Instance.OnPlayerReceiveAward -= WeaponShop_OnPlayerReceiveAward;
        else
            Debug.LogWarning("Max is: null");

    }

    public void InitWeaponShopData() {
        int weaponId = DataManager.Instance.playerData.currentWeaponId;
        int ind = 0;
        for(int i=0; i<weaponObjects.listWeapon.Length; i++) {
            if (weaponObjects.listWeapon[i].id == weaponId) {
                ind = i; break;
            }
        }
        weaponIndexSelected = ind;
        LoadWeapon(weaponId);
    }

    // Duoc goi khi moi mo shop, hoac doi vu khi
    public void LoadWeapon(int weaponId) {
        //Load Data cua weapon co Id la weaponId
        weaponData = DataManager.Instance.GetWeaponData(weaponId);
        weapon = weaponObjects.GetWeaponById(weaponData.weaponId);
        materialCount = weaponObjects.GetListMaterials(weaponData.weaponId, 0).Length;
        // SetUp cac skin cua vu khi
        weaponSkinIndexSelected = weaponData.skinIndex;
        for (int i=0; i< listWeaponSkinButtons.Length; i++) {
            if (i == weaponData.skinIndex)
                listBorderImage[i].gameObject.SetActive(true);
            else
                listBorderImage[i].gameObject.SetActive(false);
            SetUpSkin(weaponData.weaponId, i);
        }

        //SetUp material  va scale cho WeaponDisplay
        Mesh skinMesh = weaponObjects.GetMeshWeapon(weaponData.weaponId, weaponData.skinIndex);
        Material[] skinMaterial = weaponObjects.GetListMaterials(weaponData.weaponId, weaponData.skinIndex);

        SetMeshAndMaterial(weaponDisplay, skinMesh, skinMaterial);
        float scale = 3 * weapon.scale;
        weaponDisplay.transform.localScale = new Vector3(scale, scale, scale);  

        // SetUp Name va thuoc tinh cua bu khi
        weaponName.text = weapon.name;
        weaponAttribute.text = "+" + weapon.index + " " +
            weapon.attribute;

        //Kiem tra xemvu khi da duoc mua hay chua
        if (weaponData.isLock) {
            purchaseWeaponText.text = weapon.cost.ToString();
            purchaseWeaponButton.gameObject.SetActive(true);
            purchaseWeaponByAdButton.gameObject.SetActive(true);

            purchaseWeaponByAdText.text =
                weaponData.adQuantity.ToString() + "/2";

            TurnOffUI();
            return;

        }
        else {
            listWeaponSkinButtons[weaponData.skinIndex].onClick.Invoke();
            TurnOnUI();
        }
        SetUpLockSkinIcon();
        SetSelectAndEquipButton(weaponData.skinIndex);
        // Hien thi mau neu skin da luu = 0
        SetUpColorBoard(weaponData.skinIndex);
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
            if (DataManager.Instance.playerData.listWeaponData[weaponIndexSelected].isLockSkin[i] == false) {
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

    // SetUp skin cua vu khi
    void SetUpSkin(int weaponId, int skinIndex) {
        // Lay scale
        float scale = weapon.scale;
        listWeaponSkins[skinIndex].gameObject.transform.localScale = new Vector3(scale, scale, scale);
        // Set Up Skin Mesh
        Mesh skinMesh = weaponObjects.GetMeshWeapon(weaponId, skinIndex);
        Material[] skinMaterials =new Material[materialCount];
        if(skinIndex !=0 )
           skinMaterials = weaponObjects.GetListMaterials(weaponId, skinIndex);
        else {
            for (int i = 0; i < materialCount; i++) {
                int materialIndex = weaponData.weaponMaterials[i];
                //int materialIndex = DataManager.Instance.playerData.listWeaponData[weaponIndexSelected].weaponMaterials[i];
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

    // Set mau cho cac nut chon mau
    void SetUpColorBoard(int skinIndex) {
        if (skinIndex == 0) {
            SetUpTargetPartButton();

            //data = DataManager.Instance.GetWeaponData(weaponIndexSelected);
            int[] materialIndex = weaponData.weaponMaterials; 
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

    //Set Mesh va Material cho gameObject
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
        //data = DataManager.Instance.GetWeaponData(weaponIndexSelected);
        weaponData.weaponMaterials[partSelected] = materialIndex;

        //DataManager.Instance.SaveWeaponData(weaponIndexSelected, data);

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

        if (DataManager.Instance.playerData.listWeaponData[weaponIndexSelected].isLockSkin[skinIndex]) {
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
        if (weaponData.skinIndex == skinIndex) {
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
        AudioManager.Instance.PlaySoundClickButton();

        int playerCoin = PlayerPrefs.GetInt(GameVariable.PLAYER_COIN);
        if (playerCoin > weapon.cost)
            playerCoin -= weapon.cost;
        else {
            Debug.Log("Thieu tien roi b ei");
            return;
        }
        PlayerPrefs.SetInt(GameVariable.PLAYER_COIN, playerCoin);
        HomePageController.Instance.SetCoinText();

        purchaseWeaponButton.gameObject.SetActive(false);
        purchaseWeaponByAdButton.gameObject.SetActive(false);
        //
        weaponData.isLock = false;
        DataManager.Instance.playerData.currentWeaponId = weapon.id;
        //
        equipedButton.gameObject.SetActive(true);
        equipedButton.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, -500, -2);

        if (weapon.isBoom)
            weaponData.isBoom = true;
        
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
        weaponData.adQuantity += 1;
        purchaseWeaponByAdText.text = weaponData.adQuantity.ToString() + "/2";

        if (weaponData.adQuantity == 2) {
            purchaseWeaponButton.gameObject.SetActive(false);
            purchaseWeaponByAdButton.gameObject.SetActive(false);
            purchaseSkinByAdButton.gameObject.SetActive(false);

            weaponData.isLock = false;
            DataManager.Instance.playerData.currentWeaponId = weapon.id;

            equipedButton.gameObject.SetActive(true);
            equipedButton.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, -500, -2);

            
            TurnOnUI();
        }
        SetUpLockSkinIcon();


    }
    #endregion

    #region PurchaseWeaponSkin
    void SetUpPurchaseSkinButton() {
        AudioManager.Instance.PlaySoundClickButton();
        if (!CoinManager.Instance.PurchaseItem(weapon.weaponSkinCost[weaponSkinIndexSelected])) {
            Debug.Log("ko du tien");
            return;
        }         
        HomePageController.Instance.SetCoinText();
        //
        weaponData.isLockSkin[weaponSkinIndexSelected] = false;
        //
        purchaseSkinButton.gameObject.SetActive(false);
        purchaseSkinByAdButton.gameObject.SetActive(false);
        equipedButton.gameObject.SetActive(true);
        //
        Transform lockIcon = null;
        foreach(Transform child in listWeaponSkinButtons[weaponSkinIndexSelected].transform) {
            if (child.gameObject.CompareTag("LockImage")) {
                lockIcon = child; break;
            }
        }
        if(lockIcon != null) 
            lockIcon.gameObject.SetActive(false);
        //
        weaponData.skinIndex = weaponSkinIndexSelected;
        
    }
    void SetUpPurchaseSkinButtonByAd() {
        if(MaxManager.Instance != null) {
            MaxManager.Instance.ShowRewardAd();
            MaxManager.Instance.SetTypeReward(MaxManager.TypeReward.weaponSkin);
        }

    }
    void HandlerPurchaseSkinByAd() {
        weaponData.isLockSkin[weaponSkinIndexSelected] = false;
        //
        purchaseSkinButton.gameObject.SetActive(false);
        purchaseSkinByAdButton.gameObject.SetActive(false);
        equipedButton.gameObject.SetActive(true);
        //
        Transform lockIcon = null;
        foreach (Transform child in listWeaponSkinButtons[weaponSkinIndexSelected].transform) {
            if (child.gameObject.CompareTag("LockImage")) {
                lockIcon = child; break;
            }

        }
        if (lockIcon != null)
            lockIcon.gameObject.SetActive(false);
        //
        weaponData.skinIndex = weaponSkinIndexSelected;
        
    }

    #endregion
    void SetUpSelectButton() {
        AudioManager.Instance.PlaySoundClickButton();
        //
        DataManager.Instance.playerData.currentWeaponId = weapon.id;
        weaponData.skinIndex = weaponSkinIndexSelected;
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
        FirebaseManager.Instance.HandlerClickAdEvent(FirebaseManager.TypeEvent.clickWeaponShopAd, FirebaseManager.TypeAd.reward);

    }
}
