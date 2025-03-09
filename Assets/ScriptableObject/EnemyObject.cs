using UnityEngine;

[CreateAssetMenu(fileName = "EnemyObject", menuName = "EnemyScriptableObject")]
public class EnemyObject : ScriptableObject
{
    [SerializeField] private GameObject enemyObject;
    [SerializeField] private Material[] listPantsMaterial;
}
