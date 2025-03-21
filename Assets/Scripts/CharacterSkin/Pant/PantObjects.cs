using UnityEngine;

[CreateAssetMenu(fileName = "PantObjects", menuName = "Scriptable Objects/PantObjects")]
public class PantObjects : ScriptableObject
{
    public Pant[] listPants;

    public void SetPantMaterial(int pantIndex,SkinnedMeshRenderer pantSkin) {
        pantSkin.material = listPants[pantIndex].material;
    }
}
