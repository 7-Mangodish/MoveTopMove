using UnityEngine;

[CreateAssetMenu(fileName = "SetObjects", menuName = "Scriptable Objects/SetObjects")]
public class SetObjects : ScriptableObject
{
    public Set[] listSets;
    private GameObject currentHatSet;
    private GameObject currentWingSet;
    private GameObject currentArmorSet;
    private GameObject currentTailSet;


    public void SetCharacterHatSet(int setIndex, Transform hatHolderTransform) {
        if(setIndex < 0  || setIndex >= listSets.Length) {
            Debug.LogWarning("Set is Out of Range");
            return;
        } 
        if(currentHatSet != null) 
            Destroy(currentHatSet);

        if (listSets[setIndex].hat != null)
            currentHatSet = Instantiate(listSets[setIndex].hat, hatHolderTransform);
    }

    public void SetCharacterWingSet(int setIndex, Transform wingHolderTransform) {
        if (setIndex < 0 || setIndex >= listSets.Length) {
            Debug.LogWarning("Set is Out of Range");
            return;
        }
        if (currentWingSet != null)
            Destroy(currentWingSet);


        if (listSets[setIndex].wing != null)
            currentWingSet = Instantiate(listSets[setIndex].wing, wingHolderTransform);
    }

    public void SetCharacterArmorSet(int setIndex, Transform armorHolderTransform) {
        if (setIndex < 0 || setIndex >= listSets.Length) {
            Debug.LogWarning("Set is Out of Range");
            return;
        }
        if (currentArmorSet != null)
            Destroy(currentArmorSet);

        if (listSets[setIndex].armor != null)
            currentArmorSet = Instantiate(listSets[setIndex].armor, armorHolderTransform);
    }

    public void SetCharacterTailSet(int setIndex, Transform tailHolderTransform) {
        if (setIndex < 0 || setIndex >= listSets.Length) {
            Debug.LogWarning("Set is Out of Range");
            return;
        }
        if (currentTailSet != null)
            Destroy(currentTailSet);

        if (listSets[setIndex].tail != null)
            currentTailSet = Instantiate(listSets[setIndex].tail, tailHolderTransform);
    }

    public void SetCharacterMaterialSet(int setIndex, SkinnedMeshRenderer playerMesh) {
        if (setIndex < 0 || setIndex >= listSets.Length) {
            Debug.LogWarning("Set is Out of Range");
            return;
        }
        if (listSets[setIndex].material != null)
            playerMesh.material = listSets[setIndex].material;    
    }

    public void DestroySet() {
        if(currentArmorSet != null)
            Destroy(currentArmorSet);
        if (currentTailSet != null)
            Destroy(currentTailSet);
        if (currentWingSet != null)
            Destroy(currentWingSet);
        if (currentHatSet != null)
            Destroy(currentHatSet);
    }
}
