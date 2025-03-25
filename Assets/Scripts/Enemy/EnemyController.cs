using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Android;
using static ThrowWeapon;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private GameObject aimZone;

    [Header("Enemy Move")]
    [SerializeField] private Transform targetPosition;
    [SerializeField] private float maxRangeX;
    [SerializeField] private float maxRangeZ;
    private NavMeshAgent agent;

    [Header("Enemy Idle")]
    [SerializeField] private float timeIdle;
    private AnimationControl animationControl;
    private float timeIdleDuration;

    [Header("Enenmy Attack")]
    private GameObject enemyWeapon;
    [SerializeField] private float speedWeapon;
    [SerializeField] private float timeAttack;
    private EnemyRamdomItem enemyRandomItem;
    private float timeAttackDuration;
    private bool canAttack;
    private Vector3 playerPosition;

    //Level Up
    [Header("Level Up")]
    private StateManager stateManager;
    private ThrowWeapon.StateWeapon stateWeapon;

    private bool isDead = false;
    private void Awake() {
        animationControl = GetComponent<AnimationControl>();
        agent = GetComponent<NavMeshAgent>();
        stateManager = GetComponent<StateManager>();
        enemyRandomItem = GetComponent<EnemyRamdomItem>();

        stateManager.OnCharacterDead += EnemyController_OnCharacterDead;
    }

    void Start()
    {
        //Debug.Log(this.transform.position);
        agent.destination = targetPosition.position;
        stateWeapon = stateManager.GetStateWeapon();

        enemyWeapon = enemyRandomItem.GetRandomEnemyWeapon();
    }

    void Update()
    {
        if (isDead) return;
        if (CheckDistance() <= .1f) {
            animationControl.SetIdle();
            timeIdleDuration += Time.deltaTime;
            timeAttackDuration += Time.deltaTime;

            if (canAttack) {
                if(timeAttackDuration > timeAttack) {
                    animationControl.SetAttack();
                    EnemyAttack();
                    timeAttackDuration = 0;
                }
            }
            else {
                if(timeIdleDuration > timeIdle ) {
                    float randX = Random.Range(-3, 3);
                    float randZ = Random.Range(-3, 3);
                    int randM = Random.Range(-1, 1) < 0 ? -1 : 1;

                    Vector3 newPosition = targetPosition.position + new Vector3(randX * randM, 0, randZ * randM);

                    if (Math.Abs(targetPosition.position.x + randX) < maxRangeX &&
                        Math.Abs(targetPosition.position.z + randZ) < maxRangeZ) {
                        return;
                    }

                    if (NavMesh.SamplePosition(newPosition, out NavMeshHit hit, 1f, NavMesh.AllAreas)) {
                        targetPosition.position = newPosition;
                    }
                    else {
                        Debug.Log("diem khong hop le:" + newPosition);
                    }

                    timeIdleDuration = 0;
                }

            }
        }
        else {
            agent.destination = targetPosition.position;
            animationControl.SetRun();
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("PlayerZone"))
        {
            //Debug.Log("Inside Player Zone");
            aimZone.gameObject.SetActive(true);
        }
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
        if (other.gameObject.CompareTag("PlayerZone")) {
            //Debug.Log("Inside Player Zone");
            aimZone.gameObject.SetActive(false);
        }
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Enemy")) {
            canAttack = false;
        }
    }

    private async void EnemyAttack() {
        if(isDead) return;
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
        GameObject weaponSpawn = Instantiate(enemyWeapon, positionSpawn, Quaternion.Euler(new Vector3(90, 0, 0)));

        //Set trang thai(chu the, tam ban, scale, vi tri)
        stateWeapon.positionSpawn = this.transform.position;
        Rigidbody weaponRb = weaponSpawn.GetComponent<Rigidbody>();
        weaponRb.GetComponent<ThrowWeapon>().SetStateWeapon(stateWeapon);
        //Nem vu khi theo huong
        if (weaponRb != null) {
            weaponRb.linearVelocity = directWeapon.normalized * speedWeapon * Time.fixedDeltaTime;
        }
        await Task.Delay(1000);
        animationControl.EndAttack();
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

    private void EnemyController_OnCharacterDead(object sender, EventArgs e) {
        agent.speed = 0;
        isDead = true;
    }

}
