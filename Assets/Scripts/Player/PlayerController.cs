using System;
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
    private float hp;
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

    public event EventHandler OnPlayerAttack;
    private void Awake() {
        rb = GetComponent<Rigidbody>();
        animationControl = GetComponent<AnimationControl>();
        stateManager = GetComponent<StateManager>();
        stateManager.OnCharacterDead += PlayerController_OnCharacterDead;
        startGame = false;
        isDead = false;
    }

    private void Start() {
        ReferenceToObject(SkillObjects.TypeSkill.none) ;

        timeAttackDuration = 3;
        stateWeapon = stateManager.GetStateWeapon();

        if(SceneManager.GetActiveScene().buildIndex == 0)
            WeaponShopManager.Instance.OnUserChangeWeapon += PlayerController_OnUserChangeWeapon;
        else if(SceneManager.GetActiveScene().buildIndex == 1)
            StartPanelManager.Instance.OnPlayerUpgradeSkill += PlayerController_OnPlayerUpgradeSkill;
            
    }



    void Update() {
        if (!startGame && joystick.Horizontal != 0)
            startGame = true;
        if (isDead)
            return;
        direct.x = joystick.Horizontal;
        direct.z = joystick.Vertical;
        RotateCharacter(direct);
    }

    private void FixedUpdate() {
        if(timeAttackDuration < 5)
            timeAttackDuration += Time.fixedDeltaTime;

        if (direct.normalized != Vector3.zero) {
            animationControl.SetRun();
            rb.MovePosition(rb.position + direct.normalized * speed * Time.fixedDeltaTime);

        }
        else {
            animationControl.SetIdle();
            if (canAttack && timeAttackDuration > timeAttack) {
                animationControl.SetAttack();
                timeAttackDuration = 0;
                Attack();
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

        Vector3 directEnemy = targetPosition - this.transform.position;
        directEnemy.y = 0;
        RotateCharacter(directEnemy);
        angleSpread = angleAttack / weaponCount;
        int startIndexWeapon = (int)-weaponCount / 2;
        OnPlayerAttack?.Invoke(this, EventArgs.Empty);
        //Debug.Log(startIndexWeapon);
        //Debug.Log(weaponCount / 2);

        for (int i = startIndexWeapon; i <= weaponCount/2; i++) {
            if (i == 0 && weaponCount % 2 == 0)
                continue;
            // Xac dinh vi tri spawn va huong nem
            Vector3 positionSpawn = new Vector3(this.transform.position.x,
                this.transform.position.y + 0.2f, this.transform.position.z);

            Vector3 directWeapon = Quaternion.AngleAxis(startIndexWeapon * angleSpread, Vector3.up) * this.transform.forward;

            //Khoi tao vu khi
            GameObject weaponSpawn = Instantiate(weapon, positionSpawn, Quaternion.Euler(new Vector3(90, 0, 0)));
            weaponSpawn.transform.localScale = new Vector3(6, 6, 6);

            // Kiem tra xem da len cap chua, neu co thi cap nhat trang thai cua vu khi
            if (stateManager.IsLevelUp) {
                stateWeapon = stateManager.GetStateWeapon();
                stateManager.IsLevelUp = false;
                heightAttack *= transform.localScale.x;
            }

            //Set trang thai(chu the, tam ban, scale)
            Rigidbody weaponRb = weaponSpawn.GetComponent<Rigidbody>();
            weaponRb.GetComponent<ThrowWeapon>().SetStateWeapon(stateWeapon);

            //Nem vu khi theo huong
            if (weaponRb != null) {
                weaponRb.linearVelocity = directWeapon.normalized * speedWeapon * Time.fixedDeltaTime;
            }
            startIndexWeapon++;
        }


        await Task.Delay(800);
        animationControl.EndAttack();
        canAttack = false;
    }
    private void RotateCharacter(Vector3 direct) {
        if (direct == Vector3.zero)
            return;

        Quaternion rot = Quaternion.LookRotation(direct.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(this.transform.rotation, rot, deltaAngle);
    }
    private void PlayerController_OnCharacterDead(object sender, EventArgs e) {
        isDead = true;
        speed = 0;
    }
    
    // Duoc goi khi tham chieu den SkillObjects de lay chi so
    private void ReferenceToObject(SkillObjects.TypeSkill type) {
        hp = skillObjects.hp;
        speed = skillObjects.speed;
        range = skillObjects.range;
        if(type == SkillObjects.TypeSkill.range) {
            playerAttackArea.transform.localScale = new Vector3(range, range, range);
            CameraMove.Instance.UpdateCamera(.5f);
        }
        weaponCount = skillObjects.weaponCount;
    }
    private void PlayerController_OnUserChangeWeapon(object sender, EventArgs e) {
        int curWeapon = PlayerPrefs.GetInt("CurWeapon");
        Debug.Log("Vu khi duoc chon: " +curWeapon);
        WeaponShopManager.SaveData data = WeaponShopManager.Instance.GetWeaponData(curWeapon);
        Mesh mesh = weaponObjects.GetMeshWeapon(curWeapon, data.skinIndex);
        Material[] materials = weaponObjects.GetListMaterials(curWeapon, data.skinIndex);

        weaponHold.GetComponent<MeshFilter>().mesh = mesh;
        weaponHold.GetComponent<MeshRenderer>().materials = materials;

        weapon.GetComponent<MeshFilter>().mesh = mesh;
        weapon.GetComponent <MeshRenderer>().materials = materials;
    }

    // Ham xu li su kien khi player upgradeskill
    private void PlayerController_OnPlayerUpgradeSkill(object sender, SkillObjects.TypeSkill type) {
        ReferenceToObject(type);
    }

}
