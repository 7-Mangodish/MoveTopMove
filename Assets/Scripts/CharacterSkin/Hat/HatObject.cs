using UnityEngine;

[CreateAssetMenu(fileName = "HatObjects", menuName = "Scriptable Objects/HatObjects")]
public class HatObjects : ScriptableObject
{
    public Hat[] listHats;
    private GameObject currentHat;
    public void SetCharacterHat(int hatIndex, Transform hatHolderTransform) {
        if(currentHat != null)
            Destroy(currentHat);
        currentHat = Instantiate(listHats[hatIndex].hatPrefab, hatHolderTransform);
        Debug.Log(hatHolderTransform.gameObject);
    }

    public void DestroyHat() {
        if (currentHat != null)
            Destroy(currentHat);
    }
}
