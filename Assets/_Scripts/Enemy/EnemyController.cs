using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Android;
using UnityEngine.UI;
using static ThrowWeapon;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    //[SerializeField] private GameObject aimZone;

    [Header("-----Enemy Move-----")]
    [SerializeField] private Transform targetPosition;
    [SerializeField] private float maxRangeX;
    [SerializeField] private float maxRangeZ;
    private NavMeshAgent agent;

    [Header("-----Enemy Idle-----")]
    [SerializeField] private float timeIdle;
    private AnimationControl animationControl;
    private float timeIdleDuration;

    [Header("-----Enenmy Attack-----")]
    public float speedWeapon;
    public float timeAttack;
    public Transform enemyAttackArea;
    public bool isUlti;
    private GameObject enemyWeapon;
    private EnemyRamdomItem enemyRandomItem;
    private float timeAttackDuration;
    private bool canAttack;
    private Vector3 playerPosition;


    [Header("-----Level Up-----")]
    private StateManager stateManager;
    private ThrowWeapon.StateWeapon stateWeapon;

    /*--------------------INDICATOR-------------------------*/
    [Header("-----INDICATOR-----")]
    public GameObject targetCanvas;
    public GameObject indicatorContainer;
    public Image indicatorIcon;
    public Image indicatorScoreContainer;
    public TextMeshProUGUI indicatorScoreText;
    public SkinnedMeshRenderer skinCharacter;
    private Camera mainCamera;

    [Header("-----STATUS-----")]
    public GameObject characterStatus;
    public TextMeshPro characterName;
    public TextMeshPro characterScore;
    private Vector3 standardCharacterScale;
    private Vector3 standardEulerRot;

    public bool isOutScreen = false;

    private void Awake() {
        isUlti = false;
        animationControl = GetComponent<AnimationControl>();
        agent = GetComponent<NavMeshAgent>();
        stateManager = GetComponent<StateManager>();
        enemyRandomItem = GetComponent<EnemyRamdomItem>();


        /*Indicator*/
        targetCanvas = GameObject.FindGameObjectWithTag("Canvas");
        mainCamera = Camera.main;

        enemyRandomItem.RandomEnemyItem();
        enemyWeapon = enemyRandomItem.GetRandomEnemyWeapon();

    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Enemy")) {
            //Debug.Log("Enemy Can Fire");
            canAttack = true;
            playerPosition = other.transform.position;
        }
    }
    private void OnTriggerStay(Collider other) {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Enemy")) {
            //Debug.Log("Enemy is still in trigger");
            canAttack = true;
            playerPosition = other.transform.position;
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("PlayerZone") && this.gameObject.CompareTag("Enemy")) {
            //Debug.Log("Inside Player Zone");
            //aimZone.gameObject.SetActive(false);
        }
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Enemy")) {
            canAttack = false;
        }
    }

    private void OnDisable() {
        Destroy(indicatorContainer);
    }
    public void SetUpEnemy() {
        stateWeapon = stateManager.GetStateWeapon();

        targetPosition.position = this.transform.position;
        agent.destination = targetPosition.position;

        /*Indicator*/
        indicatorIcon.color = skinCharacter.material.color;
        indicatorScoreContainer.color = skinCharacter.material.color;
        indicatorContainer.transform.SetParent(targetCanvas.transform);


        /*STATUS*/
        standardEulerRot = characterStatus.transform.rotation.eulerAngles;
        standardCharacterScale = this.transform.localScale;

        characterStatus.GetComponent<SpriteRenderer>().color = skinCharacter.material.color;
        characterName.color = skinCharacter.material.color;

        int randomNum = (int)Random.Range(0, 100);
        characterName.text = "Enemy" + randomNum.ToString();
        this.gameObject.transform.parent.name = characterName.text;
    }

    public void EnemyBehaviour() {
        //UpdateIndicator();
        if (animationControl.currentState == AnimationControl.state.IsDead) {
            agent.speed = 0;
            this.gameObject.tag = GameVariable.DEAD_TAG;
            return;
        }
        if (CheckDistance() <= .1f) {
            animationControl.SetIdle();
            timeIdleDuration += Time.deltaTime;
            timeAttackDuration += Time.deltaTime;

            if (canAttack) {

                if (timeAttackDuration > timeAttack) {
                    animationControl.SetAttack();
                    timeAttackDuration = 0;
                    EnemyAttack();
                }
            }
            else {
                if (timeIdleDuration > timeIdle) {
                    targetPosition.position = GetRandomPosition();
                    timeIdleDuration = 0;
                }

            }

        }
        else {
            agent.destination = targetPosition.position;
            animationControl.SetRun();
        }

    }

    private async void EnemyAttack() {
        // Xac dinh vi tri spawn va huong nem
        Vector3 positionSpawn = new Vector3(this.transform.position.x, 
            this.transform.position.y + 0.2f, this.transform.position.z);
        Vector3 directWeapon = playerPosition - this.transform.position;
        directWeapon.y = 0f;
        RotateCharacter(directWeapon);

        // Kiem tra xem da len cap chua, neu co thi cap nhat scale cua vu khi
        if (stateManager.IsLevelUp) {
            stateWeapon = stateManager.GetStateWeapon();
            stateManager.IsLevelUp = false;
        }

        //Khoi tao vu khi
        Quaternion rot = Quaternion.LookRotation(directWeapon, Vector3.up);
        GameObject weaponSpawn = Instantiate(enemyWeapon, positionSpawn, Quaternion.Euler(new Vector3(-90, rot.eulerAngles.y, 0)));

        //Set trang thai(chu the, tam ban, scale, vi tri)
        stateWeapon.positionSpawn = this.transform.position;
        Rigidbody weaponRb = weaponSpawn.GetComponent<Rigidbody>();
        ThrowWeapon weaponThrow = weaponSpawn.GetComponent<ThrowWeapon>();

        if(isUlti)
            weaponThrow.isUlti = true;
        weaponThrow.SetStateWeapon(stateWeapon);

        //Nem vu khi theo huong
        if (weaponRb != null) {
            weaponRb.linearVelocity = directWeapon.normalized * speedWeapon * Time.fixedDeltaTime;
        }
        await Task.Delay(1000);
        isUlti = false;
        canAttack = false;
    }

    private void RotateCharacter(Vector3 direct) {
        Quaternion rot = Quaternion.LookRotation(direct.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(this.transform.rotation, rot, 360);
    }

    private float CheckDistance() {
        return (float)Math.Sqrt(
            Math.Pow((this.transform.position.x - targetPosition.position.x),2) +
            Math.Pow((this.transform.position.z - targetPosition.position.z), 2)
            );
    }

    private Vector3 GetRandomPosition() {
        float randX = Random.Range(-3, 3);
        float randZ = Random.Range(-3, 3);
        int randM = Random.Range(-1, 1) < 0 ? -1 : 1;

        Vector3 newPosition = targetPosition.position + new Vector3(randX * randM, 0, randZ * randM);

        for(int i = 0; i< 30; i++) {
            if(NavMesh.SamplePosition(newPosition, out NavMeshHit hit, .5f, NavMesh.AllAreas)) {
                newPosition = hit.position;
                break;
            }
            randX = Random.Range(-3, 3);
            randZ = Random.Range(-3, 3);
            randM = Random.Range(-1, 1) < 0 ? -1 : 1;

            newPosition = targetPosition.position + new Vector3(randX * randM, 0, randZ * randM);

        }

        return newPosition;
    }

    #region -----------Indicator----------
    public void UpdateIndicator() {
        Vector3 enemyPositionOnScreen = mainCamera.WorldToScreenPoint(this.transform.position);
        if (enemyPositionOnScreen.z < 0)
            return;
        //Debug.Log(enemyPositionOnScreen);
        if (enemyPositionOnScreen.x < 0 || enemyPositionOnScreen.x > Screen.width
            || enemyPositionOnScreen.y < 0 || enemyPositionOnScreen.y > Screen.height) {

            indicatorScoreText.text = stateManager.CurrentScore.ToString();
            //Debug.Log(sizeDelta);
            enemyPositionOnScreen.x = Mathf.Clamp(enemyPositionOnScreen.x, 0, Screen.width - 150);
            enemyPositionOnScreen.y = Mathf.Clamp(enemyPositionOnScreen.y, 0, Screen.height - 150);
            enemyPositionOnScreen.z = 0;

            Vector3 direct = enemyPositionOnScreen - new Vector3(Screen.width / 2, Screen.height / 2, 0);
            float angle = Mathf.Atan2(direct.y, direct.x) * Mathf.Rad2Deg;
            angle = (angle < 0) ? (angle + 360) : angle;
            isOutScreen = true;
            indicatorContainer.gameObject.SetActive(true);
            indicatorContainer.GetComponent<RectTransform>().position = enemyPositionOnScreen;

            indicatorIcon.rectTransform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
        else {
            isOutScreen = false;
            indicatorContainer.gameObject.SetActive(false);
        }
    }
    #endregion

    #region ------------Status------------
    public void UpdateEnemyStatus() {
        characterStatus.transform.rotation = Quaternion.Euler(standardEulerRot);

        Vector3 camToStatus = characterStatus.transform.position - Camera.main.transform.position;
        float distanceOnCameraForward = Vector3.Dot(camToStatus, Camera.main.transform.forward);

        float propotion = (distanceOnCameraForward / GameVariable.STD_DISTANCE);
        Vector3 newScale = propotion * GameVariable.STD_SCALE;

        if (standardCharacterScale != this.transform.localScale) {
            float scaleFactor = this.transform.localScale.x / standardCharacterScale.x;
            newScale /= scaleFactor;
        }
        characterStatus.transform.localScale = newScale;

        characterScore.text = stateManager.CurrentScore.ToString();
    }
    #endregion
}
