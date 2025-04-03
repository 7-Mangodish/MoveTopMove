using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponShopManager : MonoBehaviour
{
    private static WeaponShopManager instance;
    public static WeaponShopManager Instance { get => instance; }

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
    [SerializeField] private Button useOneTimeButton;
    [SerializeField] private TextMeshProUGUI weaponAttribute;

    [SerializeField] private Button purchaseSkinButton;
    [SerializeField] private TextMeshProUGUI purchaseSkinText;

    [Header("Color")]
    [SerializeField] Button[] listColorButtons = new Button[18];
    [SerializeField] Button[] listWeaponPartButtons = new Button[3];
    [SerializeField] Button[] listPartTargetButton = new Button[3];
    private int partSelected;
    int materialCount;

    private WeaponDataManager.WeaponData data;

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
            SetStart();
        });

        previousOptionsButton.onClick.AddListener(() => {
            SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

            weaponIndexSelected -= 1;
            if (weaponIndexSelected < 0)
                weaponIndexSelected = weaponObjects.listWeapon.Length-1;
            PlayerPrefs.SetInt("CurWeapon", weaponIndexSelected);
            SetStart();
        });

        selectButton.onClick.AddListener(SetUpSelectButton);
        purchaseSkinButton.onClick.AddListener(SetUpPurchaseSkinButton);
        purchaseWeaponButton.onClick.AddListener(SetUpPurchaseWeaponButton);

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
        SetStart();
    }

    void SetStart() {
        //Lay vu khi hien tai dang duoc chon
        int weaponIndexSaved = PlayerPrefs.GetInt("CurWeapon");
        weaponIndexSelected = weaponIndexSaved;

        // Them moi vao pref neu chua co
        if (!PlayerPrefs.HasKey(weaponIndexSelected.ToString())) {
            WeaponDataManager.WeaponData newData = new WeaponDataManager.WeaponData {
                skinIndex = 0,
            };
            WeaponDataManager.Instance.SaveWeaponData(weaponIndexSelected, newData);
        }

        data = WeaponDataManager.Instance.GetWeaponData(weaponIndexSaved);
        Debug.Log("Start, vu khi: " + weaponIndexSaved + " " + "skin: " +
            data.skinIndex + ", " + data.showMaterial());

        // SetUp cac skin cua vu khi
        int skinIndexSaved = data.skinIndex;
        weaponSkinIndexSelected = skinIndexSaved;
        for(int i=0; i< listWeaponSkinButtons.Length; i++) {
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
        if (weaponObjects.listWeapon[weaponIndexSaved].isLock) {
            purchaseWeaponText.text = weaponObjects.listWeapon[weaponIndexSaved].cost.ToString();
            purchaseWeaponButton.gameObject.SetActive(true);
            useOneTimeButton.gameObject.SetActive(true);
            TurnOffUI();
            return;

        }
        else {
            listWeaponSkinButtons[skinIndexSaved].onClick.Invoke();
            TurnOnUI();
        }

        for(int i=0; i < listWeaponSkinButtons.Length; i++) {
            Transform lockIcon = null;
            foreach (Transform child in listWeaponSkinButtons[i].transform) {
                if (child.gameObject.CompareTag("LockImage")) {
                    lockIcon = child; break;
                }

            }
            purchaseSkinText.text = weaponObjects.listWeapon[weaponIndexSelected].weaponSkinCost[i].ToString();
            //Debug.Log(weaponObjects.listWeapon[weaponIndexSelected].weaponSkinCost[i]);
            if (weaponObjects.listWeapon[weaponIndexSelected].weaponSkinCost[i] == 0) {
                if(lockIcon != null)
                    lockIcon.gameObject.SetActive(false);

            }
            else {
                if(lockIcon != null)
                    lockIcon.gameObject.SetActive(true);
            }
        }

        // Sua mau cua cac nut chinh mau
        Material[] skinMaterials = listWeaponSkins[0].GetComponent<MeshRenderer>().sharedMaterials;
        materialCount = skinMaterials.Length;
        SetUpTargetPartButton();
        for (int i=0; i<3; i++) {
            if (i < materialCount) {
                listWeaponPartButtons[i].gameObject.SetActive(true);
                listWeaponPartButtons[i].GetComponent<Image>().color = skinMaterials[i].color;

            }
            else
                listWeaponPartButtons[i].gameObject.SetActive(false);
        }

        // Hien thi mau neu skin duoc chon = 0
        SetUpColorBoard(skinIndexSaved, materialCount);

    }

    void SetUpSkin(int weaponIndex, int skinIndex) {
        
        // Lay scale
        float scale = weaponObjects.listWeapon[weaponIndex].scale;
        listWeaponSkins[skinIndex].gameObject.transform.localScale = new Vector3(scale, scale, scale);

        // Lay mesh va material cua skin
        Mesh skinMesh = weaponObjects.GetMeshWeapon(weaponIndex, skinIndex);
        Material[] skinMaterials = weaponObjects.GetListMaterials(weaponIndex, skinIndex);
        SetMeshAndMaterial(listWeaponSkins[skinIndex],skinMesh ,skinMaterials);
    }

    void SetUpTargetPartButton() {
        for (int j = 0; j < materialCount; j++) {
            if (j == partSelected)
                listPartTargetButton[j].gameObject.SetActive(true);
            else
                listPartTargetButton[j].gameObject.SetActive(false);
        }
    }
    void SetUpColorBoard(int skinIndex, int materialCount) {
        if (skinIndex == 0) {
            for(int i=0; i< materialCount; i++) {
                listWeaponPartButtons[i].gameObject.SetActive(true);
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

        SetUpColorBoard(skinIndex, materialCount);

        for(int i=0; i<listBorderImage.Length; i++) {
            if (skinIndex == i)
                listBorderImage[i].gameObject.SetActive(true);
            else
                listBorderImage[i].gameObject.SetActive(false);
        }
        //Debug.Log("Skin " + skinIndex);
        //Lay mesh va material cua skin
        Mesh skinMesh = listWeaponSkins[skinIndex].GetComponent<MeshFilter>().mesh;
        Material[] skinMaterials = listWeaponSkins[skinIndex].GetComponent<MeshRenderer>().sharedMaterials;
        SetMeshAndMaterial(weaponDisplay, skinMesh, skinMaterials);

        // Doi transform cua Select Button
        SetPurchaseSkinAndSelect(skinIndex);
        if (selectButton.IsActive())
            SetSelectAndEquipButton(skinIndex);

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

        weaponObjects.SetWeaponPartMaterial(weaponIndexSelected,
            0, partSelected, materialSelected);

        // Luu Data
        if (!PlayerPrefs.HasKey(weaponIndexSelected.ToString())){
            WeaponDataManager.WeaponData dataTemp = new WeaponDataManager.WeaponData();
            string jsonTemp = JsonUtility.ToJson(dataTemp);
            PlayerPrefs.SetString(weaponIndexSelected.ToString(), jsonTemp);
        }
        data = WeaponDataManager.Instance.GetWeaponData(weaponIndexSelected);
        
        data.weaponMaterials[partSelected] = materialIndex;

        WeaponDataManager.Instance.SaveWeaponData(weaponSkinIndexSelected, data);

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

        if (weaponObjects.listWeapon[weaponIndexSelected].weaponSkinCost[skinIndex] > 0) {
            purchaseSkinButton.gameObject.SetActive(true);
            purchaseSkinText.text = weaponObjects.listWeapon[weaponIndexSelected].weaponSkinCost[skinIndex].ToString();
            selectButton.gameObject.SetActive(false);
        }
        else {
            purchaseSkinButton.gameObject.SetActive(false); 
            selectButton.gameObject.SetActive(true);
        }
    }
    void SetSelectAndEquipButton(int skinIndex) {
        data = WeaponDataManager.Instance.GetWeaponData(weaponIndexSelected);
        if (data.skinIndex == skinIndex) {
            equipedButton.gameObject.SetActive(true);
            selectButton.gameObject.SetActive(false);
        }
        else {
            selectButton.gameObject.SetActive(true);
            equipedButton.gameObject.SetActive(false);
        }
    }

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
        HomePageManager.Instance.SetCoinText();

        purchaseWeaponButton.gameObject.SetActive(false);
        useOneTimeButton.gameObject.SetActive(false);
        weaponObjects.listWeapon[weaponIndexSelected].isLock = false;
        TurnOnUI();


    }
    void SetUpPurchaseSkinButton() {
        SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

        int playerCoin = PlayerPrefs.GetInt("PlayerCoin");
        if (playerCoin > weaponObjects.listWeapon[weaponIndexSelected].weaponSkinCost[weaponSkinIndexSelected])
            playerCoin -= weaponObjects.listWeapon[weaponIndexSelected].weaponSkinCost[weaponSkinIndexSelected];
        else {
            Debug.Log("Khong du tien");
            return;
        }
        PlayerPrefs.SetInt("PlayerCoin", playerCoin);
        HomePageManager.Instance.SetCoinText();

        weaponObjects.listWeapon[weaponIndexSelected].weaponSkinCost[weaponSkinIndexSelected] = 0;

        purchaseSkinButton.gameObject.SetActive(false);
        equipedButton.gameObject.SetActive(true);
        Transform lockIcon = null;
        foreach(Transform child in listWeaponSkinButtons[weaponSkinIndexSelected].transform) {
            if (child.gameObject.CompareTag("LockImage")) {
                lockIcon = child; break;
            }

        }
        if(lockIcon != null) 
            lockIcon.gameObject.SetActive(false);

        data = WeaponDataManager.Instance.GetWeaponData(weaponIndexSelected);
        data.skinIndex = weaponSkinIndexSelected;
        WeaponDataManager.Instance.SaveWeaponData(weaponIndexSelected, data);
    }
    void SetUpSelectButton() {
        SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

        data = WeaponDataManager.Instance.GetWeaponData(weaponIndexSelected);
        data.skinIndex = weaponSkinIndexSelected;
        WeaponDataManager.Instance.SaveWeaponData(weaponIndexSelected, data);
        selectButton.gameObject.SetActive(false);
        equipedButton.gameObject.SetActive(true);
    }

    void TurnOnUI() {
        purchaseWeaponButton.gameObject.SetActive(false);
        useOneTimeButton.gameObject.SetActive(false);
        foreach (Button btn in listWeaponSkinButtons)
            btn.gameObject.SetActive(true);
        foreach (Button btn in listWeaponPartButtons)
            btn.gameObject.SetActive(true);
        foreach (Button btn in listColorButtons)
            btn.gameObject.SetActive(true);
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

}
