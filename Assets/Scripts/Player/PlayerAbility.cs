using System;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static ThrowWeapon;

public class PlayerAbility : MonoBehaviour
{
    [SerializeField] private WeaponObjects weaponObjects;
    [SerializeField] private GameObject weapon;
    [SerializeField] private int currentAbility;
    private PlayerController playerController;
    private Action playerAbility;

    /*Ability0*/
    [SerializeField] private float speedWeapon;
    private StateManager stateManager;
    private ThrowWeapon.StateWeapon stateWeapon;

    /*Ability1 - Attack Back*/
    [SerializeField] private GameObject weaponCircleObject;

    /*Ability2 - WeaponCircle*/
    [SerializeField] private SkillObjects skillObjects;

    /*Ability3 - Chasing Weapon*/
    public event EventHandler OnPlayerChooseAbility3;

    /*Ability4 - Continues Attack*/
    public event EventHandler OnplayerChooseAbility4;

    /*Ability5 - CrossAttack*/
    private void Awake() {
        stateManager = GetComponent<StateManager>();
        playerController = GetComponent<PlayerController>();
    }



    void Start()
    {
        if(SceneManager.GetActiveScene().buildIndex == 1) {
            StartPanelManager.Instance.OnStartZombieMode += PlayerAbility_OnStartZombieMode;
            StartPanelManager.Instance.OnPlayerChooseAbility += PlayerAbility_OnPlayerChooseAbility;

            playerController.OnPlayerAttack += PlayerAbility_OnPlayerAttack;
            stateWeapon = stateManager.GetStateWeapon();
        }
        
    }

    private void PlayerAbility_OnPlayerChooseAbility(object sender, int abilityIndex) {
        Debug.Log(abilityIndex);
        currentAbility = abilityIndex;
        switch (currentAbility) {
            case 1:
                Ability1(); break;
            case 2:
                Ability2(); break;
            case 3:
                Ability3(); break;
            case 4:
                Ability4(); break;

        }
    }

    private void PlayerAbility_OnStartZombieMode(object sender, EventArgs e) {
        currentAbility = -1;
    }

    private void PlayerAbility_OnPlayerAttack(object sender, System.EventArgs e) {
        switch (currentAbility) {
            case 0:
                Ability0(); break;
            case 5:
                Ability5(); break;
            case 6: 
                Ability6(); break;
        }
    }
    
    private void Ability0() {

        Vector3 positionSpawn = new Vector3(this.transform.position.x,
            this.transform.position.y + 0.2f, this.transform.position.z);
        GameObject weaponSpawn = Instantiate(weapon, positionSpawn, Quaternion.Euler(new Vector3(90, 0, 0)));
        weaponSpawn.transform.localScale = new Vector3(6, 6, 6);

        // Kiem tra xem da len cap chua, neu co thi cap nhat trang thai cua vu khi
        if (stateManager.IsLevelUp) {
            stateWeapon = stateManager.GetStateWeapon();
        }

        //Set trang thai(chu the, tam ban, scale)
        Rigidbody weaponRb = weaponSpawn.GetComponent<Rigidbody>();
        stateWeapon.positionSpawn = this.transform.position;
        weaponRb.GetComponent<ThrowWeapon>().SetStateWeapon(stateWeapon);

        //Nem vu khi theo huong
        if (weaponRb != null) {
            weaponRb.linearVelocity = -1 * this.transform.forward * speedWeapon * Time.fixedDeltaTime;
        }

    }

    private void Ability1() {
        weaponCircleObject.gameObject.SetActive(true);
    }

    private void Ability2() {
        skillObjects.UpgradeWeaponCount();
        StartPanelManager.Instance.TriggerOnPlayerUpgradeSkill();
        // Can them bien luu weaponCount ban dau
        // Khi ket thuc man choi thi tra ve so luong weapon ban dau
    }

    private void Ability3() {
        OnPlayerChooseAbility3?.Invoke(this, EventArgs.Empty);
    }

    private void Ability4() {
        OnplayerChooseAbility4?.Invoke(this, EventArgs.Empty);
    }

    private void Ability5() {
        Vector3 positionSpawn = new Vector3(this.transform.position.x,
            this.transform.position.y + 0.2f, this.transform.position.z);
        for(int i=0; i<2; i++) {
            GameObject weaponSpawn = Instantiate(weapon, positionSpawn, Quaternion.Euler(new Vector3(90, 0, 0)));
            weaponSpawn.transform.localScale = new Vector3(6, 6, 6);

            // Kiem tra xem da len cap chua, neu co thi cap nhat trang thai cua vu khi
            if (stateManager.IsLevelUp) {
                stateWeapon = stateManager.GetStateWeapon();
            }

            //Set trang thai(chu the, tam ban, scale)
            Rigidbody weaponRb = weaponSpawn.GetComponent<Rigidbody>();
            stateWeapon.positionSpawn = this.transform.position;
            weaponRb.GetComponent<ThrowWeapon>().SetStateWeapon(stateWeapon);

            //Nem vu khi theo huong
            if (weaponRb != null) {
                if(i == 0)
                    weaponRb.linearVelocity = this.transform.right * speedWeapon * Time.fixedDeltaTime;
                else
                    weaponRb.linearVelocity = -1 * this.transform.right * speedWeapon * Time.fixedDeltaTime;
            }
        }

    }   

    private void Ability6() {
        Debug.Log("Ability6 is called");
        Vector3 positionSpawn = new Vector3(this.transform.position.x,
            this.transform.position.y + 0.2f, this.transform.position.z);
        for (int i = 0; i <=1; i++) {
            GameObject weaponSpawn = Instantiate(weapon, positionSpawn, Quaternion.Euler(new Vector3(90, 0, 0)));
            weaponSpawn.transform.localScale = new Vector3(6, 6, 6);
            if (weaponSpawn != null)
                Debug.Log("Weapon is spawned");

            // Kiem tra xem da len cap chua, neu co thi cap nhat trang thai cua vu khi
            if (stateManager.IsLevelUp) {
                stateWeapon = stateManager.GetStateWeapon();
            }

            //Set trang thai(chu the, tam ban, scale)
            Rigidbody weaponRb = weaponSpawn.GetComponent<Rigidbody>();
            stateWeapon.positionSpawn = this.transform.position;
            weaponRb.GetComponent<ThrowWeapon>().SetStateWeapon(stateWeapon);

            //Nem vu khi theo huong
            if (weaponRb != null) {
                if(i == 0) {
                    Vector3 direct = Quaternion.AngleAxis(60, Vector3.up) * this.transform.forward;
                    weaponRb.linearVelocity = speedWeapon * direct.normalized * Time.fixedDeltaTime;
                }
                else {
                    Vector3 direct = Quaternion.AngleAxis(-60, Vector3.up) * this.transform.forward;
                    weaponRb.linearVelocity = speedWeapon * direct.normalized * Time.fixedDeltaTime;
                }
            }
        }
    }
    //private void SetUpWeapon(GameObject weaponObject) {
    //    int curWeapon = PlayerPrefs.GetInt("CurWeapon");
    //    Debug.Log("Vu khi duoc chon: " + curWeapon);

    //    WeaponShopManager.SaveData data = WeaponShopManager.Instance.GetWeaponData(curWeapon);
    //    Mesh mesh = weaponObjects.GetMeshWeapon(curWeapon, data.skinIndex);
    //    Material[] materials = weaponObjects.GetListMaterials(curWeapon, data.skinIndex);

    //    weaponObject.GetComponent<MeshFilter>().mesh = mesh;
    //    weaponObject.GetComponent<MeshRenderer>().materials = materials;
    //}
}
