using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomePageManager : MonoBehaviour
{
    private static HomePageManager instance;
    public static HomePageManager Instance { get => instance; }

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
    [SerializeField] private TMP_InputField playerNameInput;

    [Header("Options Button")]
    [SerializeField] private GameObject AdBlockPanel;
    [SerializeField] private Button noAdsButton;
    [SerializeField] private Button exitAdBlockPanel;

    [SerializeField] private Button vibrationButton;

    [SerializeField] private Button volumnButton;
    [SerializeField] private GameObject turnOnObject;
    [SerializeField] private GameObject turnOffObject;
    private bool isMute;

    public event EventHandler OnStartGame;
    public event EventHandler OnShopping;
    public event EventHandler OnOutShopping;

    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
        homePageAnimator = homePagePanel.GetComponent<Animator>();
    }
    void Start()
    {
        SetUpOptionsButton();
        SetUpPlayerNameInput();
        SetUpWeaponShop();
        SetUpSkinShopButton();
        SetUpZombieModeButton();

        playButton.onClick.AddListener(() => {
            SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

            leftPanel.SetActive(false);
            rightPanel.SetActive(false);
            playerNameInput.gameObject.SetActive(false);
            exp.SetActive(false);

            joystick.gameObject.SetActive(true);
            OnStartGame?.Invoke(this, EventArgs.Empty);
        });


        GameManager.Instance.OnPlayerWin += HomePage_OnPlayerWin;
        SetCoinText();
        isMute = false;
    }

    private void SetUpOptionsButton() {
        noAdsButton.onClick.AddListener(() => {
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

        volumnButton.onClick.AddListener(() => {
            if (!isMute) {
                if (SoundManager.Instance != null)
                    SoundManager.Instance.TurnOffSound();

                turnOnObject.gameObject.SetActive(false);
                turnOffObject.gameObject.SetActive(true);
                isMute = true;
            }
            else {
                if (SoundManager.Instance != null)
                    SoundManager.Instance.TurnOnSound();

                turnOnObject.gameObject.SetActive(true);
                turnOffObject.gameObject.SetActive(false);
                isMute = false;
            }
        });
    }

    private void SetUpPlayerNameInput() {
        if (!PlayerPrefs.HasKey("PlayerName"))
            PlayerPrefs.SetString("PlayerName", "You");
        playerNameInput.text = PlayerPrefs.GetString("PlayerName");
        playerNameInput.onValueChanged.AddListener((string name) => {
            PlayerPrefs.SetString("PlayerName", name);
        });
    }
    private void SetUpWeaponShop() {
        weaponShopButton.onClick.AddListener(() => {
            SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

            leftPanel.SetActive(false);
            rightPanel.SetActive(false);
            playerNameInput.gameObject.SetActive(false);

            weaponShopPanel.gameObject.SetActive(true);

        });

        exitWeaponButton.onClick.AddListener(() => {
            SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

            weaponShopPanel.gameObject.SetActive(false);
            leftPanel.SetActive(true);
            rightPanel.SetActive(true);
            playerNameInput.gameObject.SetActive(true);

            OnOutShopping?.Invoke(this, EventArgs.Empty);
        });
    }
    private void SetUpSkinShopButton() {
        skinShopButton.onClick.AddListener(() => {
            SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

            skinShopPanel.gameObject.SetActive(true);
            playerNameInput.gameObject.SetActive(false);

            OnShopping?.Invoke(this, EventArgs.Empty);
            HomePageOut();
        });

    }

    private void SetUpZombieModeButton() {
        zombieModeButton.onClick.AddListener(() => {
            SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

            SceneManager.LoadScene(1);
        });
        int day = PlayerPrefs.GetInt("ZombieDayVictory");
        dayZombieModeText.text = (day + 1).ToString();
    }
    private void HomePage_OnPlayerWin(object sender, EventArgs e) {
        SetCoinText();
    }
    public void ExitSkinShop() {
        skinShopPanel.gameObject.SetActive(false);
        playerNameInput.gameObject.SetActive(true);

        OnOutShopping?.Invoke(this, EventArgs.Empty);
        HomePageIn();
    }

    public void SetCoinText() {
        if (!PlayerPrefs.HasKey("PlayerCoin"))
            PlayerPrefs.SetInt("PlayerCoin", 500);
        coinText.text =  PlayerPrefs.GetInt("PlayerCoin").ToString();
    }

    public void HomePageOut() {
        homePageAnimator.Play("HomePage_Anim_Out");
    }

    public void HomePageIn() {
        homePageAnimator.Play("HomePage_Anim_In");
    }
}
