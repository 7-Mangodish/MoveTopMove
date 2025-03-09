using System;
using System.Threading.Tasks;
using UnityEngine;

public class ThrowWeapon : MonoBehaviour
{

    private Vector3 targetPosition;
    [SerializeField] private float deltaAngle;
    public struct StateWeapon {
        public StateManager ownerStateManager;
        public float maxDistance;
        public float curScale;
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

        if (stateWeapon.ownerStateManager.CompareTag("Player") && other.gameObject.CompareTag("Enemy")) {

            //other.gameObject.GetComponent<EnemyController>().enabled = false;
            other.gameObject.GetComponent<StateManager>().TriggerCharacterDead();
            //GameManager.Instance.SpawnEnemy();

            stateWeapon.ownerStateManager.AddScore();

            Destroy(this.gameObject);
        }
        if (stateWeapon.ownerStateManager.CompareTag("Enemy") && 
            (stateWeapon.ownerStateManager.gameObject.name != other.gameObject.name)) {

            //Debug.Log(stateWeapon.ownerStateManager.gameObject.name + " " + other.gameObject.name);
            if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Enemy")) {
                //other.gameObject.GetComponent<AnimationControl>().TestParticle();

                stateWeapon.ownerStateManager.AddScore();

                Destroy(this.gameObject);
                //Debug.Log("Enemy Attack: " + other.gameObject);
            }
        }

    }


    void  Throw() {
        if (Vector3.Distance(this.transform.position, stateWeapon.ownerStateManager.transform.position) > stateWeapon.maxDistance + .25) {
            Destroy(this.gameObject);
        }
        this.transform.RotateAround(this.transform.position,Vector3.up, deltaAngle * Time.deltaTime);
    }

    public void SetStateWeapon(StateWeapon s) {
        this.stateWeapon.ownerStateManager = s.ownerStateManager;
        this.stateWeapon.maxDistance = s.maxDistance;
        this.stateWeapon.curScale = s.curScale;
    }
}
