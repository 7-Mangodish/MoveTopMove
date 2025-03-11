using System.Threading.Tasks;
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

    [Header("Player's Scale")]
    [SerializeField] private TextMeshProUGUI playerScaleText;

    [Header("Player Lose")]
    /*Revive Panel*/
    [SerializeField] private GameObject revivePanel;
    [SerializeField] private Button exitReviveButton;
    /*Dead Panel*/
    [SerializeField] private GameObject DeadPanel;
    [SerializeField] private Button continueButton;

    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        enemyQuantityText.text ="Alive: " + GameManager.Instance.GetMaxEnemyQuantity();

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

        SetUpRevivePanel();
        SetUpDeadPanel();
    }

    private void Start() {
        GameManager.Instance.OnPlayerLose += GameUIManager_OnPlayerLose;
    }

    private void GameUIManager_OnPlayerLose(object sender, System.EventArgs e) {
        revivePanel.gameObject.SetActive(true);

    }

    public void StartGame() {
        startPanel.SetActive(false);

        GameManager.Instance.OnEnemyQuantityDown += UIManager_OnEnemyQuantityDown;
    }

    private void SetUpRevivePanel() {
        exitReviveButton.onClick.AddListener(() => {
            revivePanel.gameObject.SetActive(false);
            DeadPanel.gameObject.SetActive(true);
        });
    }

    private void SetUpDeadPanel() {
        continueButton.onClick.AddListener(() => {
            Debug.Log("Continue");
            DeadPanel.gameObject.SetActive(false);
        });
    }
    private void UIManager_OnEnemyQuantityDown(object sender, int e) {
        Debug.Log(e);
        enemyQuantityText.text = "Alive: " +  e.ToString();
    }

    public async void DisplayPlayerScale(float playerScale) {
        playerScaleText.gameObject.SetActive(true);
        playerScaleText.text = playerScale.ToString() + "m";
        playerScaleText.GetComponent<Animator>().Play("PlayerScaleText");
        await Task.Delay(1500);
        playerScaleText.gameObject.SetActive(false);
    }
}
