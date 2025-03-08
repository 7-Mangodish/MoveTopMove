using System;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float speed;
    [SerializeField] private float deltaAngle;
    [SerializeField] private Joystick joystick;
    private Vector3 direct;
    private Rigidbody rb;

    private AnimationControl animationControl;

    [Header("Attack")]
    [SerializeField] private GameObject weapon;
    [SerializeField] private float speedWeapon;
    [SerializeField] private float timeAttack;
    [SerializeField] private float heightAttack;
    private float timeAttackDuration;

    private Vector3 targetPosition;
    private bool canAttack;

    [Header("Level Up")]
    private StateManager stateManager;
    private ThrowWeapon.StateWeapon stateWeapon;

    [Header("Transparent Obstacle")]
    [SerializeField] private Material transparentMaterial;
    private Material oldMaterial;

    private bool startGame;
    private void Awake() {
        rb = GetComponent<Rigidbody>();
        animationControl = GetComponent<AnimationControl>();
        stateManager = GetComponent<StateManager>();
        startGame = false;
    }

    private void Start() {
        timeAttackDuration = 3;
        stateWeapon = stateManager.GetStateWeapon();

    }

    void Update() {
        if (!startGame && joystick.Horizontal != 0)
            startGame = true;
        if (startGame == true)
            GameUIManager.Instance.StartGame();
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
        if (other.gameObject.CompareTag("Enemy"))
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
        if (other.gameObject.CompareTag("Enemy")) {
            Debug.Log("Enemy's position in Trigger Stay:" + other.transform.position);
            targetPosition = other.transform.position;
        }

    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Enemy"))
            canAttack = false;

        if (other.gameObject.CompareTag("Obstacle")) {
            other.GetComponent<MeshRenderer>().material = oldMaterial;
        }
    }
    private async void Attack() {
        
        // Xac dinh vi tri spawn va huong nem
        Vector3 positionSpawn = new Vector3(this.transform.position.x, 
            this.transform.position.y + 0.2f, this.transform.position.z);
        Vector3 directWeapon = targetPosition - this.transform.position;
        directWeapon.y = 0;
        Debug.Log("Enenmy's position: " + targetPosition);
        Debug.Log("Player's position: " + this.transform.position);
        RotateCharacter(directWeapon);

        // Kiem tra xem da len cap chua, neu co thi cap nhat trang thai cua vu khi
        if (stateManager.IsLevelUp) {
            stateWeapon = stateManager.GetStateWeapon();
            stateManager.IsLevelUp = false;
            heightAttack *= transform.localScale.x;
        }

        //Khoi tao vu khi
        GameObject weaponSpawn = Instantiate(weapon, positionSpawn, Quaternion.Euler(new Vector3(90, 0, 0)));

        //Set trang thai(chu the, tam ban, scale)
        Rigidbody weaponRb = weaponSpawn.GetComponent<Rigidbody>();
        weaponRb.GetComponent<ThrowWeapon>().SetStateWeapon(stateWeapon);
        //Nem vu khi theo huong
        if (weaponRb != null) {
            weaponRb.linearVelocity = directWeapon.normalized * speedWeapon * Time.fixedDeltaTime;
        }

        await Task.Delay(800);
        animationControl.EndAttack();
    }
    private void RotateCharacter(Vector3 direct) {
        if (direct == Vector3.zero)
            return;

        Debug.Log(direct);
        Debug.Log(direct.normalized);
        Quaternion rot = Quaternion.LookRotation(direct.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(this.transform.rotation, rot, deltaAngle);
    }

}
