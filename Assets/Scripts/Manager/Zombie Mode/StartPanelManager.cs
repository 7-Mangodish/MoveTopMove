using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.InputManagerEntry;

public class StartPanelManager : MonoBehaviour
{
    private static StartPanelManager instance;
    public static StartPanelManager Instance { get => instance; }
    [SerializeField] private SkillObjects skillObjects;
    [SerializeField] private AbilitiesObjects abilitiesObjects;

    [Header("Bottom Panel")]
    [SerializeField] private GameObject bottomPanel;
    [SerializeField] private Button[] listSkillButtons;
    [SerializeField] private TextMeshProUGUI[] listCostSkillText;
    [SerializeField] private TextMeshProUGUI[] listSkillInforText;
    [SerializeField] private TextMeshProUGUI[] listSkillLevelText;

    [Header("Center Panel")]
    [SerializeField] private GameObject centerPanel;
    [SerializeField] private Button refuseCenterButton;
    [SerializeField] private Button changeAbilityButton;
    [SerializeField] private Button selectAbilityButton;
    [SerializeField] private Image abilityImage;
    [SerializeField] private TextMeshProUGUI abilityNameText;
    [SerializeField] private TextMeshProUGUI faildUpgradeText;
    private List<int> listAbilitiesIndex;

    int currentAbility;

    [Header("Top Panel")]
    [SerializeField] private GameObject topPanel;
    [SerializeField] private Image[] listHpImage;
    [SerializeField] private GameObject playerCoinPanel;
    [SerializeField] private TextMeshProUGUI playerCoinText;
    [SerializeField] private TextMeshProUGUI zombieRemainingText;
    [SerializeField] private TextMeshProUGUI currentZombieDayText;
    [SerializeField] private Button settingButton;

    [Header("End Panel")]
    [SerializeField] private GameObject endPanel;
    [SerializeField] private Button claimCoinButton;
    [SerializeField] private Button homeEndPanelButton;
    [SerializeField] private TextMeshProUGUI x2Text;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI winTittle;
    [SerializeField] private TextMeshProUGUI loseTittle;

    [Header("Revive Panel")]
    [SerializeField] private Button reviveButton;
    [SerializeField] private Button exitRevivePanelButton;
    [SerializeField] private GameObject revivePanel;

    [Header("Zombie Day")]
    [SerializeField] private Image[] listZombieDayImage;
    [SerializeField] private Sprite dayVictoryStart;
    [SerializeField] private Sprite dayVictory;
    [SerializeField] private Sprite dayLoseStart;
    [SerializeField] private Sprite dayLose;

    [Header("Setting Panel")]
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private Button homeButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button soundOnButton;
    [SerializeField] private Button soundOffButton;
    [SerializeField] private Button vibrationOnButton;
    [SerializeField] private Button vibrationOffButton;

    public event EventHandler<SkillObjects.TypeSkill> OnPlayerUpgradeSkill;
    public event EventHandler OnStartZombieMode;
    public event EventHandler<int> OnPlayerChooseAbility;
    public event EventHandler OnTurnOnSetting;
    public event EventHandler OnTurnOffSetting;
    //public event EventHandler<int> OnPlayerSelectAbility;
    private void Awake() {
        Debug.Log(PlayerPrefs.GetInt("ZombieDayVictory"));
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);


        // Player's Coin
        SetUpPlayerCoinText();

        //SetUpListImageHp
        SetUpListHpImage();

        //SetUpAbilityButton
        SetUpAbilityButton();

        // Skill
        SetUpSkillText();
        SetUpSkillButton();

        refuseCenterButton.onClick.AddListener(() => {
            SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

            centerPanel.SetActive(false);
            bottomPanel.SetActive(false);
            playerCoinPanel.gameObject.SetActive(false);
            settingButton.gameObject.SetActive(true);

            OnStartZombieMode?.Invoke(this, EventArgs.Empty);
        });
        selectAbilityButton.onClick.AddListener(() => {
            SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

            centerPanel.SetActive(false);
            bottomPanel.SetActive(false);

            playerCoinPanel.gameObject.SetActive(false);
            settingButton.gameObject.SetActive(true);

            OnStartZombieMode?.Invoke(this, EventArgs.Empty);
            OnPlayerChooseAbility?.Invoke(this, listAbilitiesIndex[currentAbility]);
        });

        // Revive Panel
        SetUpRevivePanel();

        // ending Panel
        homeEndPanelButton.onClick.AddListener(() => {
            SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

            SceneManager.LoadScene(0);
        });
        claimCoinButton.onClick.AddListener(() => {
            SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

            CoinManager.Instance.SaveCoin();
            SetUpPlayerCoinText();
            SceneManager.LoadScene(1);
        });

        // Setting Panel
    }

    public void Start() {
        GameManager.Instance.OnEnemyQuantityDown += StartPanelManager_OnEnemyQuantityDown;
        GameManager.Instance.OnPlayerWin += StartPanelManager_OnPlayerWin;
        GameManager.Instance.OnPlayerLose += StartPanelManager_OnPlayerLose;
        SetUpUIWhenStart();
        SetUpSettingPanel();
    }

    private void SetUpUIWhenStart() {
        bottomPanel.gameObject.SetActive(true);
        centerPanel.gameObject.SetActive(true);
        topPanel.gameObject.SetActive(true);
        playerCoinPanel.gameObject.SetActive(true);
        settingButton.gameObject.SetActive(false);
        SetUpTopPanel();
    }

    public void  SetUpTopPanel() {
        zombieRemainingText.text = GameManager.Instance.GetMaxEnemyQuantity().ToString();
        currentZombieDayText.text = "Day "+(PlayerPrefs.GetInt("ZombieDayVictory")+1).ToString();
        SetUpPlayerCoinText();
        settingButton.onClick.AddListener(() => {
            SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

            settingPanel.gameObject.SetActive(true);
            Time.timeScale = 0;
            OnTurnOnSetting?.Invoke(this, EventArgs.Empty);
        });
        continueButton.onClick.AddListener(() => {
            SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

            settingPanel.gameObject.SetActive(false);
            Time.timeScale = 1;
            OnTurnOffSetting?.Invoke(this, EventArgs.Empty);
        });
    }
    public void  SetUpListHpImage() {
        for(int i=0; i<listHpImage.Length; i++) {
            if (i < skillObjects.shield)
                listHpImage[i].gameObject.SetActive(true);
            else
                listHpImage[i].gameObject.SetActive(false);
        }
    }

    void SetUpAbilityButton() {
        listAbilitiesIndex = abilitiesObjects.GetRandomAbilities();
        abilityImage.sprite = abilitiesObjects.listAbilitySprite[listAbilitiesIndex[0]];

        changeAbilityButton.onClick.AddListener(() => {
            SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

            currentAbility += 1;
            if (currentAbility >= listAbilitiesIndex.Count)
                currentAbility = 0;
            int abilityIndex = listAbilitiesIndex[currentAbility];
            abilityImage.sprite = abilitiesObjects.listAbilitySprite[abilityIndex];
            abilityNameText.text = abilitiesObjects.listAbilitySprite[abilityIndex].name;
        });

    }

    #region Skill
    void SetUpSkillText() {
        skillObjects.InitPlayerSkill();
        for (int i=0; i<listSkillButtons.Length; i++) {
            int ind = i;
            switch (ind) {
                case 0: {
                    listSkillInforText[ind].text = (skillObjects.currentLevelSkill[ind]).ToString() + " Times";
                        break;
                }
                case 1: {
                        listSkillInforText[ind].text = "+"+(skillObjects.currentLevelSkill[ind]*20).ToString() + "% Speed";
                        break;
                }
                case 2: {
                        listSkillInforText[ind].text = "+" + (skillObjects.currentLevelSkill[ind]*20).ToString() + "% Range";
                        break;
                }
                case 3: {
                        listSkillInforText[ind].text = "Max: " + (skillObjects.currentLevelSkill[ind]).ToString();
                        break;
                }
            }

            listSkillLevelText[ind].text = "Level: " + skillObjects.currentLevelSkill[ind].ToString();
            if (skillObjects.currentLevelSkill[ind] == skillObjects.maxLevelSkill[ind]) {
                listSkillLevelText[ind].text = "Level: max";
                listSkillButtons[ind].gameObject.SetActive(false);
            }

            listCostSkillText[ind].text = skillObjects.costUpdateSkill[ind].ToString();

        }
    }
    void SetUpSkillButton() {
        for (int i = 0; i < listSkillButtons.Length; i++) {
            int ind = i;
            listSkillButtons[i].onClick.AddListener(() => {
                SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

                if (!CoinManager.Instance.PurchaseItem(skillObjects.costUpdateSkill[ind])) {
                    Debug.Log("Khong du tien");
                    if(faildUpgradeText.gameObject.activeSelf)
                        faildUpgradeText.gameObject.SetActive(false);
                    faildUpgradeText.gameObject.SetActive(true);
                    return;
                }
                else {
                    SetUpPlayerCoinText();
                }

                switch (ind) {
                    case 0: {
                            if (skillObjects.UpgradeHp()) {
                                OnPlayerUpgradeSkill?.Invoke(this, SkillObjects.TypeSkill.hp);
                                SetUpListHpImage();
                            }
                            break;
                        }
                    case 1: {
                            if(skillObjects.UpgradeSpeed())
                                OnPlayerUpgradeSkill?.Invoke(this, SkillObjects.TypeSkill.speed);
                            break;
                        }
                    case 2: {
                            if(skillObjects.UpgradeRange())
                                OnPlayerUpgradeSkill?.Invoke(this, SkillObjects.TypeSkill.range);
                            break;
                        }
                    case 3: {
                            if(skillObjects.UpgradeWeaponCount())
                                OnPlayerUpgradeSkill?.Invoke(this, SkillObjects.TypeSkill.weaponCount);
                            break;
                        }
                }
                skillObjects.UpdateSkillCost();
                SetUpSkillText();
            });
        }

    }
    #endregion

    private void SetUpPlayerCoinText() {
        playerCoinText.text = PlayerPrefs.GetInt("PlayerCoin").ToString();
    }

    private void SetUpRevivePanel() {
        reviveButton.onClick.AddListener(() => {
            SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

            GameManager.Instance.DoPlayerRevive();
            revivePanel.gameObject.SetActive(false);    
        });

        exitRevivePanelButton.onClick.AddListener(() => {
            SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

            endPanel.SetActive(true);
            revivePanel.SetActive(false);
        });
    }

    private void SetUpZombieDay(int currentDay, bool isVictory) {
        if(currentDay > 5)
            currentDay %= 5;

        listZombieDayImage[currentDay-1].gameObject.SetActive(true);
        for(int i = 1; i<=currentDay-1; i +=1) {
            listZombieDayImage[i - 1].gameObject.SetActive(true);
            if (i == 1)
                listZombieDayImage[i-1].sprite = dayVictoryStart;
            else
                listZombieDayImage[i-1].sprite = dayVictory;
        }
        if(isVictory) { 
            if(currentDay == 1)
                listZombieDayImage[0].sprite = dayVictoryStart;
            else if(currentDay ==5 )
                listZombieDayImage[4].sprite = dayVictoryStart;
            else
                listZombieDayImage[currentDay - 1].sprite = dayVictory;
            Debug.Log(currentDay + " " + listZombieDayImage[currentDay-1].sprite.name);
            PlayerPrefs.SetInt("ZombieDayVictory", currentDay);
        }
        else {
            if (currentDay == 1)
                listZombieDayImage[0].sprite = dayLoseStart;
            else if (currentDay == 5)
                listZombieDayImage[4].sprite = dayLoseStart;
            else
                listZombieDayImage[currentDay-1].sprite = dayLose;
            Debug.Log(listZombieDayImage[currentDay-1].sprite.name);
        }

    }

    private void SetUpSettingPanel() {
        soundOnButton.gameObject.SetActive(true);
        soundOffButton.gameObject.SetActive(false);

        soundOnButton.onClick.AddListener(() => {
            soundOnButton.gameObject.SetActive(false);
            soundOffButton.gameObject.SetActive(true);
            SoundManager.Instance.TurnOffSound();
        });

        soundOffButton.onClick.AddListener(() => {
            soundOffButton.gameObject.SetActive(false);
            soundOnButton.gameObject.SetActive(true);
            SoundManager.Instance.TurnOnSound();
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
    private void StartPanelManager_OnEnemyQuantityDown(object sender, int e) {
        zombieRemainingText.text = e.ToString();
    }

    private void StartPanelManager_OnPlayerWin(object sender, EventArgs e) {
        endPanel.gameObject.SetActive(true);
        winTittle.gameObject.SetActive(true);
        loseTittle.gameObject.SetActive(false);


        int dayZombie = PlayerPrefs.GetInt("ZombieDayVictory");
        SetUpZombieDay(dayZombie+1, true);

        winTittle.text = "U suvived Day " +(dayZombie+1).ToString();

        coinText.text = CoinManager.Instance.GetWinCoin().ToString();
        if (CoinManager.Instance.isDoubleAward)
            x2Text.gameObject.SetActive(true);
        else 
            x2Text.gameObject.SetActive(false);
    }

    private void StartPanelManager_OnPlayerLose(object sender, EventArgs e) {
        winTittle.gameObject.SetActive(false);
        loseTittle.gameObject.SetActive(true);
        revivePanel.gameObject.SetActive(true);

        int dayZombie = PlayerPrefs.GetInt("ZombieDayVictory");
        SetUpZombieDay(dayZombie + 1, false);
    }

    public void TriggerOnPlayerUpgradeSkill() {
        OnPlayerUpgradeSkill?.Invoke(this, SkillObjects.TypeSkill.weaponCount);
    }
}
