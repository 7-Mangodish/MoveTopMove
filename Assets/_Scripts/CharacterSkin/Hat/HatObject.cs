
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HatObjects", menuName = "Scriptable Objects/HatObjects")]
public class HatObjects : ScriptableObject
{
    public List<Hat> listHats;
    private GameObject currentHat;
    public void SetCharacterHat(int hatIndex, Transform hatHolderTransform) {
        if(hatIndex < 0 || hatIndex >= listHats.Count) {
            //Debug.LogWarning("Hat Index Out of range: " + hatIndex);
            return;
        }
        if(currentHat != null)
            Destroy(currentHat);
        currentHat = Instantiate(listHats[hatIndex].hatPrefab, hatHolderTransform);
        //Debug.Log(hatHolderTransform.gameObject);
    }

    public void DestroyHat() {
        if (currentHat != null)
            Destroy(currentHat);
    }
}
