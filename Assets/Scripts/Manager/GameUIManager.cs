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

    [SerializeField] private GameObject topPanel;
    [SerializeField] private GameObject coinPanel;

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
    [SerializeField] private GameObject deadPanel;
    [SerializeField] private Button continueButton;

    [Header("Player Win")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private Button playNextZone;

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

        SetUpWinPanel();
    }

    private void Start() {
        homeManager = GameObject.FindFirstObjectByType<HomePageManager>();
        homeManager.OnStartGame += GameUIManager_OnStartGame;

        GameManager.Instance.OnPlayerLose += GameUIManager_OnPlayerLose;
        GameManager.Instance.OnPlayerWin += GameUIManager_OnPlayerWin;
        GameManager.Instance.OnEnemyQuantityDown += GameUIManager_OnEnemyQuantityDown;
    }

    private void SetUpRevivePanel() {
        exitReviveButton.onClick.AddListener(() => {
            revivePanel.gameObject.SetActive(false);
            deadPanel.gameObject.SetActive(true);
        });
    }

    private void SetUpDeadPanel() {
        continueButton.onClick.AddListener(() => {
            SceneManager.LoadScene(0);
            deadPanel.gameObject.SetActive(false);
        });
    }

    private void SetUpWinPanel() {
        playNextZone.onClick.AddListener(() => {
            SceneManager.LoadScene(0);
            winPanel.gameObject.SetActive(false);
        });
    }
    private void GameUIManager_OnEnemyQuantityDown(object sender, int e) {
        Debug.Log(e);
        enemyQuantityText.text = "Alive: " +  e.ToString();
    }

    private void GameUIManager_OnStartGame(object sender, System.EventArgs e) {
        topPanel.SetActive(true);
        IntructionPanelManager.Instance.TurnOnPanel();
        coinPanel.gameObject.SetActive(false);
    }

    private void GameUIManager_OnPlayerLose(object sender, System.EventArgs e) {
        revivePanel.gameObject.SetActive(true);

    }

    private async void GameUIManager_OnPlayerWin(object sender, System.EventArgs e) {
        await Task.Delay(1000);
        winPanel.SetActive(true);
    }
}
