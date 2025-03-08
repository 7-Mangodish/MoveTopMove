using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class StateManager : MonoBehaviour
{

    private int currentScore;
    public int CurrentScore { get { return currentScore; } }
    private int addingScore;
    public bool IsLevelUp;

    [SerializeField] private float deltaScaleWeapon;
    [SerializeField] private GameObject maxDistancePoint;
    [SerializeField] private GameObject scorePrefabs;
    private ThrowWeapon.StateWeapon stateWeapon;
    private AnimationControl animationControl;

    public event EventHandler OnPlayerTakeScore;
    private void Awake() {
        addingScore = 1;
        currentScore = 0;

        stateWeapon.ownerStateManager = this;
        stateWeapon.maxDistance = Vector3.Distance(this.transform.position, maxDistancePoint.transform.position);
        stateWeapon.curScale = 0;
        deltaScaleWeapon = 3;

        animationControl  = GetComponent<AnimationControl>();
    }
    public void AddScore() {
        this.currentScore += addingScore;
        OnPlayerTakeScore?.Invoke(this, EventArgs.Empty);
        Instantiate(scorePrefabs, this.transform.position, Quaternion.identity);
        //Debug.Log(stateWeapon.ownerStateManager + ": " + this.currentScore);
        if (currentScore%5 == 0 && currentScore !=0) {
            IsLevelUp = true;

            animationControl.PlayLevelUpEff();

            //Cap nhat scale cua nhan vat, cap nhat tam danh va scale cua vu khi
            this.transform.localScale += new Vector3(0.05f, 0.05f, 0.05f);
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
}
