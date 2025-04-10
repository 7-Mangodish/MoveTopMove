using UnityEngine;

[CreateAssetMenu(fileName = "PantObjects", menuName = "Scriptable Objects/PantObjects")]
public class PantObjects : ScriptableObject
{
    public Pant[] listPants;
    public Material defautlPantMaterial;

    public void SetPantMaterial(int pantIndex,SkinnedMeshRenderer pantSkin) {
        if(pantIndex < 0 || pantIndex >= listPants.Length) {
            Debug.LogWarning("Pant Index Out of Range: " + pantIndex);
            pantSkin.material = defautlPantMaterial;
            return;
        }

        pantSkin.material = listPants[pantIndex].material;
    }
}
