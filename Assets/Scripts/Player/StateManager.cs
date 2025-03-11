using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms.Impl;

public class StateManager : MonoBehaviour
{

    private int currentScore;
    public int CurrentScore { get { return currentScore; } }
    private int addingScore;
    public bool IsLevelUp;

    [Header("Weapon's State")]
    [SerializeField] private float deltaScaleWeapon;
    [SerializeField] private GameObject maxDistancePoint;
    //[SerializeField] private GameObject scorePrefabs;
    private ThrowWeapon.StateWeapon stateWeapon;

    [Header("Character's Control")]
    [SerializeField] private GameObject characterController;
    private AnimationControl animationControl;

    public event EventHandler<int> OnCharacterTakeScore;
    public event EventHandler OnCharacterDead;

    private int currentLevel;
    public class OnCharacterLevelUpArg : EventArgs {
        public float currentLevel;
        public float deltaPositionY;
    }
    public event EventHandler<OnCharacterLevelUpArg> OnCharacterLevelUp;

    public bool isDead = false;
    private void Awake() {
        addingScore = 1;
        currentScore = 0;
        currentLevel = 1;

        stateWeapon.ownerStateManager = this;
        stateWeapon.maxDistance = Vector3.Distance(this.transform.position, maxDistancePoint.transform.position);
        stateWeapon.curScale = 0;
        deltaScaleWeapon = 3;

        animationControl  = GetComponent<AnimationControl>();
    }
    public void AddScore() {
        if (isDead) return;

        this.currentScore += addingScore;
        OnCharacterTakeScore?.Invoke(this, this.currentScore);
        //GameObject spawnPrefab= Instantiate(scorePrefabs, this.transform.position, Quaternion.identity);
        //Destroy(spawnPrefab, 1.5f);
        //Debug.Log(stateWeapon.ownerStateManager + ": " + this.currentScore);
        if (currentScore%2 == 0 && currentScore !=0) {
            IsLevelUp = true;
            currentLevel++;

            animationControl.PlayLevelUpEff();
            OnCharacterLevelUp?.Invoke(this, new OnCharacterLevelUpArg {
                currentLevel = this.currentLevel,
                deltaPositionY = 0.05f
            });

            //Cap nhat scale cua nhan vat, cap nhat tam danh va scale cua vu khi
            this.transform.localScale += new Vector3(0.05f, 0.05f, 0.05f);
            if (this.gameObject.CompareTag("Player")) {
                GameUIManager.Instance.DisplayPlayerScale(this.transform.localScale.x * 5f);
            }


            stateWeapon.curScale += deltaScaleWeapon;
            stateWeapon.maxDistance = Vector3.Distance(this.transform.position, maxDistancePoint.transform.position);

            // Cap nhat cammera
            if (this.gameObject.CompareTag("Player"))
                UpdateCameraPosition();
        }
    }
    
    private void UpdateCameraPosition() {
        CinemachineCamera cam = FindFirstObjectByType<CinemachineCamera>();
        cam.Lens.FieldOfView += 10;
    }

    public ThrowWeapon.StateWeapon GetStateWeapon() {
        return stateWeapon;
    }

    public void TriggerCharacterDead() {
        this.isDead = true;

        //Animation
        OnCharacterDead?.Invoke(this, EventArgs.Empty);
        animationControl.SetDead();


        //Destroy Gameobject
        if (this.gameObject.CompareTag("Enemy")) {
            Destroy(this.transform.parent.gameObject, 2);
        }
        else
            Destroy(this.gameObject, 2);
    }
}
