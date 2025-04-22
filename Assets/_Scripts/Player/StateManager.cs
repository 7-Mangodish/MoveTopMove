using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using Random = UnityEngine.Random;

public class StateManager : MonoBehaviour
{
    
    private int currentScore;
    public int CurrentScore { get { return currentScore; } }
    public bool IsLevelUp;

    [SerializeField] private SkillObjects skillObjects;

    [Header("-----Weapon's State-----")]
    [SerializeField] private float deltaScaleWeapon;
    [SerializeField] private GameObject maxDistancePoint;
    private ThrowWeapon.StateWeapon stateWeapon;

    [Header("Character's Animation Control")]
    private AnimationControl animationControl;

    [Header("-----Player's Scale-----")]
    [SerializeField] private TextMeshProUGUI playerScaleText;
    private int currentLevel;

    [Header("-----Zombie Mode-----")]
    [SerializeField] private GameObject playerShield;
    public bool isLevelUpZombieMode;
    public int enemyCount;

    public event EventHandler<int> OnCharacterTakeScore;
    public bool isDead = false;

    // Player
    private void Awake() {
        currentScore = 0;
        currentLevel = 1;
        enemyCount = currentLevel * 10;

        if(maxDistancePoint != null) {
            stateWeapon.ownerStateManager = this;
            stateWeapon.maxDistance = Vector3.Distance(this.transform.position, maxDistancePoint.transform.position);
            stateWeapon.curScale = 0;
            deltaScaleWeapon = 3;
        }
        else {
            Debug.Log("maxDistnance is null in " + this.gameObject.name);
        }

        animationControl  = GetComponent<AnimationControl>();
    }

    public void AddScore() {
        if (isDead) return;

        this.currentScore += 1;
        OnCharacterTakeScore?.Invoke(this, this.currentScore);

        if (SceneManager.GetActiveScene().name == GameVariable.normalSceneName) {
            if (currentScore % 10 == 0 && currentScore != 0)
                DoCharacterLevelUp();
        }
        else {
            if(currentScore % 10 == 0 && currentScore != 0) {
                if (currentScore % enemyCount == 0) {
                    DoCharaterLevelUpZombieMode();
                    currentLevel++;
                    enemyCount += (currentLevel * 10);
                }
            }
        }
    }

#region -----CHARACTER_LEVEL_UP-----
    public void DoCharacterLevelUp() {
        IsLevelUp = true;

        animationControl.PlayLevelUpEff();
        //Cap nhat scale cua nhan vat, cap nhat tam danh va scale cua vu khi
        this.transform.localScale += new Vector3(0.05f, 0.05f, 0.05f);
        if (this.gameObject.CompareTag(GameVariable.PLAYER_TAG)) {
            DisplayPlayerScale(this.transform.localScale.x * 5f);
        }

        DoUpdateStateWeapon();

        // Cap nhat cammera
        if (this.gameObject.CompareTag(GameVariable.PLAYER_TAG)) {
            CameraController.Instance.UpdateDistanceCamera(.75f);
            SoundManager.Instance.PlaySound(SoundManager.SoundName.size_up);
        }
    }
    public void DoCharaterLevelUpZombieMode() {
        isLevelUpZombieMode = true;
        DoUpdateStateWeaponZombieMode();
    }
#endregion


#region -----UPDATE_STATE_WEAPON-----
    public void DoUpdateStateWeapon() {
        stateWeapon.curScale += deltaScaleWeapon;
        stateWeapon.maxDistance = Vector3.Distance(this.transform.position, maxDistancePoint.transform.position);
    }

    public void DoUpdateStateWeaponZombieMode() {
        stateWeapon.maxDistance = Vector3.Distance(this.transform.position, maxDistancePoint.transform.position);
        if (this.gameObject.CompareTag("Player")) {
            CameraController.Instance.UpdateDistanceCamera(.75f);
            SoundManager.Instance.PlaySound(SoundManager.SoundName.size_up);
            PlayerController.Instance.playerAttackArea.transform.localScale += new Vector3(.5f, .5f, .5f);

        }
    }

#endregion


    public ThrowWeapon.StateWeapon GetStateWeapon() {
        return stateWeapon;
    }

    public async void TriggerCharacterDead() {      
        
        if(this.gameObject.CompareTag("Player") && SceneManager.GetActiveScene().name == GameVariable.zombieSceneName) {
            if (PlayerController.Instance.shield > 0) {
                PlayerController.Instance.shield -= 1;
                ZombieUIController.Instance.SetUpListHpImage((int)PlayerController.Instance.shield);

                playerShield.gameObject.SetActive(true);
                Physics.IgnoreLayerCollision(3, 7, true);
                await Task.Delay(1000);
                playerShield.gameObject.SetActive(false);
                Physics.IgnoreLayerCollision(3,7, false);
                return;
            }
            
        }

        //Animation
        animationControl.SetDead();

        // Event
        if (this.gameObject.CompareTag(GameVariable.PLAYER_TAG)) {
            this.gameObject.tag = GameVariable.DEAD_TAG;
            await Task.Delay(2000);
            this.gameObject.SetActive(false);
        }

        //Destroy Enemy
        if (this.gameObject.CompareTag(GameVariable.ENEMY_TAG)) {
            Destroy(this.transform.parent.gameObject, 2);
        }
        this.isDead = true;

    }

    public async void DisplayPlayerScale(float playerScale) {
        playerScaleText.gameObject.SetActive(true);
        playerScaleText.text = playerScale.ToString() + "m";
        playerScaleText.GetComponent<Animator>().Play("PlayerScaleText");
        await Task.Delay(1500);
        playerScaleText.gameObject.SetActive(false);
    }
}
