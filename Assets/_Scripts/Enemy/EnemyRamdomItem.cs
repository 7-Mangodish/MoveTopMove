using Unity.VisualScripting;
using UnityEngine;

public class EnemyRamdomItem : MonoBehaviour {

    [Header("----Material----")]
    public MaterialObjects materialObjects;
    public SkinnedMeshRenderer characterSkin;


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


    public void RandomEnemyItem() {
        randWeapon = Random.Range(0, listWeapons.Length);
        weaponMesh = listWeapons[randWeapon].GetComponent<MeshFilter>().sharedMesh;
        listWeaponMaterials = listWeapons[randWeapon].GetComponent<MeshRenderer>().sharedMaterials;

        RandomEnemyMaterial();
        RandomEnemyHat();
        RandomEnemyPant();
        RandomEnemyWeaponHold();
    }

    void RandomEnemyMaterial() {
        int randNumber = Random.Range(0, materialObjects.listMaterials.Length);
        characterSkin.material = materialObjects.listMaterials[randNumber];
    }
    void RandomEnemyHat() {
        int rand = Random.Range(0, hatObjects.listHats.Count);
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
