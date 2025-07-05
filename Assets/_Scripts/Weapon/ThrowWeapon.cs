using System;
using System.Threading.Tasks;
using UnityEngine;

public class ThrowWeapon : MonoBehaviour
{

    [Header("-----Ability-----")]
    public bool isGrowing = false;
    public bool isPiering = false;
    public bool isUlti = false;
    [Header("____Attribute____")]
    [SerializeField] private float deltaAngle;
    private StateWeapon stateWeapon;
    private Rigidbody rb;
    public SoundController soundController;
    private void Start() {
        rb = GetComponent<Rigidbody>();
        this.gameObject.transform.localScale += 
            new Vector3(stateWeapon.curScale, stateWeapon.curScale, stateWeapon.curScale); 
    }
    void Update()
    {
        //Throw();
    }

    private void FixedUpdate() {
        Throw();
    }
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Zombie")) {
            //
            other.GetComponent<ZombieController>().ZombieTakeDame(stateWeapon.ownerStateManager);
            soundController.PlaySound(SoundData.SoundName.weapon_hit);
            //
            if(!isPiering)
                Destroy(this.gameObject);
            else
                this.gameObject.GetComponent<BoxCollider>().enabled = false;

        }
        else if (other.gameObject.CompareTag("Enemy")) {
            if(stateWeapon.ownerStateManager.CompareTag("Enemy") && 
                stateWeapon.ownerStateManager.transform.parent.name != other.transform.parent.name) {              
                other.GetComponent<StateManager>().TriggerCharacterDead();
                stateWeapon.ownerStateManager.AddScore();
                if (!isPiering)
                    Destroy(this.gameObject);
            }
            else if (stateWeapon.ownerStateManager.CompareTag("Player")) {
                soundController.PlaySound(SoundData.SoundName.weapon_hit);
                //
                other.gameObject.GetComponent<StateManager>().TriggerCharacterDead();
                //
                stateWeapon.ownerStateManager.AddScore();
                //
                if (!isPiering && !stateWeapon.isBoom)
                    Destroy(this.gameObject);
            }
        }
        else if (other.gameObject.CompareTag("Player")) {
            if (stateWeapon.ownerStateManager.CompareTag("Enemy")) {      
                //
                other.GetComponent<StateManager>().TriggerCharacterDead();
                stateWeapon.ownerStateManager.AddScore();
                //
                if (!isPiering)
                    Destroy(this.gameObject);
            }
        }
    }


    void  Throw() {
        if (isUlti) {
            this.transform.localScale += new Vector3(.2f, .2f, .2f);
        }
        else {
            this.transform.RotateAround(this.transform.position, Vector3.up, deltaAngle * Time.deltaTime);
            if(isGrowing)
                this.transform.localScale += new Vector3(.1f, .1f, .1f);
        }
        //
        if (Vector3.Distance(this.transform.position, stateWeapon.positionSpawn) > stateWeapon.maxDistance) {
            if(stateWeapon.isBoom && !isUlti) {
                Vector3 backVector = (PlayerController.Instance.transform.position - this.transform.position);
                rb.linearVelocity = backVector.normalized * 100 * Time.fixedDeltaTime;
                stateWeapon.positionSpawn = this.transform.position;
                stateWeapon.isBoom = false;
            }
            else
                Destroy(this.gameObject);

        }
    }

    public void SetStateWeapon(StateWeapon s) {
        this.stateWeapon.ownerStateManager = s.ownerStateManager;
        this.stateWeapon.maxDistance = s.maxDistance;
        this.stateWeapon.curScale = s.curScale;
        this.stateWeapon.positionSpawn = s.positionSpawn;
        this.stateWeapon.isBoom = s.isBoom;
        if (isUlti) {
            isPiering = true;
            this.stateWeapon.maxDistance += 1.5f;
        }
    }
    public struct StateWeapon {
        public StateManager ownerStateManager;
        public float maxDistance;
        public float curScale;
        public Vector3 positionSpawn;
        public bool isBoom;
    }
}
