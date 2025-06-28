using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomePageController : MonoBehaviour
{
    private static HomePageController instance;
    public static HomePageController Instance { get => instance; }

    [Header("WeaponShop")]
    [SerializeField] private Button weaponShopButton;
    [SerializeField] private GameObject weaponShopPanel;
    [SerializeField] private Button exitWeaponButton;

    [Header("SkinShop")]
    [SerializeField] private Button skinShopButton;
    [SerializeField] private GameObject skinShopPanel;
    [SerializeField] private Button exitSkinButton;

    [Header("Panel")]
    [SerializeField] private GameObject homePagePanel;
    private Animator homePageAnimator;
    [SerializeField] private GameObject leftPanel;
    [SerializeField] private GameObject rightPanel;
    [SerializeField] private Button playButton;
    [SerializeField] private GameObject joystick;
    [SerializeField] private Button zombieModeButton;
    [SerializeField] private TextMeshProUGUI dayZombieModeText;

    [Header("Coin")]
    [SerializeField] private GameObject exp;
    [SerializeField] private TextMeshProUGUI coinText;

    [Header("Player's Name")]
    //PlayerPersonalData playerPersonalData;
    [SerializeField] private TMP_InputField playerNameInput;

    [Header("Options Button")]
    [SerializeField] private GameObject AdBlockPanel;
    [SerializeField] private Button noAdsButton;
    [SerializeField] private Button exitAdBlockPanel;
    [SerializeField] private Button vibrationButton;
    [SerializeField] private Button volumeButton;
    [SerializeField] private GameObject turnOnObject;
    [SerializeField] private GameObject turnOffObject;
    [SerializeField] private Button cheatCoinButton;
    private bool isMute;

    public bool isStartGame;


    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
        homePageAnimator = homePagePanel.GetComponent<Animator>();
    }

    public void SetUpHomePage() {
        isStartGame = false;
        isMute = false;

        SetUpOptionsButton();
        SetUpPlayerNameInput();
        SetUpWeaponShop();
        SetUpSkinShopButton();
        SetUpZombieModeButton();

        playButton.onClick.AddListener(() => {
            //SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);
            AudioManager.Instance.PlaySoundClickButton();

            leftPanel.SetActive(false);
            rightPanel.SetActive(false);
            playerNameInput.gameObject.SetActive(false);
            exp.SetActive(false);

            //Debug.Log("Play");
            joystick.gameObject.SetActive(true);
            isStartGame = true;
        });
        SetCoinText();
    }

    private void SetUpOptionsButton() {
        noAdsButton.onClick.AddListener(() => {
            AudioManager.Instance.PlaySoundClickButton();
            //
            if (!AdBlockPanel.gameObject.activeSelf)
                AdBlockPanel.gameObject.SetActive(true);
            homePagePanel.SetActive(false);
            playerNameInput.gameObject.SetActive(false);
        });
        exitAdBlockPanel.onClick.AddListener(() => {
            AdBlockPanel.gameObject.SetActive(false);
            homePagePanel.SetActive(true);
            playerNameInput.gameObject.SetActive(true);
        });
        volumeButton.onClick.AddListener(() => {
            if (!isMute) {
                //if (SoundManager.Instance != null)
                //    SoundManager.Instance.TurnOffSound();
                DataManager.Instance.playerData.soundVolume = 0;

                turnOnObject.gameObject.SetActive(false);
                turnOffObject.gameObject.SetActive(true);
                isMute = true;
            }
            else {
                //if (SoundManager.Instance != null)
                //    SoundManager.Instance.TurnOnSound();
                DataManager.Instance.playerData.soundVolume = 1;

                turnOnObject.gameObject.SetActive(true);
                turnOffObject.gameObject.SetActive(false);
                isMute = false;
            }
        });
        cheatCoinButton.onClick.AddListener(() => {
            DataManager.Instance.UpdatePlayerCoin(DataManager.Instance.cheatPlayerCoin);
            SetCoinText();
        });
    }

    private void SetUpPlayerNameInput() {
        playerNameInput.text = DataManager.Instance.playerData.playerName;

        playerNameInput.onValueChanged.AddListener((string name) => {
            DataManager.Instance.playerData.playerName = name;
        });
    }
    private void SetUpWeaponShop() {
        weaponShopButton.onClick.AddListener(() => {
            AudioManager.Instance.PlaySoundClickButton();
            //
            PlayerController.Instance.gameObject.SetActive(false);
            //WeaponShopController.Instance.LoadWeapon(DataManager.Instance.playerPersonalData.currentWeaponIndex);
            //
            playerNameInput.gameObject.SetActive(false);
            weaponShopPanel.gameObject.SetActive(true);
            HomePageOut();

        });

        exitWeaponButton.onClick.AddListener(() => {
            AudioManager.Instance.PlaySoundClickButton();
            //
            GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag(GameVariable.ENEMY_TAG);
            //
            PlayerController.Instance.gameObject.SetActive(true);
            PlayerController.Instance.LoadWeapon();
            //DataManager.Instance.TriggerUserChangeWeaponEvent();
            weaponShopPanel.gameObject.SetActive(false);
            playerNameInput.gameObject.SetActive(true);
            HomePageIn();
        });
    }
    private void SetUpSkinShopButton() {
        skinShopButton.onClick.AddListener(() => {
            //SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);
            AudioManager.Instance.PlaySoundClickButton();
            //
            skinShopPanel.gameObject.SetActive(true);
            playerNameInput.gameObject.SetActive(false);
            //
            PlayerController.Instance.SetPlayerDance();
            SkinShopController.Ins.SetUpOpenShop();
            CameraController.Instance.TurnOnPlayerShoppingCamera();
            HomePageOut();
        });

    }

    private void SetUpZombieModeButton() {
        zombieModeButton.onClick.AddListener(() => {
            AudioManager.Instance.PlaySoundClickButton();
            //
            SceneManager.LoadScene(GameVariable.zombieSplashSceneInName) ;
        });
        int day = DataManager.Instance.playerData.zombieDayVictory;
        dayZombieModeText.text = (day + 1).ToString();
    }

    public void ExitSkinShop() {
        skinShopPanel.gameObject.SetActive(false);
        playerNameInput.gameObject.SetActive(true);
        
        CameraController.Instance.TurnOffPlayerShoppingCamera();
        PlayerController.Instance.SetPlayerStopDance();
        HomePageIn();
    }

    public void SetCoinText() {
        coinText.text =  DataManager.Instance.playerData.coin.ToString();
    }

    public void HomePageOut() {
        homePageAnimator.Play("HomePage_Anim_Out");
    }

    public void HomePageIn() {
        homePageAnimator.Play("HomePage_Anim_In");
    }

}
