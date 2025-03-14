using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class WeaponShopManager : MonoBehaviour
{
    private static WeaponShopManager instance;
    public static WeaponShopManager Instance { get { return instance; } }

    [SerializeField] private WeaponObjects weaponObjects;
    [SerializeField] private GameObject weaponDisplay;
    private int weaponIndexSelected;

    [Header("WeaponSkin")]
    [SerializeField] private GameObject[] listWeaponSkins;

    [Header("UI")]
    [SerializeField] Button[] listWeaponSkinButtons = new Button[5];
    [SerializeField] private Image[] listBorderImage = new Image[5];
    [SerializeField] private Button nextOptionsButton;
    [SerializeField] private Button previousOptionsButton;

    [SerializeField] private Button normalSelectButton;
    [SerializeField] private TextMeshProUGUI normalText;

    [SerializeField] private Button customSelectButton;
    [SerializeField] private TextMeshProUGUI customText;
    private int weaponSkinIndexSelected;
    private int weaponSkinIndexSelecting;

    [Header("Color")]
    [SerializeField] Button[] listColorButtons = new Button[18];
    [SerializeField] Button[] listWeaponPartButtons = new Button[3];
    private int materialIndexSelected;
    private int partSelected;
    int materitalCount;
    private Dictionary<int, SaveData>  weaponData = new Dictionary<int, SaveData>();
    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        nextOptionsButton.onClick.AddListener(() =>{
            weaponIndexSelected += 1;
            if (weaponIndexSelected >= weaponObjects.listWeapon.Length)
                weaponIndexSelected = 0;
            PlayerPrefs.SetInt("CurWeapon", weaponIndexSelected);
            SetStart();
        });

        previousOptionsButton.onClick.AddListener(() => {
            weaponIndexSelected -= 1;
            if (weaponIndexSelected < 0)
                weaponIndexSelected = weaponObjects.listWeapon.Length-1;
            PlayerPrefs.SetInt("CurWeapon", weaponIndexSelected);
            SetStart();
        });

        normalSelectButton.onClick.AddListener(() => {
            SetSelectButton();
        });

        customSelectButton.onClick.AddListener(() => {
            SetSelectButton();
        });

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
                SelectPart(ind);
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
        //Lay vu khi hien tai dang su dung
        int weaponIndexSaved = PlayerPrefs.GetInt("CurWeapon");

        SaveData data = GetWeaponData(weaponIndexSaved);
        Debug.Log("Start, vu khi: " + weaponIndexSaved + " " + "skin: " +
            data.skinIndex + ", " + data.showMaterial());

        // SetUp cac skin cua vu khi
        int skinIndexSaved = data.skinIndex;
        weaponSkinIndexSelected = skinIndexSaved;
        SetUpSkin(weaponIndexSaved, skinIndexSaved);

        //SetUp material cho WeaponDisplay
        Mesh skinMesh = listWeaponSkins[skinIndexSaved].GetComponent<MeshFilter>().sharedMesh;
        Material[] skinMaterial = listWeaponSkins[skinIndexSaved].GetComponent<MeshRenderer>().sharedMaterials;
        SetMeshAndMaterial(weaponDisplay,skinMesh, skinMaterial);

        // Sua mau cua cac nut chinh mau
        Material[] skinMaterials = listWeaponSkins[0].GetComponent<MeshRenderer>().sharedMaterials;
        materitalCount = skinMaterials.Length;
        if (materitalCount == 2) {
            Debug.Log("Lenght: " + 2);
            Debug.Log(listWeaponPartButtons[2].name);
            Debug.Log(listWeaponPartButtons[2].IsActive());
            listWeaponPartButtons[2].gameObject.SetActive(false);
            Debug.Log(listWeaponPartButtons[2].IsActive());

        }
        for (int i=0; i<skinMaterials.Length; i++) {
            listWeaponPartButtons[i].GetComponent<Image>().color = skinMaterials[i].color;
        }

        // Hien thi mau neu skin duoc chon = 0

        SetUpColorBoard(skinIndexSaved, materitalCount);

    }

    void SetUpSkin(int weaponIndex, int skinIndex) {
        for(int i=0; i< listWeaponSkinButtons.Length; i++) {
            int ind = i;
            //Lay mesh va material cua skin
            Mesh skinMesh = weaponObjects.GetMeshWeapon(weaponIndex, ind);
            Material[] skinMaterials = weaponObjects.GetListMaterials(weaponIndex, ind);

            if (ind == skinIndex)
                listBorderImage[ind].gameObject.SetActive(true);
            else
                listBorderImage[ind].gameObject.SetActive(false);
            SetMeshAndMaterial(listWeaponSkins[ind], skinMesh, skinMaterials);
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
            normalSelectButton.gameObject.SetActive(false);
            customSelectButton.gameObject.SetActive(true);
        }
        else {
            foreach (Button btn in listWeaponPartButtons) {
                btn.gameObject.SetActive(false);
            }
            foreach (Button btn in listColorButtons) {
                btn.gameObject.SetActive(false);
            }
            normalSelectButton.gameObject.SetActive(true);
            customSelectButton.gameObject.SetActive(false);
        }
        if (skinIndex == weaponSkinIndexSelected) {
            normalText.text = "Equipped";
            customText.text = "Euipped";
        }
        else {
            normalText.text = "Select";
            customText.text = "Select";
        }
    }
    void SetMeshAndMaterial(GameObject obj, Mesh mesh, Material[] materials) {
        obj.GetComponent<MeshFilter>().mesh = mesh;
        obj.GetComponent<MeshRenderer>().sharedMaterials = materials;
    }

    void SetSelectButton() {
        if (weaponSkinIndexSelected != weaponSkinIndexSelecting) {
            normalText.text = "Equipped";
            customText.text = "Equipped";

            weaponSkinIndexSelected = weaponSkinIndexSelecting;
            SaveData saveData = GetWeaponData(weaponIndexSelected);
            saveData.skinIndex = weaponSkinIndexSelected;
            SaveWeaponData(weaponIndexSelected, saveData);
        }
        else {
            Debug.Log("Equipped");
        }
    }
    void SelectSkin(int skinIndex) {
        SetUpColorBoard(skinIndex, materitalCount);

        for(int i=0; i<listBorderImage.Length; i++) {
            if (skinIndex == i)
                listBorderImage[i].gameObject.SetActive(true);
            else
                listBorderImage[i].gameObject.SetActive(false);

        }

        weaponSkinIndexSelecting = skinIndex;
        //Lay mesh va material cua skin
        Mesh skinMesh = listWeaponSkins[skinIndex].GetComponent<MeshFilter>().mesh;
        Material[] skinMaterials = listWeaponSkins[skinIndex].GetComponent<MeshRenderer>().sharedMaterials;
        SetMeshAndMaterial(weaponDisplay, skinMesh, skinMaterials);
    }

    void SelectMaterial(int materialIndex) {
        Material materialSelected = listColorButtons[materialIndex].GetComponent<Image>().material;

        SetWeaponDisplay(partSelected, materialSelected);

        // Can sua cho nay
        weaponObjects.SetWeaponPartMaterial(weaponIndexSelected, 
            0, partSelected, materialSelected);

        // Luu Data
        if (!PlayerPrefs.HasKey(weaponIndexSelected.ToString())){
            SaveData dataTemp = new SaveData();
            string jsonTemp = JsonUtility.ToJson(dataTemp);
            PlayerPrefs.SetString(weaponIndexSelected.ToString(), jsonTemp);
        }
        SaveData data = GetWeaponData(weaponIndexSelected);
        //Debug.Log("Vu khi:" + weaponIndexSelected + " " + JsonUtility.ToJson(data));
        data.weaponMaterials[partSelected] = materialIndex;

        SaveWeaponData(weaponIndexSelected, data);

    }
    void SelectPart(int ind) {
        partSelected = ind;
    }
    // Set material theo tung part
    void SetWeaponDisplay(int weaponPart, Material material) {
        // hien thi mau tren phan duoc chon
        MeshRenderer renderer = weaponDisplay.GetComponent<MeshRenderer>();
        Material[] weaponDisplayMaterials = renderer.sharedMaterials;
        weaponDisplayMaterials[weaponPart] = material;
        renderer.sharedMaterials = weaponDisplayMaterials;

        //hien thi mau tren button duoc chon
        listWeaponPartButtons[weaponPart].GetComponent<Image>().color = material.color;

    }
    private SaveData GetWeaponData(int weaponIndex) {
        string json = PlayerPrefs.GetString(weaponIndex.ToString());
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        return data;
    }

    private void SaveWeaponData(int weaponIndex, SaveData data) {
        string json = JsonUtility.ToJson(data);
        Debug.Log("Save Weapon: " + weaponIndex + "skin: " + data.skinIndex + ", " + data.showMaterial());
        PlayerPrefs.SetString(weaponIndex.ToString(), json);
    }
    private class SaveData {
        public int skinIndex;
        public int[] weaponMaterials = new int[3];

        public string showMaterial() {
            return weaponMaterials[0] + " " + weaponMaterials[1] + " " + weaponMaterials[2];
        }
    }
}
