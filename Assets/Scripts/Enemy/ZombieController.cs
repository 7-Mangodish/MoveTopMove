using System;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{

    [SerializeField] private GameObject aimZone;
    [Header("Enemy Move")]
    private Transform targetTransform;
    private NavMeshAgent agent;
    private AnimationControl animationControl;

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
    }

    void Update() {
        agent.destination = targetTransform.position;

    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Player")) {
            Debug.Log("PlayerDead");
            animationControl.PlayDeadEff();
            Destroy(this.gameObject);
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
}
