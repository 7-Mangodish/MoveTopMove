using UnityEngine;

[CreateAssetMenu(fileName = "ArmorObject", menuName = "Scriptable Objects/ArmorObject")]
public class ArmorObjects : ScriptableObject
{
    public Armor[] listArmor;
    private GameObject currentArmor;

    public void SetCharacterArmor(int armorIndex, Transform armorHolderTransform) {
        if (armorIndex < 0 || armorIndex >= listArmor.Length) {
            Debug.LogWarning("Armor Index Out of Range " + armorIndex);
            return;
        }
        if (currentArmor != null)
            Destroy(currentArmor);
        currentArmor = Instantiate(listArmor[armorIndex].armorPrefab, armorHolderTransform);
    }

    public void DestroyArmor() {
        if (currentArmor != null)
            Destroy(currentArmor);
    }
}
