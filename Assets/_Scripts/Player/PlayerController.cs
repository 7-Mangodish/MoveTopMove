using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private static PlayerController instance;
    public static PlayerController Instance { get => instance; }

    [SerializeField] WeaponObjects weaponObjects;
    public PlayerSkin playerSkin;
    private SkillData skillData;

    [Header("-----Skill-----")]
    [SerializeField] private float speed;
    public GameObject playerAttackArea;
    public SphereCollider playerAttackAreaCollider;
    public float shield;
    public float range;
    public float weaponQuantity;
    public float bonusWeaponQuantity;
    public int weaponMultipleAbility;

    [Header("-----Move-----")]
    [SerializeField] private float deltaAngle;
    [SerializeField] private Joystick joystick;
    private Vector3 direct;
    private Rigidbody rb;
    private AnimationControl animationControl;

    [Header("-----Attack-----")]
    [SerializeField] private GameObject weapon;
    [SerializeField] private Transform weaponHold;
    [SerializeField] private Transform weaponSpawnTransform;
    [SerializeField] private float speedWeapon;
    [SerializeField] private float timeAttack;
    [SerializeField] private float angleAttack;
    private float angleSpread;
    private float timeAttackDuration;
    private Vector3 directEnemy;
    private bool canAttack;
    private Vector3 weaponSpawnPosition;
    public Transform aimArea;
    private GameObject aimEnemy;
    WeaponData weaponData;

    [Header("-----Level Up-----")]
    private StateManager stateManager;
    private ThrowWeapon.StateWeapon stateWeapon;
    public bool startGame;

    [Header("-----Player Ability-----")]
    public GameObject weaponAbility1;
    private int chosenAbility;

    [Header("-----Status-----")]
    public GameObject playerStatus;
    public TextMeshPro playerName;
    public TextMeshPro playerScore;
    public SkinnedMeshRenderer playerSkinMesh;
    public GameObject textScoreEffect;
    private Vector3 standardCharacterScale;
    private Vector3 standardEulerRot;

    [Header("-----Ulti-----")]
    public bool isUlti;

    private SoundController soundController;
    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        rb = GetComponent<Rigidbody>();
        animationControl = GetComponent<AnimationControl>();
        stateManager = GetComponent<StateManager>();
        soundController = GetComponent<SoundController>();
        stateManager.OnCharacterTakeScore += StateManager_OnCharacterTakeScore;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag(GameVariable.ENEMY_TAG) ||
            other.gameObject.CompareTag(GameVariable.ZOMBIE_TAG)) {
            if(!aimArea.gameObject.activeSelf && aimEnemy== null) {
                //aimEnemy = other.gameObject;
                //aimArea.gameObject.SetActive(true);
                DoCheckEnemyInside();
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag(GameVariable.AIM_AREA)) { 
            aimArea.position = Vector3.zero;
            aimEnemy = null;
            aimArea.gameObject.SetActive(false);
        }
    }
    public void SetUpPlayer() {
        timeAttackDuration = 3;
        chosenAbility = -1;
        startGame = false;
        weaponQuantity = 1;
        weaponMultipleAbility = 1;
        isUlti = false;
        stateWeapon = stateManager.GetStateWeapon();
        /*Sua lai scale neu no la boom*/
        LoadWeapon();
        //
        if(SceneManager.GetActiveScene().name == GameVariable.zombieSceneName) {
            SetUpAttackRange();
        }
        /*Status*/
        standardEulerRot = playerStatus.transform.rotation.eulerAngles;
        standardCharacterScale = this.transform.localScale;
        //
        playerStatus.GetComponent<SpriteRenderer>().color = playerSkinMesh.material.color;
        playerName.color = playerSkinMesh.material.color;
        playerName.text = DataManager.Instance.playerData.playerName;
    }

    public void GetPlayerInput() {
        if (!startGame && joystick.Horizontal != 0) {
            startGame = true;
        }
        if (this.gameObject.tag == GameVariable.DEAD_TAG)
            return;
        direct.x = joystick.Horizontal;
        direct.z = joystick.Vertical;
        RotateCharacter(direct);
    }

    public void PlayerBehaviour() {
        if (aimEnemy != null)
            aimArea.position = aimEnemy.transform.position;
        else {
            aimArea.position = Vector3.zero;
            aimArea.gameObject.SetActive(false);
        }
        //
        if (this.gameObject.tag == GameVariable.DEAD_TAG) return;
        //
        if(stateManager.isDead&& this.gameObject.activeSelf==false && 
            SceneManager.GetActiveScene().name == GameVariable.zombieSceneName && chosenAbility == 10 ) {
            DoAbility();
        }
        //
        if (timeAttackDuration < 5)
            timeAttackDuration += Time.fixedDeltaTime;
        //
        if (direct.normalized != Vector3.zero) {
            animationControl.SetRun();
            rb.MovePosition(rb.position + direct.normalized * speed * Time.fixedDeltaTime);

        }
        else {
            animationControl.SetIdle();
            //
            if (timeAttackDuration > timeAttack) canAttack = true;
            if (!canAttack) return;
            if (DoCheckEnemyInside()) {
                timeAttackDuration = 0;
                canAttack = false;
                //
                directEnemy = aimArea.position - this.transform.position;
                directEnemy.y = 0;
                //
                if (isUlti) {
                    RotateCharacter(directEnemy);
                    animationControl.SetUlti();
                    playerAttackArea.transform.localScale = new Vector3(2, 2, 1);

                }
                else {
                    if (chosenAbility != 4) {
                        RotateCharacter(directEnemy);
                        animationControl.SetAttack();

                    }
                    else {
                        RotateCharacter(directEnemy);
                        StartCoroutine(AttackTwice());
                    }
                }

            }
        }
    }

    #region -----------ATTACK-----------
    private bool DoCheckEnemyInside() {
        float radius = playerAttackAreaCollider.radius * playerAttackAreaCollider.transform.lossyScale.x;
        Collider[] hitCollider = Physics.OverlapSphere(this.transform.position, radius);
        //
        if (aimArea.gameObject.activeSelf && aimEnemy != null && !aimEnemy.CompareTag(GameVariable.DEAD_TAG)) {
            foreach(Collider hit in hitCollider) {
                if (hit.gameObject.CompareTag(GameVariable.ENEMY_TAG) || hit.gameObject.CompareTag(GameVariable.ZOMBIE_TAG)) {
                    if (hit.gameObject == aimEnemy)
                        return true;
                }
            }
        }
        //
        float minDisTance = float.MaxValue;
        foreach (Collider hit in hitCollider) {
            if (hit.gameObject.CompareTag(GameVariable.ENEMY_TAG) || hit.gameObject.CompareTag(GameVariable.ZOMBIE_TAG)) {

                float distance = Vector2.Distance(this.gameObject.transform.position, hit.gameObject.transform.position);
                if ( distance < minDisTance ) {
                    minDisTance = distance;
                    aimEnemy = hit.gameObject;
                }
            }
        }
        //
        if (aimEnemy != null)
            aimArea.gameObject.SetActive(true);
        else
            aimArea.gameObject.SetActive(false);

        return false;
    }
    private async void Attack() {
        if (stateManager.isDead)
            return;
        //
        soundController.PlaySound(SoundData.SoundName.weapon_throw);
        weaponHold.gameObject.SetActive(false);
        // Kiem tra xem da len cap chua, neu co thi cap nhat lai trang thai cua vu khi
        if (stateManager.IsLevelUp) {
            stateWeapon = stateManager.GetStateWeapon();
            stateManager.IsLevelUp = false;
        }
        if (stateManager.isLevelUpZombieMode) {
            stateWeapon = stateManager.GetStateWeapon();
            stateManager.isLevelUpZombieMode = false;

            if (weaponQuantity < bonusWeaponQuantity + 1)
                weaponQuantity +=1;
        }
        // Nhan voi weaponMultipleAbility neu la ability
        weaponQuantity *= weaponMultipleAbility;
        // Tan cong va goi su kien
        angleSpread = angleAttack / weaponQuantity;
        int startIndexWeapon = (int) - weaponQuantity / 2;
        // Xac dinh vi tri spawn va huong nem
        weaponSpawnPosition = new Vector3(weaponSpawnTransform.position.x,
            weaponSpawnTransform.position.y, weaponSpawnTransform.position.z);
        // Nem vu khi
        for (int i = startIndexWeapon; i <= weaponQuantity /2; i++) {
            if (i == 0 && weaponQuantity % 2 == 0) continue;
            //
            Vector3 directWeapon = Quaternion.AngleAxis(startIndexWeapon * angleSpread, Vector3.up) * directEnemy;
            //Khoi tao vu khi
            Quaternion rot = Quaternion.LookRotation(directWeapon.normalized, Vector3.up);
            GameObject weaponSpawn = Instantiate(weapon, weaponSpawnPosition, Quaternion.Euler(-90, rot.eulerAngles.y, 0));
            //Set trang thai(chu the, tam ban, scale, vi tri khoi tao)
            if (!weaponData.isBoom)
                weaponSpawn.transform.localScale = new Vector3(9, 9, 9);
            else {
                weaponSpawn.transform.localScale = new Vector3(3, 3, 3);
                stateWeapon.isBoom = true;
                weaponSpawn.transform.rotation = Quaternion.Euler(-90, rot.eulerAngles.y -90, 0); 
            }
            //
            stateWeapon.positionSpawn = weaponSpawnPosition;
            Rigidbody weaponRb = weaponSpawn.GetComponent<Rigidbody>();
            ThrowWeapon weaponThrow = weaponRb.GetComponent<ThrowWeapon>();
            // Kiem tra xem player co chon ability8 hay khong
            if (chosenAbility == 8) {
                weaponThrow.isGrowing = true;
                stateWeapon.maxDistance += .5f;
            }
            // Kiem tra xem player co chon ability9 hay khong
            if (chosenAbility == 9) {
                weaponThrow.isPiering = true;
            }
            if (isUlti) {
                speedWeapon = 125;
                weaponThrow.isUlti = true;
            }
            else {
                speedWeapon = 100;
            }
            weaponThrow.SetStateWeapon(stateWeapon);
            //Nem vu khi theo huong
            if (weaponRb != null) {
                weaponRb.linearVelocity = directWeapon.normalized * speedWeapon * Time.fixedDeltaTime;
            }
            startIndexWeapon++;
        }
        // Thuc hien ki nang dac biet
        if (chosenAbility == 0 || chosenAbility == 5 || chosenAbility == 6)
            DoAbility();
        //
        await Task.Delay(800);
        isUlti = false;
        if(weaponHold != null)
            weaponHold.gameObject.SetActive(true);
    }

    private void EndAttack() {
        animationControl.SetEndAttack();
        canAttack = false;
    }
    #endregion

    private void RotateCharacter(Vector3 direct) {
        if (direct == Vector3.zero)
            return;

        Quaternion rot = Quaternion.LookRotation(direct.normalized, Vector3.up);    
        transform.rotation = Quaternion.RotateTowards(this.transform.rotation, rot, deltaAngle);
    }

    // Duoc goi khi tham chieu den Skill de lay chi so
    public void ReferenceToSkillAndAbility(SkillData skillData) {
        shield = skillData.shield;
        speed = skillData.speed;
        range = skillData.range;
        bonusWeaponQuantity = skillData.weaponBonus;

        chosenAbility = ZombieUIController.choosenAbility;
        if (chosenAbility == 1 || chosenAbility == 2 || chosenAbility == 4 ||
            chosenAbility == 7 || chosenAbility == 11 || chosenAbility == 12 || chosenAbility == 13)
            DoAbility();
    }
    /*Duoc goi khi player update Range*/
    public void SetUpAttackRange() {
        skillData = DataManager.Instance.GetSkillData();
        range = skillData.range;
        playerAttackArea.transform.localScale = new Vector3(range, range, range);

    }

    #region ----------WEAPON_MATERIALS----------
    private void LoadWeaponSkin(GameObject wp) {
        int curWeaponId = DataManager.Instance.playerData.currentWeaponId;
        //
        Mesh mesh = weaponObjects.GetMeshWeapon(curWeaponId, 2);
        wp.GetComponent<MeshFilter>().mesh = mesh;
        //
        Material[] weaponMaterial = DataManager.Instance.GetWeaponMaterial(curWeaponId);
        int materialCount = weaponObjects.GetListMaterials(curWeaponId, 2).Length;
        Material[] materials = new Material[materialCount];
        for (int i = 0; i < materialCount; i++) {
            materials[i] = weaponMaterial[i];
        }
        //
        wp.GetComponent<MeshRenderer>().materials = materials;
    }


    //private void PlayerController_OnUserChangeWeapon(object sender, EventArgs e) {
    //    if (DataManager.Instance.playerData.isWeaponBoom) {
    //        weaponHold.transform.localScale = new Vector3(11, 11, 11);
    //        weaponHold.transform.localPosition = new Vector3(0, .5f, 0);
    //    }
    //    LoadWeaponSkin(weapon);
    //    LoadWeaponSkin(weaponHold.gameObject);
    //}

    public void LoadWeapon() {
        int curWeaponId = DataManager.Instance.playerData.currentWeaponId;
        weaponData = DataManager.Instance.GetWeaponData(curWeaponId);
        if (weaponData.isBoom) {
            weaponHold.transform.localScale = new Vector3(11, 11, 11);
            weaponHold.transform.localPosition = new Vector3(0, .5f, 0);
        }
        LoadWeaponSkin(weapon);
        LoadWeaponSkin(weaponHold.gameObject);
    }
#endregion

    #region ----------Dance----------
    public void SetPlayerDance() {
        animationControl.SetDance();
    }
    public void SetPlayerStopDance() {
        animationControl.StopDance();
    }
    public void SetPlayerWinDance() {
        if (this == null)
            return;
        if (animationControl)
            animationControl.SetDanceWin();
        this.transform.rotation = Quaternion.Euler(0, 180, 0);
        speed = 0;
        canAttack = false;
        joystick.gameObject.SetActive(false);
    }
#endregion

    #region ----------Abilty---------
    
    private void DoAbility() {
        switch (chosenAbility) {
            case 0:
                Ability0(); break;
            case 1:
                Ability1();break;
            case 2: 
                Ability2(); break;
            case 3: 
                Ability3(); break;
            case 4: 
                Ability4(); break;
            case 5: 
                Ability5(); break;
            case 6:
                Ability6(); break;
            case 7:
                Ability7(); break;
            case 8:
                Ability8(); break;
            case 9:
                Ability9(); break;
            case 10:
                Ability10(); break;
            case 11:
                Ability11(); break;
            case 12:
                Ability12(); break;
            case 13:
                Ability13(); break;
        }
    }

    /*Backward Attack*/
    private void Ability0() {
        Vector3 spawnPosition = new Vector3(this.transform.position.x,
            weaponSpawnPosition.y, this.transform.position.z);
        GameObject weaponSpawn = Instantiate(weapon, weaponSpawnPosition, Quaternion.Euler(new Vector3(90, 0, 0)));
        weaponSpawn.transform.localScale = new Vector3(9, 9, 9);

        // Kiem tra xem da len cap chua, neu co thi cap nhat trang thai cua vu khi
        if (stateManager.IsLevelUp) {
            stateWeapon = stateManager.GetStateWeapon();
        }

        //Set trang thai(chu the, tam ban, scale)
        Rigidbody weaponRb = weaponSpawn.GetComponent<Rigidbody>();
        stateWeapon.positionSpawn = this.transform.position;
        weaponRb.GetComponent<ThrowWeapon>().SetStateWeapon(stateWeapon);

        //Nem vu khi theo huong
        if (weaponRb != null) {
            weaponRb.linearVelocity = -1 * this.transform.forward * speedWeapon * Time.fixedDeltaTime;
        }
    }

    /*Circle Weapon*/
    private void Ability1() {

        LoadWeaponSkin(weaponAbility1);
        weaponAbility1.SetActive(true);
    }

    /*Add Weapon Count*/
    private void Ability2() {
        weaponQuantity += 1;
    }

    private void Ability3() {

    }

    /*Duplicate Atact*/
    private void Ability4() {
        animationControl.SetSpeedMultiplayer();
    }

    /*Cross Attack*/
    private void Ability5() {
        Vector3 positionSpawn = new Vector3(this.transform.position.x,
            weaponSpawnPosition.y, this.transform.position.z);
        for (int i = 0; i < 2; i++) {
            GameObject weaponSpawn = Instantiate(weapon, positionSpawn, Quaternion.Euler(new Vector3(90, 0, 0)));
            weaponSpawn.transform.localScale = new Vector3(9, 9, 9);

            // Kiem tra xem da len cap chua, neu co thi cap nhat trang thai cua vu khi
            if (stateManager.IsLevelUp) {
                stateWeapon = stateManager.GetStateWeapon();
            }

            //Set trang thai(chu the, tam ban, scale)
            Rigidbody weaponRb = weaponSpawn.GetComponent<Rigidbody>();
            stateWeapon.positionSpawn = this.transform.position;
            weaponRb.GetComponent<ThrowWeapon>().SetStateWeapon(stateWeapon);

            //Nem vu khi theo huong
            if (weaponRb != null) {
                if (i == 0)
                    weaponRb.linearVelocity = this.transform.right * speedWeapon * Time.fixedDeltaTime;
                else
                    weaponRb.linearVelocity = -1 * this.transform.right * speedWeapon * Time.fixedDeltaTime;
            }
        }
    }

    /*Diagonal Attack*/
    private void Ability6() {
        Vector3 positionSpawn = new Vector3(this.transform.position.x,
            this.transform.position.y + 0.2f, this.transform.position.z);
        for (int i = 0; i <= 1; i++) {
            GameObject weaponSpawn = Instantiate(weapon, positionSpawn, Quaternion.Euler(new Vector3(90, 0, 0)));
            weaponSpawn.transform.localScale = new Vector3(9, 9, 9);

            // Kiem tra xem da len cap chua, neu co thi cap nhat trang thai cua vu khi
            if (stateManager.IsLevelUp) {
                stateWeapon = stateManager.GetStateWeapon();
            }

            //Set trang thai(chu the, tam ban, scale)
            Rigidbody weaponRb = weaponSpawn.GetComponent<Rigidbody>();
            stateWeapon.positionSpawn = this.transform.position;
            weaponRb.GetComponent<ThrowWeapon>().SetStateWeapon(stateWeapon);

            //Nem vu khi theo huong
            if (weaponRb != null) {
                if (i == 0) {
                    Vector3 direct = Quaternion.AngleAxis(60, Vector3.up) * this.transform.forward;
                    weaponRb.linearVelocity = speedWeapon * direct.normalized * Time.fixedDeltaTime;
                }
                else {
                    Vector3 direct = Quaternion.AngleAxis(-60, Vector3.up) * this.transform.forward;
                    weaponRb.linearVelocity = speedWeapon * direct.normalized * Time.fixedDeltaTime;
                }
            }
        }
    }

    /*Double Reward*/
    private void Ability7() {
        DataManager.Instance.isDoubleAward = true;
    }

    /*Growing Weapon*/
    private void Ability8() {

    }
    /*Piering Weapon*/
    private void Ability9() {

    }

    /*Revive*/
    private void Ability10() {
        Vector3 newPosition = SpawnZombieController.Instance.GetValidPosition();
        this.PlayerRevive(newPosition);
    }

    /*Start Bigger*/
    private void Ability11() {
        this.transform.localScale += new Vector3(0.05f, 0.05f, 0.05f);
        CameraController.Instance.UpdateDistanceCamera(.75f);
        stateManager.DoUpdateStateWeapon();
    }

    /*X3 Weapon*/
    private void Ability12() {
        weaponMultipleAbility = 3;
    }

    /*Speed*/
    private void Ability13() {
        speed += 2;
    }
    
    private IEnumerator AttackTwice() {
        Debug.Log("AttackTwice");
        animationControl.SetAttack();
        yield return new WaitForSeconds(.5f);
        animationControl.SetEndAttack();
        animationControl.SetAttack();
    }
#endregion
    public void PlayerRevive(Vector3 newPosition) {
        canAttack = false;
        stateManager.isDead = false;
        animationControl.SetIdle();
        aimArea.gameObject.SetActive(false);
        aimEnemy = null;
        this.gameObject.tag = "Player";

        //ReferenceToObject();
        this.gameObject.SetActive(true);
        this.gameObject.transform.localPosition = new Vector3(newPosition.x, 0, newPosition.z);
    }

    #region ------------Status------------

    public void SetPlayerName() {      ;
        playerName.text = DataManager.Instance.playerData.playerName;
    }
    public void UpdatePlayerStatus() {
        playerStatus.transform.rotation = Quaternion.Euler(standardEulerRot);
        //
        Vector3 camToStatus = playerStatus.transform.position - Camera.main.transform.position;
        float distanceOnCameraForward = Vector3.Dot(camToStatus, Camera.main.transform.forward);
        //
        float propotion = (distanceOnCameraForward / GameVariable.STD_DISTANCE);
        Vector3 newScale = propotion * GameVariable.STD_SCALE;
        //
        if (standardCharacterScale != this.transform.localScale) {
            float scaleFactor = this.transform.localScale.x / standardCharacterScale.x;
            newScale /= scaleFactor;
        }
        playerStatus.transform.localScale = newScale;

    }

    private async void StateManager_OnCharacterTakeScore(object sender, int e) {
        playerScore.text = stateManager.CurrentScore.ToString();
        textScoreEffect.gameObject.SetActive(true);
        await Task.Delay(500);
        textScoreEffect.gameObject.SetActive(false);
    }
    #endregion
}
