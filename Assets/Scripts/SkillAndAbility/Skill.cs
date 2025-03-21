using System;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "SkillObjects", menuName = "Scriptable Objects/SkillObjects")]
public class SkillObjects : ScriptableObject
{

    public float hp;//ori: 1
    public float speed;//ori: 2
    public float range;//ori: 2
    public float weaponCount;//ori: 1

    public int[] currentLevelSkill = new int[4];
    public int[] maxLevelSkill = new int[4];
    public int[] costUpdateSkill = new int[4];

    public enum TypeSkill {
        none,
        hp,
        speed,
        range,
        weaponCount
    }
    
    public void resetLevel() {
        hp = 1;
        speed = 2;
        range = 2;
        weaponCount = 1;

        for(int i =0; i< currentLevelSkill.Length; i++) {
            currentLevelSkill[i] = 1;
        }
    }
    public bool UpgradeHp() {
        if(currentLevelSkill[0] < maxLevelSkill[0]) {
            currentLevelSkill[0]++;
            hp++;
            return true;
        }
        return false;
    }

    public bool UpgradeSpeed() {
        if (currentLevelSkill[1] < maxLevelSkill[1]) {
            currentLevelSkill[1]++;
            speed += .4f;
            return true;
        }
        return false;
    }

    public bool UpgradeRange() {
        if (currentLevelSkill[2] < maxLevelSkill[2]) {
            currentLevelSkill[2]++;
            range += .4f;
            return true;
        }
        return false;
    }

    public bool UpgradeWeaponCount() {
        if (currentLevelSkill[3] < maxLevelSkill[3]) {
            currentLevelSkill[3]++;
            weaponCount ++;
            return true;
        }
        return false;
    }
}
