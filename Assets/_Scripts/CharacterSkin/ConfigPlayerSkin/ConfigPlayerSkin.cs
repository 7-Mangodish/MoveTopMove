using UnityEngine;

[CreateAssetMenu(fileName = "ConfigPlayerSkin", menuName = "Scriptable Objects/ConfigPlayerSkin", order = 2)]
public class ConfigPlayerSkin : ScriptableObject{
    public GameObject[] listHatObject;
    public Material[] listPantMaterial;
    public GameObject[] listArmorObject;
    public PlayerSet[] listPlayerSetObject;
}
[System.Serializable]
public class PlayerSet {
    public GameObject hatSetObject;
    public GameObject armorSetObject;
    public GameObject wingSetObject;
    public GameObject tailSetObject;
    public Material matSetObject;
}
