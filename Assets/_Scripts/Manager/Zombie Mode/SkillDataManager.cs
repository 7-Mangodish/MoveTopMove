using Unity.VisualScripting;
using UnityEngine;

public class SkillDataManager : MonoBehaviour
{
    private static SkillDataManager instance;
    public static SkillDataManager Instance { get => instance; }

    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    public SkillData GetSkillData() {
        if (!PlayerPrefs.HasKey("PlayerSkill")) {
            SkillData skillData = new SkillData();
            string json = JsonUtility.ToJson(skillData);
            PlayerPrefs.SetString("PlayerSkill", json);
        }
        string jsonData = PlayerPrefs.GetString("PlayerSkill");
        SkillData data = JsonUtility.FromJson<SkillData>(jsonData);
        return data;
    }

    public void SetSkillData(SkillData data) {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("PlayerSkill", json);
    }

    public class SkillData {
        public float shield;

        public float speed;

        public float range;

        public float weaponBonus;

        public int[] skillLevel = new int[4];
        public int[] skillCost = new int[4];
        public int[] maxLevelSkill = new int[4];

        private int BEGIN_SHIELD_COST = 100;
        private int BEGIN_SPEED_COST = 200;
        private int BEGIN_RANGE_COST = 500;
        private int BEGIN_WEAPON_COST = 1000;

        public float BEGIN_SHIELD = 0;
        public float BEGIN_SPEED = 2;
        public float BEGIN_RANGE = 2;
        public float BEGIN_WEAPON = 0;

        public SkillData() {
            shield = BEGIN_SHIELD;
            speed = BEGIN_SPEED;
            range = BEGIN_RANGE;
            weaponBonus = BEGIN_WEAPON;

            skillCost[0] = BEGIN_SHIELD_COST;
            skillCost[1] = BEGIN_SPEED_COST;
            skillCost[2] = BEGIN_RANGE_COST;
            skillCost[3] = BEGIN_WEAPON_COST;

            maxLevelSkill[0] = 3;
            maxLevelSkill[1] = 5;
            maxLevelSkill[2] = 5;
            maxLevelSkill[3] = 5;
        }

        public void UpdateSkillCost() {
            skillCost[0] = BEGIN_SHIELD_COST * (skillLevel[0] + 1);
            skillCost[1] = BEGIN_SPEED_COST * (skillLevel[1] + 1);
            skillCost[2] = BEGIN_RANGE_COST * (skillLevel[2] + 1);
            skillCost[3] = BEGIN_WEAPON_COST * (skillLevel[3] + 1);
        }

        public bool UpgradeHp() {
            if (skillLevel[0] < maxLevelSkill[0]) {
                skillLevel[0]++;
                shield++;
                return true;
            }
            return false;
        }

        public bool UpgradeSpeed() {
            if (skillLevel[1] < maxLevelSkill[1]) {
                skillLevel[1]++;
                speed += .4f;
                return true;
            }
            return false;
        }

        public bool UpgradeRange() {
            if (skillLevel[2] < maxLevelSkill[2]) {
                skillLevel[2]++;
                range += .5f;
                return true;
            }
            return false;
        }

        public bool UpgradeWeaponCount() {
            if (skillLevel[3] < maxLevelSkill[3]) {
                skillLevel[3]++;
                weaponBonus++;
                return true;
            }
            return false;
        }

        public void InitPlayerSkill() {
            shield = skillLevel[0];
            speed = skillLevel[1] * 0.4f + 2;
            range = skillLevel[2] * .5f + 2;
            weaponBonus = skillLevel[3];
        }
    }
}
