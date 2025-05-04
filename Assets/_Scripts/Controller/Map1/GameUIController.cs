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
    [SerializeField] private Button soundOnButton;
    [SerializeField] private Button soundOffButton;
    [SerializeField] private Button vibrationOnButton;
    [SerializeField] private Button vibrationOffButton;
    public bool isSetting;

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
    [SerializeField] private TextMeshProUGUI rankText;
    public bool isRevived;
    public bool isClickedRevive;

    [Header("-----Player Win-----")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private Button playNextZone;
    [SerializeField] private TextMeshProUGUI winCoinText;
    [SerializeField] private int winCoin;
    [SerializeField] private Button winx3RewardButton;
    private HomePageController homeManager;

    public List<GameObject> listCharacterDisplay = new List<GameObject>();
    private bool isClickedx3Reward;
    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);


        SetUpRevivePanel();
        SetUpDeadPanel();
        SetSettingPanel();
        SetUpWinPanel();
        StartCoroutine(TurnOffInstructPanel());
    }

    private void Start() {
        MaxManager.Instance.OnPlayerReceiveAward += GameUI_OnPlayerReceiveAward;
        isRevived = false;
        isClickedx3Reward = false;
    }

    private void OnDisable() {
        if(MaxManager.Instance != null)
            MaxManager.Instance.OnPlayerReceiveAward -= GameUI_OnPlayerReceiveAward;
        else
            Debug.LogWarning("Max is: "+MaxManager.Instance);
    }
    private void SetSettingPanel() {
        isSetting = false;
        settingButton.onClick.AddListener(() => {
            AudioManager.Instance.PlaySoundClickButton();
            settingUIPanel.gameObject.SetActive(true);
            Map1GameController.Instance.TurnOffEnemyStatus();
            Time.timeScale = 0;
        });

        settingHomeButton.onClick.AddListener(() => {
            AudioManager.Instance.PlaySoundClickButton();
            Map1GameController.Instance.TurnOnEnemyStatus();
            SceneManager.LoadScene(GameVariable.normalSceneName);
            Time.timeScale = 1f;
        });

        settingContinueButton.onClick.AddListener(() => {
            AudioManager.Instance.PlaySoundClickButton();
            settingUIPanel.SetActive(false);
            Map1GameController.Instance.TurnOnEnemyStatus();
            Time.timeScale = 1f;
        });

        soundOnButton.onClick.AddListener(() => {
            soundOnButton.gameObject.SetActive(false);
            soundOffButton.gameObject.SetActive(true);
            //
            DataManager.Instance.playerData.soundVolume = 0;
        });

        soundOffButton.onClick.AddListener(() => {
            soundOffButton.gameObject.SetActive(false);
            soundOnButton.gameObject.SetActive(true);
            //
            DataManager.Instance.playerData.soundVolume = 0;
        });

        vibrationOnButton.onClick.AddListener(() => {
            vibrationOnButton.gameObject.SetActive(false);
            vibrationOffButton.gameObject.SetActive(true);
        });

        vibrationOffButton.onClick.AddListener(() => {
            vibrationOffButton.gameObject.SetActive(false);
            vibrationOnButton.gameObject.SetActive(true);
        });
    }
    private void SetUpRevivePanel() {
        exitReviveButton.onClick.AddListener(() => {
            AudioManager.Instance.PlaySoundClickButton();
            //
            revivePanel.gameObject.SetActive(false);
            deadPanel.gameObject.SetActive(true);
        });
        reviveByAdButton.onClick.AddListener(() => {
            AudioManager.Instance.PlaySoundClickButton();
            //
            MaxManager.Instance.ShowRewardAd();
            isClickedRevive = true;
        });
    }

    private void SetUpDeadPanel() {
        continueButton.onClick.AddListener(() => {
            AudioManager.Instance.PlaySoundClickButton();
            SceneManager.LoadScene(GameVariable.normalSceneName);
            //
            deadPanel.gameObject.SetActive(false);
            if(isClickedx3Reward)
                CoinManager.Instance.SaveCoin(deadCoin * 3);
            else
                CoinManager.Instance.SaveCoin(deadCoin);

        });
        deadx3RewardButton.onClick.AddListener(() => {
            AudioManager.Instance.PlaySoundClickButton();
            //
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
                CoinManager.Instance.SaveCoin(winCoin * 3);
            else
                CoinManager.Instance.SaveCoin(winCoin);
        });
        winx3RewardButton.onClick.AddListener(() => {
            MaxManager.Instance.ShowRewardAd();
            isClickedx3Reward = true;
        });
        winCoinText.text = winCoin.ToString();
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
        //TurnOffFloatingText();
        if (!isRevived) {
            revivePanel.gameObject.SetActive(true);
            await Task.Delay(5200);

            if(revivePanel.gameObject!= null)
                revivePanel.gameObject.SetActive(false);
            if (!isClickedRevive)
                deadPanel.gameObject.SetActive(true);
        }
        else
            deadPanel.gameObject.SetActive(true);
    }

    public async void TurnOnWinPanel() {
        //TurnOffFloatingText();
        await Task.Delay(1000);
        if(winPanel) winPanel.SetActive(true);
    }

    private void GameUI_OnPlayerReceiveAward(object sender, MaxManager.TypeReward t) {
        if (isClickedRevive && t == MaxManager.TypeReward.revive) {
            //GameController.Instance.DoPlayerRevive();
            revivePanel.gameObject.SetActive(false);
            isRevived = true;
            FirebaseManager.Instance.HandlerClickAdEvent(FirebaseManager.TypeEvent.clickReviveAd, FirebaseManager.TypeAd.reward);
            Debug.LogWarning("isRevived: " + isRevived);
        }

    }

    //public void TurnOffFloatingText() {
    //    GameObject[] listName = GameObject.FindGameObjectsWithTag(GameVariable.CHARACTER_STATUS_TAG);
    //    listCharacterDisplay.Clear();
    //    listCharacterDisplay.AddRange(listName);
    //    foreach (GameObject obs in listCharacterDisplay) {
    //        //Debug.Log(obs);
    //        if(obs != null) 
    //            obs.SetActive(false);
    //    }
    //}

    //public void TurnOnFloatingText() {
    //    foreach (GameObject obs in listCharacterDisplay) {
    //        if(obs != null) 
    //            obs.SetActive(true);
    //    }
    //}

    public void SetUpPlayerRank(int rank) {
        rankText.text = $"#{rank}";
    }
    IEnumerator TurnOffInstructPanel() {
        yield return new  WaitUntil(()=> PlayerController.Instance.startGame);
        instructionPanel.SetActive(false);
    }
}
