using UnityEngine;

public class LoadPlayerSkinManager : MonoBehaviour
{
    private static LoadPlayerSkinManager instance;
    public static LoadPlayerSkinManager Instance;

    [Header("Hat")]
    [SerializeField] private GameObject hatObjects;
    [SerializeField] private Transform hatHolderTransform;
    private void Awake() {
        if(instance == null) {
            instance = this;
        }
        else {
            Destroy(this.gameObject);
        }
    }
    void Start()
    {


    }

    void Update()
    {
        
    }

    public void LoadPlayerHat() {
        if (!PlayerPrefs.HasKey("PlayerHat")) {
            PlayerPrefs.SetInt("PlayerHat", 0);
        }
        else {
            Debug.Log("Hat Saved: " + PlayerPrefs.GetInt("PlayerHat"));
        }
    }
}
