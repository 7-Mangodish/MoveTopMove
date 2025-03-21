using UnityEngine;

public class EnemyRandomMaterial : MonoBehaviour
{
    [SerializeField] private MaterialObjects materialObjects;
    private SkinnedMeshRenderer characterSkin;

    private void Awake() {
        characterSkin = GetComponentInChildren<SkinnedMeshRenderer>();
        int randNumber = Random.Range(0, materialObjects.listMaterials.Length);
        characterSkin.material = materialObjects.listMaterials[randNumber];
    }

    void Start()
    {

    }
}
