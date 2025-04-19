using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
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

    [Header("-----Setting-----")]
    [SerializeField] private Button settingButton;
    [SerializeField] private GameObject settingUIPanel;
    [SerializeField] private Button settingHomeButton;
    [SerializeField] private Button settingContinueButton;

    [Header("-----InGame-----")]
    [SerializeField] private TextMeshProUGUI enemyQuantityText;
    [SerializeField] private GameObject instructionPanel;

    [Header("-----Player Lose-----")]
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
    public bool isRevived;
    public bool isClickedRevive;

    [Header("-----Player Win-----")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private Button playNextZone;
    [SerializeField] private TextMeshProUGUI winCoinText;
    [SerializeField] private int winCoin;
    [SerializeField] private Button winx3RewardButton;
    private HomePageController homeManager;

    private List<GameObject> listCharacterDisplay = new List<GameObject>();
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
        StartCoroutine(TurnOffInstructPanel());
    }

    private void Start() {
        MaxManager.Instance.OnPlayerReceiveAward += GameUI_OnPlayerReceiveAward;
        isRevived = false;
        isClickedx3Reward = false;
    }

    private void OnDisable() {
        MaxManager.Instance.OnPlayerReceiveAward -= GameUI_OnPlayerReceiveAward;
    }
    private void SetSetUpPanel() {
        settingButton.onClick.AddListener(() => {
            settingUIPanel.gameObject.SetActive(true);
            TurnOffFloatingText();
            Time.timeScale = 0;
        });

        settingHomeButton.onClick.AddListener(() => {
            SceneManager.LoadScene(GameVariable.normalSceneName);
            Time.timeScale = 1f;
        });

        settingContinueButton.onClick.AddListener(() => {
            settingUIPanel.SetActive(false);
            Time.timeScale = 1f;
            TurnOnFloatingText();
        });
    }
    private void SetUpRevivePanel() {
        exitReviveButton.onClick.AddListener(() => {
            revivePanel.gameObject.SetActive(false);
            deadPanel.gameObject.SetActive(true);
        });
        reviveByAdButton.onClick.AddListener(() => {
            Debug.Log(this.name);
            MaxManager.Instance.ShowRewardAd();
            isClickedRevive = true;
        });
    }

    private void SetUpDeadPanel() {
        continueButton.onClick.AddListener(() => {
            SceneManager.LoadScene(GameVariable.normalSceneName);

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
            SceneManager.LoadScene(GameVariable.normalSceneName);

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
        instructionPanel.SetActive(true);
        coinPanel.gameObject.SetActive(false);
    }

    public async void TurnOnReviveUI() {
        TurnOffFloatingText();
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
        TurnOffFloatingText();
        await Task.Delay(1000);
        winPanel.SetActive(true);
    }

    private void GameUI_OnPlayerReceiveAward(object sender, MaxManager.TypeReward t) {
        if (isClickedRevive && t == MaxManager.TypeReward.revive) {
            //GameController.Instance.DoPlayerRevive();
            revivePanel.gameObject.SetActive(false);
            isRevived = true;
        }
    }

    public void TurnOffFloatingText() {
        GameObject[] listName = GameObject.FindGameObjectsWithTag("NameCharacter");

        listCharacterDisplay.Clear();
        listCharacterDisplay.AddRange(listName);
        foreach (GameObject obs in listCharacterDisplay) {
            obs.SetActive(false);
        }
    }

    public void TurnOnFloatingText() {
        foreach (GameObject obs in listCharacterDisplay) {
            obs.SetActive(true);
        }
    }
    IEnumerator TurnOffInstructPanel() {
        yield return new  WaitUntil(()=> PlayerController.Instance.startGame);
        instructionPanel.SetActive(false);
    }
}
