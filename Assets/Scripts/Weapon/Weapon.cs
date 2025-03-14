using UnityEngine;

[System.Serializable]
public class Weapon 
{

    //[SerializeField] private GameObject weaponObject;
    [SerializeField] private GameObject[] weaponSkins;
    [SerializeField] private string name;
    [SerializeField] private bool isLock;
    public enum WeaponAttribute {
        None,
        Range, 
        Damage  
    }

    public WeaponAttribute attribute;
    public int index;

    //public GameObject GetWeapon() {
    //    return weaponObject;
    //}

    public GameObject GetWeaponSkin(int index) {
        return weaponSkins[index];
    }

    public void TurnOff() {
        foreach(GameObject weapon in weaponSkins) {
            weapon.gameObject.SetActive(false);
        }
    }

    public void SetMaterialWeaponSkin(int skinIndex, int weaponPart, Material material) {
        Renderer renderer = weaponSkins[skinIndex].GetComponent<Renderer>();

        Material[] materials = renderer.sharedMaterials;
        materials[weaponPart] = material;

        renderer.sharedMaterials = materials;
    }
}
