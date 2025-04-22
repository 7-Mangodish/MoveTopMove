using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ZombieUIController : MonoBehaviour
{
    private static ZombieUIController instance;
    public static ZombieUIController Instance { get => instance; }
    [SerializeField] private SkillObjects skillObjects;
    [SerializeField] private AbilitiesObjects abilitiesObjects;

    [Header("-----Bottom Panel-----")]
    [SerializeField] private GameObject bottomPanel;
    [SerializeField] private Button[] listSkillButtons;
    [SerializeField] private TextMeshProUGUI[] listCostSkillText;
    [SerializeField] private TextMeshProUGUI[] listSkillInforText;
    [SerializeField] private TextMeshProUGUI[] listSkillLevelText;

    [Header("-----Center Panel-----")]
    [SerializeField] private GameObject centerPanel;
    [SerializeField] private Button refuseCenterButton;
    [SerializeField] private Button changeAbilityButton_1;
    [SerializeField] private Button changeAbilityButton_2;
    [SerializeField] private Button selectAbilityButton;
    [SerializeField] private Button homeCenterButton;
    [SerializeField] private Image abilityImage;
    [SerializeField] private TextMeshProUGUI abilityNameText;
    [SerializeField] private TextMeshProUGUI faildUpgradeText;
    private List<int> listAbilitiesIndex;
    private int currentAbility;
    private bool isClickSelectAbilityButton;
    public static int choosenAbility;

    [Header("-----Top Panel-----")]
    [SerializeField] private GameObject topPanel;
    [SerializeField] private Image[] listHpImage;
    [SerializeField] private GameObject playerCoinPanel;
    [SerializeField] private TextMeshProUGUI playerCoinText;
    [SerializeField] private TextMeshProUGUI zombieRemainingText;
    [SerializeField] private TextMeshProUGUI currentZombieDayText;
    [SerializeField] private Button settingButton;

    [Header("-----End Panel------")]
    [SerializeField] private GameObject endPanel;
    [SerializeField] private Button claimCoinButton;
    [SerializeField] private Button homeEndPanelButton;
    [SerializeField] private Button x3CoinButton;
    [SerializeField] private TextMeshProUGUI x3CoinText;
    [SerializeField] private TextMeshProUGUI x2Text;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI winTittle;
    [SerializeField] private TextMeshProUGUI loseTittle;
    private int endGameCoin;

    [Header("-----Revive Panel------")]
    [SerializeField] private Button reviveButton;
    [SerializeField] private Button exitRevivePanelButton;
    [SerializeField] private GameObject revivePanel;
    public bool isRevived;
    private bool isCLickRevive;

    [Header("-----Zombie Day-----")]
    [SerializeField] private Image[] listZombieDayImage;
    [SerializeField] private Sprite dayVictoryStart;
    [SerializeField] private Sprite dayVictory;
    [SerializeField] private Sprite dayLoseStart;
    [SerializeField] private Sprite dayLose;

    [Header("-----Setting Panel-----")]
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private Button homeButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button soundOnButton;
    [SerializeField] private Button soundOffButton;
    [SerializeField] private Button vibrationOnButton;
    [SerializeField] private Button vibrationOffButton;

    [Header("-----Instuction Panel-----")]
    [SerializeField] private GameObject instructionPanel;

    private SkillData skillData;
    public bool isStartGame = false;
    public enum TypeSkill {
        none,
        hp,
        speed,
        range,
        weaponCount
    }


    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

    }

    private void OnEnable() {
        MaxManager.Instance.OnPlayerReceiveAward += ZombieUI_OnPlayerReceiveAward;
    }

    private void OnDisable() {
        MaxManager.Instance.OnPlayerReceiveAward -= ZombieUI_OnPlayerReceiveAward;
    }
    public void SetUpUIWhenStart() {
        choosenAbility = -1;
        skillData = DataManager.Instance.GetSkillData();

        SetUpTopPanel();
        SetUpCenterPanelButton();
        SetUpSettingPanel();
        SetUpListHpImage((int)skillData.shield);
        SetUpPlayerCoinText();

        SetUpSkillText();
        SetUpSkillButton();
        SetUpAbilityButton();

        SetUpRevivePanel();
        SetUpEndPanel();

        isCLickRevive = false;
        isClickSelectAbilityButton = false;

        bottomPanel.gameObject.SetActive(true);
        centerPanel.gameObject.SetActive(true);
        topPanel.gameObject.SetActive(true);
        playerCoinPanel.gameObject.SetActive(true);
        settingButton.gameObject.SetActive(false);


    }

    void SetUpCenterPanelButton() {
        refuseCenterButton.onClick.AddListener(() => {
            SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

            centerPanel.SetActive(false);
            bottomPanel.SetActive(false);
            playerCoinPanel.gameObject.SetActive(false);
            settingButton.gameObject.SetActive(true);
            isStartGame = true;
        });
        selectAbilityButton.onClick.AddListener(() => {
            SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);
            MaxManager.Instance.ShowRewardAd();

        });

        homeCenterButton.onClick.AddListener(() => {
            SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);
            SceneManager.LoadScene(GameVariable.normalSceneName);
        });
    }
    public void  SetUpTopPanel() {
        int day = DataManager.Instance.GetZombieDayVictory();
        currentZombieDayText.text = "Day "+(day+1).ToString();

        SetUpPlayerCoinText();

        settingButton.onClick.AddListener(() => {
            if (SoundManager.Instance != null) 
                SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

            settingPanel.gameObject.SetActive(true);
            Time.timeScale = 0;
        });
    }

    public void DisplayZombieCount(int zombieCount) {
        zombieRemainingText.text = zombieCount.ToString();
    }

    public void  SetUpListHpImage(int hpCount) {
        for(int i=0; i<listHpImage.Length; i++) {
            if (i < hpCount)
                listHpImage[i].gameObject.SetActive(true);
            else
                listHpImage[i].gameObject.SetActive(false);
        }
    }

    void SetUpAbilityButton() {
        listAbilitiesIndex = abilitiesObjects.GetRandomAbilities();
        abilityImage.sprite = abilitiesObjects.listAbilitySprite[listAbilitiesIndex[0]];
        abilityNameText.text = abilitiesObjects.listAbilitySprite[listAbilitiesIndex[0]].name; ;

        changeAbilityButton_1.onClick.AddListener(() => {
            SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

            currentAbility += 1;
            if (currentAbility >= listAbilitiesIndex.Count)
                currentAbility = 0;
            int abilityIndex = listAbilitiesIndex[currentAbility];
            abilityImage.sprite = abilitiesObjects.listAbilitySprite[abilityIndex];
            abilityNameText.text = abilitiesObjects.listAbilitySprite[abilityIndex].name;
        });

        changeAbilityButton_2.onClick.AddListener(() => {
            currentAbility += 1;
            if (currentAbility >= listAbilitiesIndex.Count)
                currentAbility = 0;
            int abilityIndex = listAbilitiesIndex[currentAbility];
            abilityImage.sprite = abilitiesObjects.listAbilitySprite[abilityIndex];
            abilityNameText.text = abilitiesObjects.listAbilitySprite[abilityIndex].name;
        });
    }

    #region -------------------Skill------------------------
    void SetUpSkillText() {
        skillData.InitPlayerSkill();
        for (int i=0; i<listSkillButtons.Length; i++) {
            int ind = i;
            switch (ind) {
                case 0: {
                    listSkillInforText[ind].text = (skillData.skillLevel[ind]).ToString() + " Times";
                        break;
                }
                case 1: {
                        listSkillInforText[ind].text = "+"+(skillData.skillLevel[ind] * 20).ToString() + "% Speed";
                        break;
                }
                case 2: {
                        listSkillInforText[ind].text = "+" + (skillData.skillLevel[ind] * 20).ToString() + "% Range";
                        break;
                }
                case 3: {
                        listSkillInforText[ind].text = "Max: " + (skillData.skillLevel[ind]).ToString();
                        break;
                }
            }

            listSkillLevelText[ind].text = "Level: " + skillData.skillLevel[ind].ToString();
            if (skillData.skillLevel[ind] == skillData.maxLevelSkill[ind]) {
                listSkillLevelText[ind].text = "Level: max";
                listSkillButtons[ind].gameObject.SetActive(false);
            }

            listCostSkillText[ind].text = skillData.skillCost[ind].ToString();

        }
    }
    void SetUpSkillButton() {
        for (int i = 0; i < listSkillButtons.Length; i++) {
            int ind = i;
            listSkillButtons[i].onClick.AddListener(() => {
                skillData = DataManager.Instance.GetSkillData();

                SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

                if (!CoinManager.Instance.PurchaseItem(skillData.skillCost[ind])) {
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
                            if (skillData.UpgradeHp()) {
                                //OnPlayerUpgradeSkill?.Invoke(this, TypeSkill.hp);
                                DataManager.Instance.SetSkillData(skillData);
                                SetUpListHpImage((int)skillData.shield);
                            }
                            break;
                        }
                    case 1: {
                            if(skillData.UpgradeSpeed())
                                DataManager.Instance.SetSkillData(skillData);
                                //OnPlayerUpgradeSkill?.Invoke(this, TypeSkill.speed);
                            break;
                        }
                    case 2: {
                            if (skillData.UpgradeRange()) {
                                DataManager.Instance.SetSkillData(skillData);
                                CameraController.Instance.SetUpgradeSkillRangeCamera();
                                PlayerController.Instance.SetUpAttackRange();
                            }
                                //OnPlayerUpgradeSkill?.Invoke(this, TypeSkill.range);
                            break;
                        }
                    case 3: {
                            if(skillData.UpgradeWeaponCount())
                                DataManager.Instance.SetSkillData(skillData);

                            //OnPlayerUpgradeSkill?.Invoke(this, TypeSkill.weaponCount);
                            break;
                        }
                }
                skillData.UpdateSkillCost();
                SetUpSkillText();
            });
        }

    }
    #endregion

    #region ------------------End_Game------------------------
    private void SetUpRevivePanel() {
        reviveButton.onClick.AddListener(() => {
            SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

            isCLickRevive = true;
            // Show Quang cao
            MaxManager.Instance.ShowRewardAd();
        });

        exitRevivePanelButton.onClick.AddListener(() => {
            SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

            endPanel.SetActive(true);
            revivePanel.SetActive(false);
        });
    }

    private void SetUpEndPanel() {
        homeEndPanelButton.onClick.AddListener(() => {
            SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

            SceneManager.LoadScene(GameVariable.normalSceneName);
        });
        claimCoinButton.onClick.AddListener(() => {
            SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

            CoinManager.Instance.SaveCoin(endGameCoin);
            SetUpPlayerCoinText();
            SceneManager.LoadScene(GameVariable.zombieSceneName);
        });
        x3CoinButton.onClick.AddListener(() => {
            MaxManager.Instance.ShowRewardAd();
        });
    }

    #endregion

    private void SetUpPlayerCoinText() {
        playerCoinText.text = PlayerPrefs.GetInt(GameVariable.PLAYER_COIN).ToString();
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
            DataManager.Instance.SetZombieDayVictory(currentDay);
        }

        else {
            if (currentDay == 1)
                listZombieDayImage[0].sprite = dayLoseStart;
            else if (currentDay == 5)
                listZombieDayImage[4].sprite = dayLoseStart;
            else
                listZombieDayImage[currentDay-1].sprite = dayLose;
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

        homeButton.onClick.AddListener(() => {
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);
            SceneManager.LoadScene(0);
        });

        continueButton.onClick.AddListener(() => {
            SoundManager.Instance.PlaySound(SoundManager.SoundName.button_click);

            settingPanel.gameObject.SetActive(false);
            Time.timeScale = 1;
        });
    }

    #region ---------------WIN_LOSE_UI----------------
    public void TurnOnWinUI() {
        if(endPanel != null)
            endPanel.gameObject.SetActive(true);
        winTittle.gameObject.SetActive(true);
        loseTittle.gameObject.SetActive(false);


        int dayZombie = DataManager.Instance.GetZombieDayVictory();
        SetUpZombieDay(dayZombie + 1, true);

        winTittle.text = "U suvived Day " + (dayZombie + 1).ToString();

        endGameCoin = CoinManager.Instance.GetWinCoin();
        coinText.text = endGameCoin.ToString();
        if (CoinManager.Instance.isDoubleAward)
            x2Text.gameObject.SetActive(true);
        else
            x2Text.gameObject.SetActive(false);
    }

    public async void TurnOnLoseUI() {
        winTittle.gameObject.SetActive(false);
        loseTittle.gameObject.SetActive(true);
        if (!isRevived) {
            revivePanel.gameObject.SetActive(true);
            await Task.Delay(5200);
            revivePanel.gameObject.SetActive(false);
            if(!isCLickRevive)
                endPanel.gameObject.SetActive(true);
        }
        else
            endPanel.gameObject.SetActive(true);

        int dayZombie = DataManager.Instance.GetZombieDayVictory(); ;
        SetUpZombieDay(dayZombie + 1, false);

        endGameCoin = CoinManager.Instance.GetLoseCoin();
        coinText.text = endGameCoin.ToString();
        x3CoinText.text = (endGameCoin * 3).ToString();
        if (CoinManager.Instance.isDoubleAward)
            x2Text.gameObject.SetActive(true);
        else
            x2Text.gameObject.SetActive(false);
    }
    #endregion
    private void ZombieUI_OnPlayerReceiveAward(object sender, MaxManager.TypeReward t) {
        if(!isRevived && isCLickRevive) {
            //GameController.Instance.DoPlayerRevive();
            isRevived = true;
            revivePanel.gameObject.SetActive(false);
        }
        if (!isClickSelectAbilityButton) {
            isClickSelectAbilityButton = true;
            centerPanel.SetActive(false);
            bottomPanel.SetActive(false);

            playerCoinPanel.gameObject.SetActive(false);
            settingButton.gameObject.SetActive(true);

            isStartGame = true;
            choosenAbility = listAbilitiesIndex[currentAbility];
            //OnPlayerChooseAbility?.Invoke(this, listAbilitiesIndex[currentAbility]);
        }
        if(t == MaxManager.TypeReward.x3Coin) {
            endGameCoin *= 3;

            CoinManager.Instance.SaveCoin(endGameCoin);
            SetUpPlayerCoinText();
            SceneManager.LoadScene(GameVariable.zombieSceneName);
        }

    }

    public IEnumerator ShowInstruction() {
        instructionPanel.SetActive(true);
        yield return new WaitUntil(() => PlayerController.Instance.startGame);
        instructionPanel.SetActive(false);
    }

}
