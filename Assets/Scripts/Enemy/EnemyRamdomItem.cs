using Unity.VisualScripting;
using UnityEngine;

public class EnemyRamdomItem : MonoBehaviour {

    [Header("----Material----")]
    [SerializeField] private MaterialObjects materialObjects;
    private SkinnedMeshRenderer characterSkin;


    [Header("----Hat-----")]
    [SerializeField] private HatObjects hatObjects;
    [SerializeField] private Transform hatHolder;

    [Header("----Pant----")]
    [SerializeField] private PantObjects pantObjects;
    [SerializeField] private SkinnedMeshRenderer pantRenderer;

    [Header("----Weapon----")]
    [SerializeField] private GameObject[] listWeapons;
    [SerializeField] private GameObject weaponHold;
    private Mesh weaponMesh;
    private Material[] listWeaponMaterials;
    private int randWeapon;

    private void Awake() {
        randWeapon = Random.Range(0, listWeapons.Length);
        weaponMesh = listWeapons[randWeapon].GetComponent<MeshFilter>().sharedMesh;
        listWeaponMaterials = listWeapons[randWeapon].GetComponent<MeshRenderer>().sharedMaterials;

        RandomEnemyMaterial();
        RandomEnemyHat();
        RandomEnemyPant();
        RandomEnemyWeaponHold();
    }

    void RandomEnemyMaterial() {
        characterSkin = GetComponentInChildren<SkinnedMeshRenderer>();
        int randNumber = Random.Range(0, materialObjects.listMaterials.Length);
        characterSkin.material = materialObjects.listMaterials[randNumber];
    }
    void RandomEnemyHat() {
        int rand = Random.Range(0, hatObjects.listHats.Length);
        Instantiate(hatObjects.listHats[rand].hatPrefab, hatHolder);
    }
    void RandomEnemyPant() {
        int rand = Random.Range(0, pantObjects.listPants.Length);
        pantRenderer.material = pantObjects.listPants[rand].material;
    }

    void RandomEnemyWeaponHold() {
        weaponHold.GetComponent<MeshFilter>().mesh = weaponMesh;
        weaponHold.GetComponent<MeshRenderer>().sharedMaterials = listWeaponMaterials;
    }

    public GameObject GetRandomEnemyWeapon() {
        return listWeapons[randWeapon];
    }
}
