using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "AbilitiesObjects", menuName = "Scriptable Objects/AbilitiesObjects")]
public class AbilitiesObjects : ScriptableObject
{
    public Sprite[] listAbilitySprite;
    
    public List<int> GetRandomAbilities() {
        int num1 = Random.Range(0, listAbilitySprite.Length);
        int num2 = num1;
        while (num1 == num2)
            num2 = Random.Range(0, listAbilitySprite.Length);
        List<int> list = new List<int>();    
        list.Add(num1);
        list.Add(num2);
        return list;
    }

    public Sprite GetSprite(int index) {
        return listAbilitySprite[index];
    }
}
