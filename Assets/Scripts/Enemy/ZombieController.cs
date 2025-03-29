using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Android;

public class ZombieController : MonoBehaviour
{

    [SerializeField] private GameObject aimZone;

    [Header("Enemy Move")]
    private Transform targetTransform;
    private NavMeshAgent agent;
    private AnimationControl animationControl;

    [SerializeField] private int hp;

    public event EventHandler OnZombieDead;
    private void Awake() {
        targetTransform = GameObject.FindGameObjectWithTag("Player").transform;
        animationControl = GetComponent<AnimationControl>();
        if (animationControl == null)
            Debug.Log("Animation null");
        agent = GetComponent<NavMeshAgent>();
    }

    void Start() {
        agent.destination = targetTransform.position;
    }

    void Update() {
        if(targetTransform != null) {
            agent.destination = targetTransform.position;
            agent.speed = 0.5f;
            animationControl.SetZombieRun();
        }
        else {
            agent.destination = this.transform.position;
            agent.speed = 0;
            animationControl.SetDanceWin();
        }

    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Player")) {
            //Debug.Log("Player Dead");
            StateManager playerStateManager = other.gameObject.GetComponent<StateManager>();
            if(playerStateManager != null ) {
                playerStateManager.TriggerCharacterDead();
            }
            ZombieDead();

        }
    }
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("PlayerZone")) {
            //Debug.Log("Inside Player Zone");
            aimZone.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("PlayerZone")) {
            //Debug.Log("Inside Player Zone");
            aimZone.gameObject.SetActive(false);
        }
    }

    private void ZombieDead() {
        OnZombieDead?.Invoke(this, EventArgs.Empty);
        Destroy(this.gameObject);
    }

    public void ZombieTakeDame(StateManager playerState) {
        hp -= 1;
        animationControl.PlayDeadEff();
        if (hp == 6 || hp == 3) {
            this.transform.localScale -= new Vector3(.25f, .25f, .25f);
        }
        if (hp == 0) {
            ZombieDead();
            playerState.AddScore();
        }

    }
    private void OnDestroy() {
        GameManager.Instance.DoZombieDead();
    }
}
