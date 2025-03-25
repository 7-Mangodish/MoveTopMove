using System;
using System.Collections;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] WeaponObjects weaponObjects;
    [SerializeField] SkillObjects skillObjects;

    [Header("Skill")]
    [SerializeField] private float speed;
    [SerializeField] private GameObject playerAttackArea;
    private float shield;
    private float range;
    private float weaponCount;

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
    [SerializeField] private float heightAttack;
    [SerializeField] private float angleAttack;
    private float angleSpread;
    private float timeAttackDuration;
    private Vector3 targetPosition;
    private bool canAttack;

    [Header("Level Up")]
    private StateManager stateManager;
    private ThrowWeapon.StateWeapon stateWeapon;

    [Header("Transparent Obstacle")]
    [SerializeField] private Material transparentMaterial;
    private Material oldMaterial;
    private bool isDead;
    private bool startGame;

    /*Ability*/
    public event EventHandler OnPlayerAttack;
    private PlayerAbility playerAbility;

    //Ability3
    private bool isAbility3;

    // Ability4
    private bool isAbility4;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        animationControl = GetComponent<AnimationControl>();
        stateManager = GetComponent<StateManager>();
        playerAbility = GetComponent<PlayerAbility>();

        startGame = false;
        isDead = false;
        weaponCount = 1;
    }

    private void Start() {
        SetUpWeaponMaterial();

        if(SceneManager.GetActiveScene().buildIndex == 1) {
            playerAbility.OnPlayerChooseAbility3 += PlayerController_OnPlayerChooseAbility3;
            playerAbility.OnplayerChooseAbility4 += PlayerController_OnplayerChooseAbility4;
        }
        GameManager.Instance.OnPlayerWin += PlayerController_OnPlayerWin;

        stateManager.OnCharacterDead += PlayerController_OnCharacterDead;

        ReferenceToObject() ;

        timeAttackDuration = 3;
        stateWeapon = stateManager.GetStateWeapon();

        if(SceneManager.GetActiveScene().buildIndex == 0) {
            WeaponDataManager.Instance.OnUserChangeWeapon += PlayerController_OnUserChangeWeapon;
            HomePageManager.Instance.OnShopping += PlayerController_OnShopping;
            HomePageManager.Instance.OnOutShopping += PlayerController_OnOutShopping;
        }
        else if(SceneManager.GetActiveScene().buildIndex == 1)
            StartPanelManager.Instance.OnPlayerUpgradeSkill += PlayerController_OnPlayerUpgradeSkill;
            
    }

    void Update() {
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

    private  void FixedUpdate() {
        if(timeAttackDuration < 5)
            timeAttackDuration += Time.fixedDeltaTime;

        if (direct.normalized != Vector3.zero) {
            animationControl.SetRun();
            rb.MovePosition(rb.position + direct.normalized * speed * Time.fixedDeltaTime);

        }
        else {
            animationControl.SetIdle();
            if (canAttack && timeAttackDuration > timeAttack) {
                if (!isAbility4) {
                    animationControl.SetAttack();
                    Attack();
                }
                else {
                    StartCoroutine(AttackTwice());
                }
                timeAttackDuration = 0;
            }
        }

    }


    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Zombie"))
        {
            canAttack = true;
            targetPosition = other.transform.position;
        }
        if (other.gameObject.CompareTag("Obstacle")) {
            oldMaterial = other.GetComponent<MeshRenderer>().material;
            other.GetComponent<MeshRenderer>().material = transparentMaterial;
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Zombie") ) {
           // Debug.Log("Enemy's position in Trigger Stay:" + other.transform.position);
            targetPosition = other.transform.position;
            canAttack = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Zombie"))
            canAttack = false;

        if (other.gameObject.CompareTag("Obstacle")) {
            other.GetComponent<MeshRenderer>().material = oldMaterial;
        }
    }
    private async void Attack() {

        // chon direct va quay character ve huong ke dich
        Vector3 directEnemy = targetPosition - this.transform.position;
        directEnemy.y = 0;
        RotateCharacter(directEnemy);

        // Delay .2s gay bien mat vu khi
        await Task.Delay(200);
        weaponHold.gameObject.SetActive(false);

        // Tan cong va goi su kien
        angleSpread = angleAttack / weaponCount;
        int startIndexWeapon = (int)-weaponCount / 2;
        OnPlayerAttack?.Invoke(this, EventArgs.Empty);

        for (int i = startIndexWeapon; i <= weaponCount/2; i++) {
            if (i == 0 && weaponCount % 2 == 0)
                continue;
            // Xac dinh vi tri spawn va huong nem
            Vector3 positionSpawn = new Vector3(weaponSpawnPosition.position.x,
                weaponSpawnPosition.position.y + 0.2f, weaponSpawnPosition.position.z);

            Vector3 directWeapon = Quaternion.AngleAxis(startIndexWeapon * angleSpread, Vector3.up) * this.transform.forward;   

            //Khoi tao vu khi
            GameObject weaponSpawn = Instantiate(weapon, positionSpawn, Quaternion.Euler(new Vector3(90, 0, 0)));
            weaponSpawn.transform.localScale = new Vector3(6, 6, 6);


            // Kiem tra xem da len cap chua, neu co thi cap nhat lai trang thai cua vu khi
            if (stateManager.IsLevelUp) {
                stateWeapon = stateManager.GetStateWeapon();

                stateManager.IsLevelUp = false;
                heightAttack *= transform.localScale.x;
            }

            //Set trang thai(chu the, tam ban, scale, vi tri khoi tao)
            stateWeapon.positionSpawn = positionSpawn;
            Rigidbody weaponRb = weaponSpawn.GetComponent<Rigidbody>();
            weaponRb.GetComponent<ThrowWeapon>().SetStateWeapon(stateWeapon);

            //Nem vu khi theo huong
            if (weaponRb != null) {
                weaponRb.linearVelocity = directWeapon.normalized * speedWeapon * Time.fixedDeltaTime;
            }
            startIndexWeapon++;
        }

        StartCoroutine(StopAnimation());

        await Task.Delay(800);
        weaponHold.gameObject.SetActive(true);
        canAttack = false;

    }
    private void RotateCharacter(Vector3 direct) {
        if (direct == Vector3.zero)
            return;

        Quaternion rot = Quaternion.LookRotation(direct.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(this.transform.rotation, rot, deltaAngle);
    }
    
    // Duoc goi khi tham chieu den SkillObjects de lay chi so
    private void ReferenceToObject() {
        shield = skillObjects.shield;
        speed = skillObjects.speed;
        range = skillObjects.range;
        playerAttackArea.transform.localScale = new Vector3(range, range, range);
        weaponCount += skillObjects.weaponBonus;
    }

    private void SetUpWeaponMaterial() {
        int curWeapon = PlayerPrefs.GetInt("CurWeapon");
        Debug.Log("Vu khi duoc chon: " + curWeapon);
        WeaponDataManager.SaveData data = WeaponDataManager.Instance.GetWeaponData(curWeapon);
        Mesh mesh = weaponObjects.GetMeshWeapon(curWeapon, data.skinIndex);
        Material[] materials = weaponObjects.GetListMaterials(curWeapon, data.skinIndex);

        weaponHold.GetComponent<MeshFilter>().mesh = mesh;
        weaponHold.GetComponent<MeshRenderer>().materials = materials;

        weapon.GetComponent<MeshFilter>().mesh = mesh;
        weapon.GetComponent<MeshRenderer>().materials = materials;
    }
    private void PlayerController_OnCharacterDead(object sender, EventArgs e) {
        isDead = true;
        speed = 0;
    }

    private void PlayerController_OnShopping(object sender, EventArgs e) {
        animationControl.SetDance();
    }
    private void PlayerController_OnOutShopping(object sender, EventArgs e) {
        animationControl.StopDance();
    }

    private void PlayerController_OnUserChangeWeapon(object sender, EventArgs e) {
        SetUpWeaponMaterial();
    }

    // Ham xu li su kien khi player upgradeskill, tham chieu toi skill object 1 lan nua
    private void PlayerController_OnPlayerUpgradeSkill(object sender, SkillObjects.TypeSkill type) {
        ReferenceToObject();
    }

    //Ham xu li xu kien khi player chon ability3, 
    private void PlayerController_OnPlayerChooseAbility3(object sender, EventArgs e) {
        //isAbility3 = true;
    }
    private void PlayerController_OnplayerChooseAbility4(object sender, EventArgs e) {
        isAbility4 = true;
        animationControl.SetSpeedMultiplayer();
    }
    private void PlayerController_OnPlayerWin(object sender, EventArgs e) {
        animationControl.SetDanceWin();
        this.transform.rotation = Quaternion.Euler(0, 180, 0);
        speed = 0;  
        canAttack = false;
        joystick.gameObject.SetActive(false);
    }
    IEnumerator StopAnimation() {
        while (animationControl.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")
            && animationControl.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) {
            yield return null;
        }

        animationControl.EndAttack();
        //Debug.Log("End Attack");
    }
    private IEnumerator AttackTwice() {
        animationControl.SetAttack();
        Attack();

        yield return new WaitForSeconds(0.5f);

        animationControl.SetAttack();
        Attack();
    }



}
