using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.InputManagerEntry;

public class StartPanelManager : MonoBehaviour
{
    private static StartPanelManager instance;
    public static StartPanelManager Instance { get => instance; }

    [SerializeField] private Button resetLevelButton;
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
    [SerializeField] private TextMeshProUGUI faildUpgradeText;
    private List<int> listAbilitiesIndex;

    int currentAbility;

    [Header("Top Panel")]
    [SerializeField] private Image[] listHpImage;
    [SerializeField] private TextMeshProUGUI playerCoinText;
    [SerializeField] private TextMeshProUGUI zombieRemainingText;

    [Header("Winning Panel")]
    [SerializeField] private GameObject winningPanel;

    [Header("Losing Panel")]
    [SerializeField] private GameObject revivePanel;
    [SerializeField] private Button exitRevivePanelButton;
    [SerializeField] private GameObject losingPanel;
    [SerializeField] private Button homeLosingPanelButton;

    public event EventHandler<SkillObjects.TypeSkill> OnPlayerUpgradeSkill;
    public event EventHandler OnStartZombieMode;
    public event EventHandler<int> OnPlayerChooseAbility;
    //public event EventHandler<int> OnPlayerSelectAbility;
    private void Awake() {
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

        resetLevelButton.onClick.AddListener(() => {
            skillObjects.resetLevel();
            SetUpSkillText();
        });
        refuseCenterButton.onClick.AddListener(() => {
            centerPanel.SetActive(false);
            bottomPanel.SetActive(false);

            OnStartZombieMode?.Invoke(this, EventArgs.Empty);
        });
        selectAbilityButton.onClick.AddListener(() => {
            centerPanel.SetActive(false);
            bottomPanel.SetActive(false);

            OnStartZombieMode?.Invoke(this, EventArgs.Empty);
            OnPlayerChooseAbility?.Invoke(this, listAbilitiesIndex[currentAbility]);
        });

        // Revive Panel
        exitRevivePanelButton.onClick.AddListener(() => {
            losingPanel.SetActive(true);
            revivePanel.SetActive(false);
        });

        // Losing Panel
        homeLosingPanelButton.onClick.AddListener(() => {
            SceneManager.LoadScene(0);
        });
    }

    public void Start() {
        zombieRemainingText.text = GameManager.Instance.GetMaxEnemyQuantity().ToString();
        GameManager.Instance.OnEnemyQuantityDown += StartPanelManager_OnEnemyQuantityDown;
        GameManager.Instance.OnPlayerWin += StartPanelManager_OnPlayerWin;
        GameManager.Instance.OnPlayerLose += StartPanelManager_OnPlayerLose;
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
            currentAbility += 1;
            if (currentAbility >= listAbilitiesIndex.Count)
                currentAbility = 0;
            int abilityIndex = listAbilitiesIndex[currentAbility];
            abilityImage.sprite = abilitiesObjects.listAbilitySprite[abilityIndex];

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
                SetUpSkillText();
            });
        }

    }
    #endregion

    private void SetUpPlayerCoinText() {
        playerCoinText.text = PlayerPrefs.GetInt("PlayerCoin").ToString();
    }

    private void StartPanelManager_OnEnemyQuantityDown(object sender, int e) {
        zombieRemainingText.text = e.ToString();
    }

    private void StartPanelManager_OnPlayerWin(object sender, EventArgs e) {
        winningPanel.gameObject.SetActive(true);
    }

    private void StartPanelManager_OnPlayerLose(object sender, EventArgs e) {
        revivePanel.gameObject.SetActive(true);
    }

    public void TriggerOnPlayerUpgradeSkill() {
        OnPlayerUpgradeSkill?.Invoke(this, SkillObjects.TypeSkill.weaponCount);
    }
}
