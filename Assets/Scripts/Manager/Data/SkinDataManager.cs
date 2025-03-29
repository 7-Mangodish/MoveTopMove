using UnityEngine;

public class SkinDataManager : MonoBehaviour
{
    private static SkinDataManager instance;
    public static SkinDataManager Instance {  get { return instance; } }

    private void Awake() {
        if(instance == null) {
            instance = this;
        }
        else {
            Destroy(this.gameObject);
        }
    }

    public SkinData GetSkinData() {
        string json = PlayerPrefs.GetString("PlayerSkin");
        SkinData data = JsonUtility.FromJson<SkinData>(json);
        return data;
    }
    public void SaveSkinData(SkinData data) {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("PlayerSkin", json);
        Debug.Log("Save Skin: " + data.hatIndex + " " + data.pantIndex + " " + data.armorIndex + " " + data.setIndex);
    }
    public class SkinData {
        public int hatIndex;
        public int pantIndex;
        public int armorIndex;
        public int setIndex;
        public bool isSet;
    }
}
