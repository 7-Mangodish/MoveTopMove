using UnityEngine;
using static SkillObjects;

public class LoadSkinWhenStart : MonoBehaviour
{
    [Header("Hat")]
    [SerializeField] private HatObjects hatObjects;
    [SerializeField] private Transform hatHolderTransform;

    [Header("Pant")]
    [SerializeField] private PantObjects pantObjects;
    [SerializeField] private GameObject characterPant;

    [Header("Armor")]
    [SerializeField] private ArmorObjects armorObjects;
    [SerializeField] private Transform armorHolderTransform;

    [Header("Set")]
    [SerializeField] private SetObjects setObjects;
    [SerializeField] private Transform wingHolderTransform;
    [SerializeField] private Transform tailHolderTransform;
    [SerializeField] private SkinnedMeshRenderer playerMesh;
    [SerializeField] private Material playerMaterial;

    private SkinDataManager.SkinData data;
    void Start()
    {
        data = SkinDataManager.Instance.GetSkinData();
        LoadSkin();
    }

    void LoadSkin() {
        if (data.isSet) {
            hatObjects.DestroyHat();
            characterPant.gameObject.SetActive(false);
            armorObjects.DestroyArmor();

            setObjects.SetCharacterHatSet(data.setIndex, hatHolderTransform);
            setObjects.SetCharacterArmorSet(data.setIndex, armorHolderTransform);
            setObjects.SetCharacterWingSet(data.setIndex, wingHolderTransform);
            setObjects.SetCharacterTailSet(data.setIndex, tailHolderTransform);
            setObjects.SetCharacterMaterialSet(data.setIndex, playerMesh);
        }
        else {
            SkinnedMeshRenderer pantSkin = characterPant.GetComponent<SkinnedMeshRenderer>();

            setObjects.DestroySet();
            if (!characterPant.gameObject.activeSelf)
                characterPant.gameObject.SetActive(true);
            playerMesh.material = playerMaterial;
            hatObjects.SetCharacterHat(data.hatIndex, hatHolderTransform);
            pantObjects.SetPantMaterial(data.pantIndex, pantSkin);
            armorObjects.SetCharacterArmor(data.armorIndex, armorHolderTransform);
        }
    }
}
