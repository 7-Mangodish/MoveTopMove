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

    public event EventHandler OnZombieDead;
    private void Awake() {
        targetTransform = GameObject.FindGameObjectWithTag("Player").transform;
        animationControl = GetComponent<AnimationControl>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Start() {
        if (animationControl != null)
            animationControl.SetZombieRun();
        else
            Debug.Log("Animation control is null");
        agent.destination = targetTransform.position;
        GameManager.Instance.OnPlayerLose += ZombieController_OnPlayerLose;
    }

    void Update() {
        if(targetTransform != null)
            agent.destination = targetTransform.position;
        else {
            agent.destination = this.transform.position;
            agent.speed = 0;
        }

    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Player")) {
            Debug.Log("Player Dead");
            StateManager playerStateManager = other.gameObject.GetComponent<StateManager>();
            if(playerStateManager != null ) {
                playerStateManager.TriggerCharacterDead();
            }

            animationControl.PlayDeadEff();
            Destroy(this.gameObject);
            OnZombieDead?.Invoke(this, EventArgs.Empty);
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

    private void ZombieController_OnPlayerLose(object sender, EventArgs e) {
        if(animationControl != null)
            animationControl.SetDanceWin();
    }
}
