using JetBrains.Annotations;
using UnityEngine;

[System.Serializable]
public class Weapon 
{
    [SerializeField] private GameObject[] weaponSkins;
    [SerializeField] public int[] weaponSkinCost = new int[5];
    [SerializeField] public float scale;
    [SerializeField] public string name;
    [SerializeField] public int cost;

    [HideInInspector] public float standardScale = 4000;
    public enum WeaponAttribute {
        None,
        Range, 
        Damage  
    }

    public WeaponAttribute attribute;
    public int index;

    public GameObject GetWeaponSkin(int index) {
        return weaponSkins[index];
    }


    //public void SetMaterialWeaponSkin(int skinIndex, int weaponPart, Material material) {
    //    Renderer renderer = weaponSkins[skinIndex].GetComponent<Renderer>();

    //    Material[] materials = renderer.sharedMaterials;
    //    materials[weaponPart] = material;

    //    renderer.sharedMaterials = materials;
    //}
}
