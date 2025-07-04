using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class AnimationControl : MonoBehaviour
{
    [SerializeField] private Transform deadPrefab;
    [SerializeField] private Transform levelUpPrefab;
    public Animator animator;
    private Color characterColor;
    public enum state {
        IsIdle,
        IsAttack,
        IsDead,
        IsWin,
        IsDance,
        IsWalk,
        IsRun,
        IsUlti
    }
    public state currentState = state.IsIdle;

    private void Awake() {
        animator = GetComponent<Animator>();
        if(this.gameObject.CompareTag(GameVariable.PLAYER_TAG))
            animator.SetFloat("AttackSpeed", 1.5f);

    }

    private void SetState(state state, bool val) {
        animator.SetBool(state.ToString(), val);    
    }


    public void SetZombieRun() {
        SetState(state.IsWalk, false);
        SetState(state.IsWin, false);
    }
    public void SetIdle() {
        SetState(state.IsIdle, true);
    }
    public void SetRun() {
        SetState(state.IsIdle, false);
        SetState(state.IsAttack, false);
        currentState = state.IsRun;
    }
    public void SetAttack() {
        SetState(state.IsAttack, true);
        SetState(state.IsUlti, false);
        currentState = state.IsAttack;
    }

    public void SetUlti() {
        //Debug.LogWarning("Ulti");
        SetState(state.IsUlti, true);
        SetState(state.IsAttack, true);
    }
    public void SetDead() {
        SetState(state.IsDead, true);
        currentState = state.IsDead;

        PlayDeadEff();
    }
    public void SetDanceWin() {
        SetState(state.IsWin, true);
    }

    public void SetDance() {
        SetState(state.IsDance, true);
    }
    public void StopDance() {
        SetState(state.IsDance, false);
    }
    public void SetEndAttack() {
        SetState(state.IsAttack, false);
        SetState(state.IsUlti, false);
    }
    public void PlayDeadEff() {
        characterColor = GetComponentInChildren<SkinnedMeshRenderer>().material.color;

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

    public void SetSpeedMultiplayer() {
        animator.SetFloat("AttackSpeed", 2.5f);
    }
}
