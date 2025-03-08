using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

public class AnimationControl : MonoBehaviour
{
    [SerializeField] private Transform deadPrefab;
    [SerializeField] private Transform levelUpPrefab;
    private Animator animator;
    private Color characterColor;
    public enum state {
        IsIdle,
        IsAttack,
        IsDead
    }
    //public state currentState = state.Idle;
    void Start() {
        animator = GetComponent<Animator>();
        //animator.SetBool("IsIdle", false);
        characterColor = GetComponentInChildren<SkinnedMeshRenderer>().material.color;
        
    }

    private void SetState(state state, bool val) {
        animator.SetBool(state.ToString(), val);
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
        if (this.gameObject.CompareTag("Enemy")) {
            Destroy(this.gameObject.GetComponent<EnemyController>());
        }
        Destroy(this.gameObject,2);
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

    public void TestParticle() {
        PlayDeadEff();
    }
}
