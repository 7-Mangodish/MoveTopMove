using Unity.VisualScripting;
using UnityEngine;

public class SkillDataManager : MonoBehaviour
{
    //private static SkillDataManager instance;
    //public static SkillDataManager Instance { get => instance; }

    //private void Awake() {
    //    if (instance == null)
    //        instance = this;
    //    else
    //        Destroy(this.gameObject);
    //}

    //public SkillData GetSkillData() {
    //    if (!PlayerPrefs.HasKey("PlayerSkill")) {
    //        SkillData skillData = new SkillData();
    //        string json = JsonUtility.ToJson(skillData);
    //        PlayerPrefs.SetString("PlayerSkill", json);
    //    }
    //    string jsonData = PlayerPrefs.GetString("PlayerSkill");
    //    SkillData data = JsonUtility.FromJson<SkillData>(jsonData);
    //    return data;
    //}

    //public void SetSkillData(SkillData data) {
    //    string json = JsonUtility.ToJson(data);
    //    PlayerPrefs.SetString("PlayerSkill", json);
    //}
}
