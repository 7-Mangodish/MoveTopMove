using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    private static GameUIController instance;
    public static GameUIController Instance { get { return instance; } }

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
    [SerializeField] private Button reviveByAdButton;
    /*Dead Panel*/
    [SerializeField] private GameObject deadPanel;
    [SerializeField] private Button continueButton;
    [SerializeField] private TextMeshProUGUI deadCoinText;
    [SerializeField] private int deadCoin;
    [SerializeField] private Button deadx3RewardButton;
    private bool isRevived;
    private bool isClickedRevive;

    [Header("Player Win")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private Button playNextZone;
    [SerializeField] private TextMeshProUGUI winCoinText;
    [SerializeField] private int winCoin;
    [SerializeField] private Button winx3RewardButton;
    private HomePageController homeManager;

    private bool isClickedx3Reward;
    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);



        SetUpRevivePanel();
        SetUpDeadPanel();
        SetSetUpPanel();
        SetUpWinPanel();
    }

    private void Start() {

        MaxManager.Instance.OnPlayerReceiveAward += GameUI_OnPlayerReceiveAward;


        isRevived = false;
        isClickedx3Reward = false;
    }

    private void SetSetUpPanel() {
        settingButton.onClick.AddListener(() => {
            settingUIPanel.gameObject.SetActive(true);
            Time.timeScale = 0;
        });

        settingHomeButton.onClick.AddListener(() => {
            SceneManager.LoadScene(0);
            Time.timeScale = 1f;
        });

        settingContinueButton.onClick.AddListener(() => {
            settingUIPanel.SetActive(false);
            Time.timeScale = 1f;
        });
    }
    private void SetUpRevivePanel() {
        exitReviveButton.onClick.AddListener(() => {
            revivePanel.gameObject.SetActive(false);
            deadPanel.gameObject.SetActive(true);
        });
        reviveByAdButton.onClick.AddListener(() => {
            MaxManager.Instance.ShowRewardAd();
            isClickedRevive = true;
        });
    }

    private void SetUpDeadPanel() {
        continueButton.onClick.AddListener(() => {
            SceneManager.LoadScene(0);
            deadPanel.gameObject.SetActive(false);
            if(isClickedx3Reward)
                CoinManager.Instance.SaveCoin(deadCoin * 3);
            else
                CoinManager.Instance.SaveCoin(deadCoin);

        });
        deadx3RewardButton.onClick.AddListener(() => {
            MaxManager.Instance.ShowRewardAd();
            isClickedx3Reward = true;
        });
        deadCoinText.text = deadCoin.ToString();
    }

    private void SetUpWinPanel() {
        playNextZone.onClick.AddListener(() => {
            SceneManager.LoadScene(0);
            winPanel.gameObject.SetActive(false);
            if (isClickedx3Reward)
                CoinManager.Instance.SaveCoin(deadCoin * 3);
            else
                CoinManager.Instance.SaveCoin(deadCoin);
        });
        winx3RewardButton.onClick.AddListener(() => {
            MaxManager.Instance.ShowRewardAd();
            isClickedx3Reward = true;
        });
    }
    public void DisplayEnemyCount(int e) {
        enemyQuantityText.text = "Alive: " +  e.ToString();
    }

    public void TurnOnInGameUI() {
        topPanel.SetActive(true);
        IntructionPanelManager.Instance.TurnOnPanel();
        coinPanel.gameObject.SetActive(false);
    }

    public async void TurnOnReviveUI() {
        //revivePanel.gameObject.SetActive(true);
        if (!isRevived) {
            revivePanel.gameObject.SetActive(true);
            await Task.Delay(5200);
            revivePanel.gameObject.SetActive(false);
            if (!isClickedRevive)
                deadPanel.gameObject.SetActive(true);
        }
        else
            deadPanel.gameObject.SetActive(true);
    }

    public async void TurnOnWinPanel() {
        await Task.Delay(1000);
        winPanel.SetActive(true);
    }

    private void GameUI_OnPlayerReceiveAward(object sender, MaxManager.TypeReward t) {
        if (isClickedRevive && t == MaxManager.TypeReward.revive) {
            GameController.Instance.DoPlayerRevive();
            revivePanel.gameObject.SetActive(false);
            isRevived = true;
        }
    }
}
