using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class AnimationControl : MonoBehaviour
{
    [SerializeField] private Transform deadPrefab;
    [SerializeField] private Transform levelUpPrefab;
    private Animator animator;
    private Color characterColor;
    public enum state {
        IsIdle,
        IsAttack,
        IsDead,
        IsDance,
        IsWalk,
    }
    //public state currentState = state.Idle;

    private void Awake() {
        animator = GetComponent<Animator>();


    }
    void Start() {
        characterColor = GetComponentInChildren<SkinnedMeshRenderer>().material.color;
        //animator.SetBool("IsIdle", false);
        if (SceneManager.GetActiveScene().buildIndex == 0) {
            HomePageManager homeManager = GameObject.FindFirstObjectByType<HomePageManager>();
            homeManager.OnShopping += Animation_OnShopping;
            homeManager.OnOutShopping += Animation_OnOutShopping;
        }
    }

    private void SetState(state state, bool val) {
        animator.SetBool(state.ToString(), val);
    }
    private void Animation_OnShopping(object sender, System.EventArgs e) {
        SetState(state.IsDance, true);
    }

    private void Animation_OnOutShopping(object sender, System.EventArgs e) {
        SetState(state.IsDance, false);
    }

    public void SetZombieRun() {
        SetState(state.IsWalk, false);
    }
    public void SetIdle() {
        SetState(state.IsIdle, true);
    }
    public void SetRun() {
        SetState(state.IsIdle, false);
    }
    public void SetAttack() {
        SetState(state.IsAttack, true);
    }
    public void SetDead() {
        SetState(state.IsDead, true);
        PlayDeadEff();
    }
    public void EndAttack() {
        SetState(state.IsAttack, false);
    }
    public void PlayDeadEff() {
        Vector3 positionSpawn = new Vector3(this.transform.position.x, this.transform.position.y + .2f, this.transform.position.z);
        Transform pref = Instantiate(deadPrefab, positionSpawn, Quaternion.identity);
        ParticleSystem myParticleSystem = pref.GetComponent<ParticleSystem>();
        ParticleSystem.ColorOverLifetimeModule colorModule = myParticleSystem.colorOverLifetime;
        
        colorModule.color = 
            new ParticleSystem.MinMaxGradient(new Color(characterColor.r, characterColor.g, characterColor.b, .5f), 
            new Color(characterColor.r, characterColor.g, characterColor.b, 1));
    }

    public void PlayLevelUpEff() {
        Vector3 positionSpawn = new Vector3(this.transform.position.x, this.transform.position.y + .2f, this.transform.position.z);
        Transform pref = Instantiate(levelUpPrefab, positionSpawn, Quaternion.identity);
        ParticleSystem myParticleSystem = pref.GetComponent<ParticleSystem>();
        ParticleSystem.ColorOverLifetimeModule colorModule = myParticleSystem.colorOverLifetime;

        colorModule.color = characterColor;
    }
}
