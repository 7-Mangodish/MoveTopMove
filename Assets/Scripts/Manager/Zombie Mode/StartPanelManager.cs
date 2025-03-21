using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputManagerEntry;

public class StartPanelManager : MonoBehaviour
{
    private static StartPanelManager instance;
    public static StartPanelManager Instance { get => instance; }

    [SerializeField] private Button resetLevelButton;

    [Header("Bottom Panel")]
    [SerializeField] private SkillObjects skillObjects;
    [SerializeField] private Button[] listSkillButtons;
    [SerializeField] private TextMeshProUGUI[] listCostSkillText;
    [SerializeField] private TextMeshProUGUI[] listSkillInforText;
    [SerializeField] private TextMeshProUGUI[] listSkillLevelText;

    [Header("Center Panel")]
    [SerializeField] private AbilitiesObjects abilitiesObjects;
    [SerializeField] private GameObject centerPanel;
    [SerializeField] private Button exitCenterButton;
    [SerializeField] private Button changeAbilityButton;
    [SerializeField] private Image abilityImage;
    private List<int> listAbilitiesIndex;
    int currentAbility;

    public event EventHandler<SkillObjects.TypeSkill> OnPlayerUpgradeSkill;
    public event EventHandler OnStartZombieMode;
    public event EventHandler<int> OnPlayerSelectAbility;
    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        //SetUpAbilityButton
        SetUpAbilityButton();

        // Skill
        SetUpSkillText();
        SetUpSkillButton();

        resetLevelButton.onClick.AddListener(() => {
            skillObjects.resetLevel();
            SetUpSkillText();
        });
        exitCenterButton.onClick.AddListener(() => {
            centerPanel.SetActive(false);
            OnStartZombieMode?.Invoke(this, EventArgs.Empty);
        });
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
        for(int i=0; i<listSkillButtons.Length; i++) {
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
                switch (ind) {
                    case 0: {
                            if(skillObjects.UpgradeHp())
                                OnPlayerUpgradeSkill?.Invoke(this, SkillObjects.TypeSkill.hp);
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
}
