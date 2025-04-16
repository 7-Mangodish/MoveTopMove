using System;
using System.Collections;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private static PlayerController instance;
    public static PlayerController Instance { get => instance; }

    [SerializeField] WeaponObjects weaponObjects;
    private SkillDataManager.SkillData skillData;

    [Header("Skill")]
    [SerializeField] private float speed;
    [SerializeField] private GameObject playerAttackArea;
    public SphereCollider playerAttackAreaCollider;
    private float shield;
    private float range;
    private float weaponQuantity;
    private float bonusWeaponQuantity;
    private int weaponMultipleAbility;

    [Header("Move")]
    [SerializeField] private float deltaAngle;
    [SerializeField] private Joystick joystick;
    private Vector3 direct;
    private Rigidbody rb;
    private AnimationControl animationControl;

    [Header("Attack")]
    [SerializeField] private GameObject weapon;
    [SerializeField] private GameObject weaponHold;
    [SerializeField] private Transform weaponSpawnPosition;
    [SerializeField] private float speedWeapon;
    [SerializeField] private float timeAttack;
    [SerializeField] private float angleAttack;
    private float angleSpread;
    private float timeAttackDuration;
    private Vector3 targetPosition;
    private Vector3 directEnemy;
    private bool canAttack;

    [Header("Level Up")]
    private StateManager stateManager;
    private ThrowWeapon.StateWeapon stateWeapon;
    private bool isDead;
    private bool startGame;

    /*Ability*/
    public event EventHandler OnPlayerAttack;
    private PlayerAbility playerAbility;

    //Ability3
    private bool isAbility3;

    // Ability4
    private bool isAbility4;

    // Ability 8
    private bool isAbility8;

    // Ability 9
    private bool isAbility9;

    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        rb = GetComponent<Rigidbody>();
        animationControl = GetComponent<AnimationControl>();
        stateManager = GetComponent<StateManager>();
        playerAbility = GetComponent<PlayerAbility>();

        startGame = false;
        isDead = false;
        weaponQuantity = 1;
        weaponMultipleAbility = 1;
    }

    private void Start() {

        if(SceneManager.GetActiveScene().name == GameVariable.zombieSceneName) {
            playerAbility.OnPlayerChooseAbility2 += PlayerController_OnPlayerChooseAbility2;
            playerAbility.OnPlayerChooseAbility3 += PlayerController_OnPlayerChooseAbility3;
            playerAbility.OnplayerChooseAbility4 += PlayerController_OnplayerChooseAbility4;
            playerAbility.OnPlayerChooseAbility8 += PlayerController_OnPlayerChooseAbility8;
            playerAbility.OnPlayerChooseAbility9 += PlayerController_OnPlayerChooseAbility9;
            playerAbility.OnPlayerChooseAbility10 += PlayerController_OnPlayerChooseAbility10;
            playerAbility.OnPlayerChooseAbility11 += PlayerController_OnPlayerChooseAbility11;
            playerAbility.OnPlayerChooseAbility12 += PlayerAbility_OnPlayerChooseAbility12;
            playerAbility.OnPlayerChooseAbility13 += PlayerController_OnPlayerChooseAbility13;
        }

        GameController.Instance.OnPlayerRevive += PlayerController_OnPlayerRevive;
        //stateManager.OnCharacterDead += PlayerController_OnCharacterDead;


        timeAttackDuration = 3;
        stateWeapon = stateManager.GetStateWeapon();

        if(SceneManager.GetActiveScene().name == GameVariable.normalSceneName) {
            DataManager.Instance.OnUserChangeWeapon += PlayerController_OnUserChangeWeapon;
            HomePageController.Instance.OnShopping += PlayerController_OnShopping;
            HomePageController.Instance.OnOutShopping += PlayerController_OnOutShopping;
        }
        else if(SceneManager.GetActiveScene().name == GameVariable.zombieSceneName) {
            ReferenceToObject();
            StartPanelManager.Instance.OnPlayerUpgradeSkill += PlayerController_OnPlayerUpgradeSkill;
        }

    }

    public void GetPlayerInput() {
        if (!startGame && joystick.Horizontal != 0) {
            startGame = true;
            IntructionPanelManager.Instance.TurnOffPanel();
        }
        if (isDead)
            return;
        direct.x = joystick.Horizontal;
        direct.z = joystick.Vertical;
        RotateCharacter(direct);
    }

    public void PlayerBehaviour() {
        if (timeAttackDuration < 5)
            timeAttackDuration += Time.fixedDeltaTime;

        if (direct.normalized != Vector3.zero) {
            animationControl.SetRun();
            rb.MovePosition(rb.position + direct.normalized * speed * Time.fixedDeltaTime);
            if (!canAttack)
                canAttack = true;
        }
        else {
            animationControl.SetIdle();

            if (timeAttackDuration > timeAttack)
                canAttack = true;
            if (!canAttack)
                return;

            if (DoCheckEnemyInside()) {
                timeAttackDuration = 0;
                canAttack = false;
                Debug.Log("Attack");

                if (!isAbility4) {
                    directEnemy = targetPosition - this.transform.position;
                    directEnemy.y = 0;
                    RotateCharacter(directEnemy);
                    animationControl.SetAttack();

                }
                else {
                    directEnemy = targetPosition - this.transform.position;
                    directEnemy.y = 0;
                    RotateCharacter(directEnemy);
                    StartCoroutine(AttackTwice());
                }
            }
        }
    }

    private bool DoCheckEnemyInside() {
        float radius = playerAttackAreaCollider.radius * playerAttackAreaCollider.transform.lossyScale.x;
        Collider[] hitCollider = Physics.OverlapSphere(this.transform.position, radius );
        foreach(Collider hit in  hitCollider) {
            if(hit.gameObject.CompareTag("Enemy") || hit.gameObject.CompareTag("Zombie")) {
                targetPosition = hit.transform.position;
                Debug.Log(hit.gameObject);
                return true;
            }
        }
        return false;
    }
    private async void Attack() {
        if (isDead)
            return;

        SoundManager.Instance.PlaySound(SoundManager.SoundName.weapon_throw);

        weaponHold.gameObject.SetActive(false);

        // Kiem tra xem da len cap chua, neu co thi cap nhat lai trang thai cua vu khi
        if (stateManager.IsLevelUp) {
            stateWeapon = stateManager.GetStateWeapon();
            stateManager.IsLevelUp = false;
        }
        if (stateManager.isLevelUpZombieMode) {
            stateWeapon = stateManager.GetStateWeapon();
            playerAttackArea.transform.localScale += new Vector3(.5f, .5f, .5f);

            stateManager.isLevelUpZombieMode = false;
            if (weaponQuantity < bonusWeaponQuantity + 1)
                weaponQuantity +=1;
        }

        // Nhan voi weaponMultipleAbility neu la ability
        weaponQuantity *= weaponMultipleAbility;

        // Tan cong va goi su kien
        angleSpread = angleAttack / weaponQuantity;
        int startIndexWeapon = (int) - weaponQuantity / 2;
        OnPlayerAttack?.Invoke(this, EventArgs.Empty);
      
        for (int i = startIndexWeapon; i <= weaponQuantity /2; i++) {
            if (i == 0 && weaponQuantity % 2 == 0)
                continue;
            // Xac dinh vi tri spawn va huong nem
            Vector3 positionSpawn = new Vector3(weaponSpawnPosition.position.x,
                weaponSpawnPosition.position.y, weaponSpawnPosition.position.z);

            Vector3 directWeapon = Quaternion.AngleAxis(startIndexWeapon * angleSpread, Vector3.up) * directEnemy;   

            //Khoi tao vu khi
            GameObject weaponSpawn = Instantiate(weapon, positionSpawn, Quaternion.Euler(new Vector3(90, 0, 0)));
            weaponSpawn.transform.localScale = new Vector3(9, 9, 9);

            //Set trang thai(chu the, tam ban, scale, vi tri khoi tao)
            stateWeapon.positionSpawn = positionSpawn;
            Rigidbody weaponRb = weaponSpawn.GetComponent<Rigidbody>();
            ThrowWeapon weaponThrow = weaponRb.GetComponent<ThrowWeapon>();

            // Kiem tra xem player co chon ability8 hay khong
            if (isAbility8) {
                weaponThrow.isGrowing = true;
                stateWeapon.maxDistance += .5f;
            }
            // Kiem tra xem player co chon ability9 hay khong
            if (isAbility9) {
                weaponThrow.isPiering = true;
            }

            weaponRb.GetComponent<ThrowWeapon>().SetStateWeapon(stateWeapon);

            //Nem vu khi theo huong
            if (weaponRb != null) {
                weaponRb.linearVelocity = directWeapon.normalized * speedWeapon * Time.fixedDeltaTime;
            }
            startIndexWeapon++;
        }
        await Task.Delay(800);
        weaponHold.gameObject.SetActive(true);

    }

    private void EndAttack() {
        animationControl.SetEndAttack();
    }
    private void RotateCharacter(Vector3 direct) {
        if (direct == Vector3.zero)
            return;

        Quaternion rot = Quaternion.LookRotation(direct.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(this.transform.rotation, rot, deltaAngle);
    }
    
    // Duoc goi khi tham chieu den SkillObjects de lay chi so
    private void ReferenceToObject() {
        skillData = SkillDataManager.Instance.GetSkillData();
        shield = skillData.shield;
        speed = skillData.speed;
        range = skillData.range;
        playerAttackArea.transform.localScale = new Vector3(range, range, range);
        bonusWeaponQuantity = skillData.weaponBonus;
    }

    private void SetUpWeaponMaterial(DataManager.OnUserChangeWeaponArg e) {
        if (!PlayerPrefs.HasKey("CurWeapon")) {
            PlayerPrefs.SetInt("CurWeapon", 0);
            Debug.LogWarning("Chua co key: CurWeapon");
        }
        int curWeapon = PlayerPrefs.GetInt("CurWeapon");

        WeaponData data = DataManager.Instance.GetWeaponData(curWeapon);
        Mesh mesh = weaponObjects.GetMeshWeapon(curWeapon, data.skinIndex);
        weaponHold.GetComponent<MeshFilter>().mesh = mesh;
        weapon.GetComponent<MeshFilter>().mesh = mesh;

        int materialCount = weaponObjects.GetListMaterials(curWeapon, 2).Length;
        Material[] materials = new Material[materialCount];

        if (e.skinIndex != 0) {
            materials = weaponObjects.GetListMaterials(curWeapon, data.skinIndex);
            // weaponHold.GetComponent<MeshRenderer>().materials = materials;
        }
        else {
            materials = e.materials;
            //weaponHold.GetComponent<MeshRenderer>().materials = materials;
        }

        weaponHold.GetComponent<MeshRenderer>().materials = materials;
        weapon.GetComponent<MeshRenderer>().materials = materials;
    }

    private void PlayerController_OnShopping(object sender, EventArgs e) {
        animationControl.SetDance();
    }
    private void PlayerController_OnOutShopping(object sender, EventArgs e) {
        animationControl.StopDance();
    }

    private void PlayerController_OnUserChangeWeapon(object sender, DataManager.OnUserChangeWeaponArg e) {
        //Debug.Log("Change Weapon");
        //Debug.Log(e.skinIndex);
        //Debug.Log(e.materials[0] + " " +  e.materials[1] + " " + e.materials[2]);
        SetUpWeaponMaterial(e);
    }

    // Ham xu li su kien khi player upgradeskill, tham chieu toi skill object 1 lan nua
    private void PlayerController_OnPlayerUpgradeSkill(object sender, StartPanelManager.TypeSkill type) {
        ReferenceToObject();
    }

    private void PlayerController_OnPlayerChooseAbility2(object sender, EventArgs e) {
        weaponQuantity += 1;
    }

    private void PlayerController_OnPlayerChooseAbility3(object sender, EventArgs e) {
        //isAbility3 = true;
    }
    private void PlayerController_OnplayerChooseAbility4(object sender, EventArgs e) {
        isAbility4 = true;
        animationControl.SetSpeedMultiplayer();
    }
    private void PlayerController_OnPlayerChooseAbility8(object sender, EventArgs e) {
        isAbility8 = true;
    }
    private void PlayerController_OnPlayerChooseAbility9(object sender, EventArgs e) {
        isAbility9 = true;
    }
    private void PlayerController_OnPlayerChooseAbility10(object sender, EventArgs e) {
        stateManager.isCanRevive = true;
    }
    private void PlayerController_OnPlayerChooseAbility11(object sender, EventArgs e) {
        this.transform.localScale += new Vector3(0.05f, 0.05f, 0.05f);
        CameraController.Instance.UpdateDistanceCamera(.75f);
        stateManager.DoUpdateStateWeapon();
    }
    private void PlayerAbility_OnPlayerChooseAbility12(object sender, EventArgs e) {
        weaponMultipleAbility = 3;
    }
    private void PlayerController_OnPlayerChooseAbility13(object sender, EventArgs e) {
        speed += 2;
    }
    public void SetPlayerWinDance () {
        if(this == null)
            return;
        if(animationControl)
            animationControl.SetDanceWin();
        this.transform.rotation = Quaternion.Euler(0, 180, 0);
        speed = 0;  
        canAttack = false;
        joystick.gameObject.SetActive(false);
    }
    private void PlayerController_OnPlayerRevive(object sender, Vector3 e) {
        canAttack = false;
        animationControl.SetIdle();
        isDead = false;
        ReferenceToObject();
    }
    private IEnumerator AttackTwice() {
        animationControl.SetAttack();
        yield return new WaitForSeconds(.5f);
        animationControl.SetEndAttack();
        animationControl.SetAttack();
    }


}
