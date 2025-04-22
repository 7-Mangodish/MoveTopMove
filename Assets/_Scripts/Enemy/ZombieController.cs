using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ZombieController : MonoBehaviour
{


    [Header("-----Zombie_Move-----")]
    public int hp;
    private Transform targetTransform;
    private NavMeshAgent agent;
    private AnimationControl animationControl;

    [Header("-----Zombie_Indicator-----")]
    private GameObject targetCanvas;
    private Camera mainCamera;
    public MaterialObjects materialObjects;
    [SerializeField] private GameObject indicatorContainer;
    [SerializeField] private Image indicatorIcon;
    [SerializeField] private SkinnedMeshRenderer skinCharacter;

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag(GameVariable.PLAYER_TAG)) {
            //Debug.Log("Player Dead");
            StateManager playerStateManager = other.gameObject.GetComponent<StateManager>();
            if(playerStateManager != null ) {
                playerStateManager.TriggerCharacterDead();
            }
            Destroy(this.gameObject);
            //ZombieDead();

        }
    }

    public void SetUpZombie() {
        RandomZombieMaterial();

        /*----------Move--------------*/
        agent = GetComponent<NavMeshAgent>();
        GameObject player = GameObject.FindGameObjectWithTag(GameVariable.PLAYER_TAG);

        if (player != null) {
            targetTransform = player.transform;
            agent.destination = targetTransform.position;
        }

        animationControl = GetComponent<AnimationControl>();

        /*-----------Indicator----------*/
        targetCanvas = GameObject.FindGameObjectWithTag("Canvas");

        mainCamera = Camera.main;
        indicatorIcon.color = skinCharacter.material.color;
        indicatorContainer.transform.SetParent(targetCanvas.transform);
        indicatorContainer.SetActive(false);
    }
    public void ZombieBehaviour() {
        if (targetTransform != null && targetTransform.gameObject.activeSelf 
            && targetTransform.gameObject.tag == GameVariable.PLAYER_TAG) {
            agent.destination = targetTransform.position;
            agent.speed = 0.5f;
            animationControl.SetZombieRun();
        }
        else {
            agent.destination = this.transform.position;
            agent.speed = 0;
            animationControl.SetDanceWin();
        }
        UpdateIndicator();
    }

    public void ZombieTakeDame(StateManager playerState) {
        hp -= 1;
        animationControl.PlayDeadEff();
        if (hp == 6 || hp == 3) {
            this.transform.localScale -= new Vector3(.25f, .25f, .25f);
        }
        if (hp == 0) {
            playerState.AddScore();
        }
        Destroy(this.gameObject);
    }

    /*Cap nhat vi tri indicator*/
    void UpdateIndicator() {
        Vector3 enemyPositionOnScreen = mainCamera.WorldToScreenPoint(this.transform.position);
        if (enemyPositionOnScreen.z < 0)
            return;
        if (enemyPositionOnScreen.x < 0 || enemyPositionOnScreen.x > Screen.width
            || enemyPositionOnScreen.y < 0 || enemyPositionOnScreen.y > Screen.height) {

            //Debug.Log(sizeDelta);
            enemyPositionOnScreen.x = Mathf.Clamp(enemyPositionOnScreen.x, 0, Screen.width - 150);
            enemyPositionOnScreen.y = Mathf.Clamp(enemyPositionOnScreen.y, 0, Screen.height - 150);
            enemyPositionOnScreen.z = 0;

            Vector3 direct = enemyPositionOnScreen - new Vector3(Screen.width / 2, Screen.height / 2, 0);
            float angle = Mathf.Atan2(direct.y, direct.x) * Mathf.Rad2Deg;
            angle = (angle < 0) ? (angle + 360) : angle;

            indicatorContainer.gameObject.SetActive(true);
            indicatorContainer.GetComponent<RectTransform>().position = enemyPositionOnScreen;

            indicatorIcon.rectTransform.rotation = Quaternion.Euler(0, 0, angle - 90);


        }
        else {
            indicatorContainer.gameObject.SetActive(false);
        }
    }
    void RandomZombieMaterial() {
        int randNumber = Random.Range(0, materialObjects.listMaterials.Length);
        skinCharacter.material = materialObjects.listMaterials[randNumber];
    }
}
