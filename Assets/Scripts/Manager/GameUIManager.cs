using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    private static GameUIManager instance;
    public static GameUIManager Instance { get { return instance; } }

    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject topPanel;

    [Header("Setting")]
    [SerializeField] private Button settingButton;
    [SerializeField] private GameObject settingUIPanel;
    [SerializeField] private Button settingHomeButton;
    [SerializeField] private Button settingContinueButton;

    [Header("Enemy's Quantity")]
    [SerializeField] private TextMeshProUGUI enemyQuantityText;



    [Header("Player Lose")]
    /*Revive Panel*/
    [SerializeField] private GameObject revivePanel;
    [SerializeField] private Button exitReviveButton;
    /*Dead Panel*/
    [SerializeField] private GameObject DeadPanel;
    [SerializeField] private Button continueButton;


    private HomePageManager homeManager;
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
            SceneManager.LoadScene(0);
        });

        settingContinueButton.onClick.AddListener(() => {
            settingUIPanel.SetActive(false);
            Time.timeScale = 1f;
        });

        SetUpRevivePanel();
        SetUpDeadPanel();
    }

    private void Start() {
        homeManager = GameObject.FindFirstObjectByType<HomePageManager>();
        homeManager.OnStartGame += GameUIManager_OnStartGame;

        GameManager.Instance.OnPlayerLose += GameUIManager_OnPlayerLose;
        GameManager.Instance.OnEnemyQuantityDown += GameUIManager_OnEnemyQuantityDown;
    }

    private void GameUIManager_OnStartGame(object sender, System.EventArgs e) {
        startPanel.SetActive(true);
        topPanel.SetActive(true);
    }

    private void GameUIManager_OnPlayerLose(object sender, System.EventArgs e) {
        revivePanel.gameObject.SetActive(true);

    }

    public void TurnOffStartPanel() {
        startPanel.SetActive(false);
    }

    private void SetUpRevivePanel() {
        exitReviveButton.onClick.AddListener(() => {
            revivePanel.gameObject.SetActive(false);
            DeadPanel.gameObject.SetActive(true);
        });
    }

    private void SetUpDeadPanel() {
        continueButton.onClick.AddListener(() => {
            SceneManager.LoadScene(0);
            DeadPanel.gameObject.SetActive(false);
        });
    }
    private void GameUIManager_OnEnemyQuantityDown(object sender, int e) {
        Debug.Log(e);
        enemyQuantityText.text = "Alive: " +  e.ToString();
    }

}
