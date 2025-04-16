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

    private int BEGIN_SHIELD_COST = 100;
    private int BEGIN_SPEED_COST = 200;
    private int BEGIN_RANGE_COST = 500;
    private int BEGIN_WEAPON_COST = 1000;
    public enum TypeSkill {
        none,
        hp,
        speed,
        range,
        weaponCount
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

    public void UpdateSkillCost() {
        costUpdateSkill[0] = BEGIN_SHIELD_COST * (currentLevelSkill[0] +1);
        costUpdateSkill[1] = BEGIN_SPEED_COST * (currentLevelSkill[1] +1);
        costUpdateSkill[2] = BEGIN_RANGE_COST * (currentLevelSkill[2] +1);
        costUpdateSkill[3] = BEGIN_WEAPON_COST * (currentLevelSkill[3] + 1);
    }
}
