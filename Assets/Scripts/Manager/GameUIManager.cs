using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    private static GameUIManager instance;
    public static GameUIManager Instance { get { return instance; } }

    [SerializeField] private GameObject startPanel;

    [Header("Setting")]
    [SerializeField] private Button settingButton;
    [SerializeField] private GameObject settingUIPanel;
    [SerializeField] private Button settingHomeButton;
    [SerializeField] private Button settingContinueButton;

    [Header("Enemy's Quantity")]
    [SerializeField] private TextMeshProUGUI enemyQuantityText;

    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        enemyQuantityText.text ="Alive: " + "30";

        settingButton.onClick.AddListener(() => {
            settingUIPanel.gameObject.SetActive(true);
            Time.timeScale = 0;
        });

        settingHomeButton.onClick.AddListener(() => {
            Debug.Log("Home");
        });

        settingContinueButton.onClick.AddListener(() => {
            settingUIPanel.SetActive(false);
            Time.timeScale = 1f;
        });
    }

    public void StartGame() {
        startPanel.SetActive(false);

        GameManager.Instance.OnEnemyQuantityDown += UIManager_OnEnemyQuantityDown;
    }

    private void UIManager_OnEnemyQuantityDown(object sender, int e) {
        enemyQuantityText.text = "Alive: " +  e.ToString();
    }
}
