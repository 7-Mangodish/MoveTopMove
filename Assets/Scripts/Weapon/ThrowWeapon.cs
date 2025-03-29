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

    public bool isGrowing = false;
    public bool isPiering = false;
    private void Start() {
        this.gameObject.transform.localScale += 
            new Vector3(stateWeapon.curScale, stateWeapon.curScale, stateWeapon.curScale); 
    }
    void Update()
    {
        Throw();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Zombie")) {

            //stateWeapon.ownerStateManager.AddScore();
            other.GetComponent<ZombieController>().ZombieTakeDame(stateWeapon.ownerStateManager);

            if(!isPiering)
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

                Debug.Log("Enemy Take Dame");
            }
        }
        else if (other.gameObject.CompareTag("Player")) {
            if (stateWeapon.ownerStateManager.CompareTag("Enemy")) {
                other.GetComponent<StateManager>().TriggerCharacterDead();
                stateWeapon.ownerStateManager.AddScore();

                //GameManager.Instance.PlayerLose();
                Destroy(this.gameObject);
            }
        }
    }


    void  Throw() {
        if (isGrowing)
            this.transform.localScale += new Vector3(.15f, .15f, .15f); 

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
