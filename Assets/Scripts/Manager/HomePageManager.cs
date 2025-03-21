using System;
using TMPro;
using UnityEngine;
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
    [SerializeField] private GameObject leftPanel;
    [SerializeField] private GameObject rightPanel;

    [SerializeField] private Button playButton;
    [SerializeField] private GameObject joystick;

    [Header("Coin")]
    [SerializeField] private TextMeshProUGUI coinText;
    public event EventHandler OnStartGame;
    public event EventHandler OnShopping;
    public event EventHandler OnOutShopping;
    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }
    void Start()
    {
        weaponShopButton.onClick.AddListener(() => {
            leftPanel.SetActive(false);
            rightPanel.SetActive(false);
            weaponShopPanel.gameObject.SetActive(true);

        });

        skinShopButton.onClick.AddListener(() => {
            leftPanel.SetActive(false);
            rightPanel.SetActive(false);
            skinShopPanel.gameObject.SetActive(true);

            OnShopping?.Invoke(this, EventArgs.Empty);
        });

        exitWeaponButton.onClick.AddListener(() => {
            weaponShopPanel.gameObject.SetActive(false);
            leftPanel.SetActive(true);
            rightPanel.SetActive(true);

            OnOutShopping?.Invoke(this, EventArgs.Empty);
        });

        exitSkinButton.onClick.AddListener(() => {
            skinShopPanel.gameObject.SetActive(false);
            leftPanel.SetActive(true);
            rightPanel.SetActive(true);

            OnOutShopping?.Invoke(this, EventArgs.Empty);
        });

        playButton.onClick.AddListener(() => {
            Debug.Log("Start Game");
            leftPanel.SetActive(false);
            rightPanel.SetActive(false);
            joystick.gameObject.SetActive(true);
            OnStartGame?.Invoke(this, EventArgs.Empty);
        });

        GameManager.Instance.OnPlayerWin += HomePage_OnPlayerWin;
        SetCoinText();
    }

    private void HomePage_OnPlayerWin(object sender, EventArgs e) {
        SetCoinText();
    }

    public void SetCoinText() {
        coinText.text =  PlayerPrefs.GetInt("PlayerCoin").ToString();
    }

    public void LoadHomepage() {
        leftPanel.SetActive(true);
        rightPanel.SetActive(true);
    }
}
