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
    [SerializeField] private TextMeshProUGUI coinText;
    public event EventHandler OnStartGame;
    public event EventHandler OnShopping;
    public event EventHandler OnOutShopping;

    [Header("Player's Name")]
    [SerializeField] private TMP_InputField playerNameInput;

    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
        homePageAnimator = homePagePanel.GetComponent<Animator>();
    }
    void Start()
    {
        SetUpWeaponShop();
        SetUpSkinShop();
        SetUpZombieModeButton();

        playButton.onClick.AddListener(() => {
            SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

            leftPanel.SetActive(false);
            rightPanel.SetActive(false);
            joystick.gameObject.SetActive(true);
            OnStartGame?.Invoke(this, EventArgs.Empty);
        });
        playerNameInput.text = PlayerPrefs.GetString("PlayerName");
        playerNameInput.onValueChanged.AddListener((string name) => {
            PlayerPrefs.SetString("PlayerName", name);
        });

        GameManager.Instance.OnPlayerWin += HomePage_OnPlayerWin;
        SetCoinText();
    }


    private void SetUpWeaponShop() {
        weaponShopButton.onClick.AddListener(() => {
            SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

            leftPanel.SetActive(false);
            rightPanel.SetActive(false);
            weaponShopPanel.gameObject.SetActive(true);

        });

        exitWeaponButton.onClick.AddListener(() => {
            SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

            weaponShopPanel.gameObject.SetActive(false);
            leftPanel.SetActive(true);
            rightPanel.SetActive(true);

            OnOutShopping?.Invoke(this, EventArgs.Empty);
        });
    }
    private void SetUpSkinShop() {
        skinShopButton.onClick.AddListener(() => {
            SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

            skinShopPanel.gameObject.SetActive(true);

            OnShopping?.Invoke(this, EventArgs.Empty);
            HomePageOut();
        });


        exitSkinButton.onClick.AddListener(() => {
            SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

            skinShopPanel.gameObject.SetActive(false);
            OnOutShopping?.Invoke(this, EventArgs.Empty);
            HomePageIn();
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

    public void SetCoinText() {
        coinText.text =  PlayerPrefs.GetInt("PlayerCoin").ToString();
    }

    public void HomePageOut() {
        homePageAnimator.Play("HomePage_Anim_Out");
    }

    public void HomePageIn() {
        homePageAnimator.Play("HomePage_Anim_In");
    }
}
