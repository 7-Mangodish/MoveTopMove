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
        if(currentHatSet != null) 
            Destroy(currentHatSet);

        if (listSets[setIndex].hat != null)
            currentHatSet = Instantiate(listSets[setIndex].hat, hatHolderTransform);
        //else
        //    Debug.Log("Set " + setIndex + " does not have hat");
    }

    public void SetCharacterWingSet(int setIndex, Transform wingHolderTransform) {
        if (currentWingSet != null)
            Destroy(currentWingSet);


        if (listSets[setIndex].wing != null)
            currentWingSet = Instantiate(listSets[setIndex].wing, wingHolderTransform);
        //else
        //    Debug.Log("Set " + setIndex + " does not have wing");
    }

    public void SetCharacterArmorSet(int setIndex, Transform armorHolderTransform) {
        if (currentArmorSet != null)
            Destroy(currentArmorSet);

        if (listSets[setIndex].armor != null)
            currentArmorSet = Instantiate(listSets[setIndex].armor, armorHolderTransform);
        //else
        //    Debug.Log("Set " + setIndex + " does not have armor");
    }

    public void SetCharacterTailSet(int setIndex, Transform tailHolderTransform) {
        if (currentTailSet != null)
            Destroy(currentTailSet);

        if (listSets[setIndex].tail != null)
            currentTailSet = Instantiate(listSets[setIndex].tail, tailHolderTransform);
        //else
        //    Debug.Log("Set " + setIndex + " does not have tail");
    }

    public void SetCharacterMaterialSet(int setIndex, SkinnedMeshRenderer playerMesh) {
        if (listSets[setIndex].material != null)
            playerMesh.material = listSets[setIndex].material;    
        //else
        //    Debug.Log("Set " + setIndex + " does not have material");
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
