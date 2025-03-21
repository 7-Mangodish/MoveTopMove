using UnityEngine;

[CreateAssetMenu(fileName = "HatObjects", menuName = "Scriptable Objects/HatObjects")]
public class HatObjects : ScriptableObject
{
    public Hat[] listHats;
    private GameObject currentHat;
    public void SetCharacterHat(int hatIndex, Transform hatHolderTransform) {
        if (hatIndex == 0)
            return;
        if(currentHat != null)
            Destroy(currentHat);
        currentHat = Instantiate(listHats[hatIndex].hatPrefab, hatHolderTransform);

    }

    public void DestroyHat() {
        if (currentHat != null)
            Destroy(currentHat);
    }
}
