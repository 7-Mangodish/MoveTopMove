using System;
using System.Threading.Tasks;
using UnityEngine;

public class ThrowWeapon : MonoBehaviour
{

    [SerializeField] private float deltaAngle;
    public struct StateWeapon {
        public StateManager ownerStateManager;
        public float maxDistance;
        public float curScale;
        public Vector3 positionSpawn;
    }
    private StateWeapon stateWeapon;
    private void Start() {
        this.gameObject.transform.localScale += new Vector3(stateWeapon.curScale, stateWeapon.curScale, stateWeapon.curScale); 
        //Debug.Log(stateWeapon.ownerStateManager);
        //Debug.Log(stateWeapon.maxDistance);
        //Debug.Log(stateWeapon.curScale);
    }
    void Update()
    {
        Throw();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Zombie")) {
            AnimationControl animationControl = other.GetComponent<AnimationControl>();
            animationControl.PlayDeadEff();

            stateWeapon.ownerStateManager.AddScore();

            GameManager.Instance.DoZombieDead();

            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }
        else if (other.gameObject.CompareTag("Enemy")) {
            if(stateWeapon.ownerStateManager.CompareTag("Enemy") && 
                stateWeapon.ownerStateManager.transform.parent.name != other.transform.parent.name) {

                other.GetComponent<StateManager>().TriggerCharacterDead();
                stateWeapon.ownerStateManager.AddScore();
                GameManager.Instance.DoEnemyDead();

                Destroy(this.gameObject);
            }
            else if (stateWeapon.ownerStateManager.CompareTag("Player")) {

                other.gameObject.GetComponent<StateManager>().TriggerCharacterDead();
                GameManager.Instance.DoEnemyDead();
                stateWeapon.ownerStateManager.AddScore();

                Destroy(this.gameObject);

            }
        }
        else if (other.gameObject.CompareTag("Player")) {
            if (stateWeapon.ownerStateManager.CompareTag("Enemy")) {
                other.GetComponent<StateManager>().TriggerCharacterDead();
                stateWeapon.ownerStateManager.AddScore();

                GameManager.Instance.PlayerLose();
                Destroy(this.gameObject);
            }
        }
    }


    void  Throw() {
        if (Vector3.Distance(this.transform.position, stateWeapon.positionSpawn) > stateWeapon.maxDistance) {
            //Debug.Log("Out range");
            Destroy(this.gameObject);
        }
        this.transform.RotateAround(this.transform.position,Vector3.up, deltaAngle * Time.deltaTime);
    }

    public void SetStateWeapon(StateWeapon s) {
        this.stateWeapon.ownerStateManager = s.ownerStateManager;
        this.stateWeapon.maxDistance = s.maxDistance;
        this.stateWeapon.curScale = s.curScale;
        this.stateWeapon.positionSpawn = s.positionSpawn;
    }

}
