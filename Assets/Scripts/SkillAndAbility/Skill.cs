using System;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "SkillObjects", menuName = "Scriptable Objects/SkillObjects")]
public class SkillObjects : ScriptableObject
{

    public float shield;//ori: 0
    public float speed;//ori: 2
    public float range;//ori: 2
    public float weaponBonus;//ori: 1

    public int[] currentLevelSkill = new int[4];
    public int[] maxLevelSkill = new int[4];
    public int[] costUpdateSkill = new int[4];

    public float BEGIN_SHIELD = 0;
    public float BEGIN_SPEED = 2;
    public float BEGIN_RANGE = 2;
    public float BEGIN_WEAPON = 0;
    public enum TypeSkill {
        none,
        hp,
        speed,
        range,
        weaponCount
    }
    public void resetLevel() {
        shield = 0;
        speed = 2;
        range = 2;
        weaponBonus = 0;

        for(int i =0; i< currentLevelSkill.Length; i++) {
            currentLevelSkill[i] = 1;
        }
    }
    public bool UpgradeHp() {
        if(currentLevelSkill[0] < maxLevelSkill[0]) {
            currentLevelSkill[0]++;
            shield++;
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
            range += .5f;
            return true;
        }
        return false;
    }
    public bool UpgradeWeaponCount() {
        if (currentLevelSkill[3] < maxLevelSkill[3]) {
            currentLevelSkill[3]++;
            weaponBonus ++;
            return true;
        }
        return false;
    }

    public void InitPlayerSkill() {
        shield = currentLevelSkill[0];
        speed = currentLevelSkill[1] * 0.4f + 2;
        range = currentLevelSkill[2] * .5f + 2;
        weaponBonus = currentLevelSkill[3];
    }
}
