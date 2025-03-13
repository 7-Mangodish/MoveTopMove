using System.Collections.Generic;
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

    [Header("UI")]
    [SerializeField] Button[] listWeaponSkinButtons = new Button[5];
    [SerializeField] private Button nextOptionsButton;
    [SerializeField] private Button previousOptionsButton;
    [SerializeField] private Material defautlMaterial;
    private int weaponSkinIndexSelected;

    [Header("Color")]
    [SerializeField] Button[] listColorButtons = new Button[18];
    [SerializeField] Button[] listWeaponPartButtons = new Button[3];
    private int materialIndexSelected;
    private int partSelected;

    //[SerializeField] private  WeaponDataObject weaponDataObject;
    public enum WeaponInfor {
        //weaponIndexSeleted,
        weaponSkinIndexSelected,
        materialIndex0,
        materialIndex1,
        materialIndex2,
    }
    public WeaponInfor weaponInfor;
    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        nextOptionsButton.onClick.AddListener(() =>{
            weaponIndexSelected += 1;
            if (weaponIndexSelected > weaponObjects.listWeapon.Length)
                weaponIndexSelected = 0;          
        });

        previousOptionsButton.onClick.AddListener(() => {
            weaponIndexSelected -= 1;
            if (weaponIndexSelected < 0)
                weaponIndexSelected = weaponObjects.listWeapon.Length-1;
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
        //for(int i=0; i<listWeaponPartButtons.Length; i++) {
        //    int colorIndex = PlayerPrefs.GetInt(i.ToString());
        //    listWeaponPartButtons[i].GetComponent<Image>().color =
        //        listColorButtons[colorIndex].GetComponent<Image>().material.color;
        //}
    }

    void SelectSkin(int skinIndex) {
        //Debug.Log("Vu khi thu: " + weaponIndexSelected + ", Skin thu: " + skinIndex);
        PlayerPrefs.SetInt(WeaponInfor.weaponSkinIndexSelected.ToString(), skinIndex);

        //Lay mesh va material cua weapon
        Mesh weaponMesh = weaponObjects.GetMeshWeapon(weaponIndexSelected, skinIndex);
        Material[] weaponMaterials = weaponObjects.GetListMaterials(weaponIndexSelected, skinIndex);

        //Hien thi tren weapon
        weaponDisplay.GetComponent<MeshFilter>().mesh = weaponMesh;

        MeshRenderer renderer = weaponDisplay.GetComponent<MeshRenderer>();
        renderer.sharedMaterials = weaponMaterials;
    }

    void SelectMaterial(int colorIndex) {   
        Material materialSelected = listColorButtons[colorIndex].GetComponent<Image>().material;
        int skin = PlayerPrefs.GetInt(WeaponInfor.weaponSkinIndexSelected.ToString());
        
        SetWeaponDisplay(partSelected, materialSelected);
        //PlayerPrefs.SetInt(partSelected.ToString(), colorIndex);
        //if(weaponIndexSelected < weaponDataObject.listWeaponDatas.Count) {
        //    WeaponData data = new WeaponData(weaponSkinIndexSelected);
        //    weaponDataObject.listWeaponDatas.Add(data);
        //}
        //weaponDataObject.SetMaterialIndexSelected(weaponIndexSelected, partSelected, colorIndex);

        Debug.Log(materialSelected.name);
    }
    void SelectPart(int ind) {
        partSelected = ind;
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
}
