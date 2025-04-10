using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAbility : MonoBehaviour
{
    [SerializeField] private WeaponObjects weaponObjects;
    [SerializeField] private GameObject weapon;
    [SerializeField] private int currentAbility;
    private PlayerController playerController;
    private Action playerAbility;

    /*Ability0 - Attack Behind*/
    [SerializeField] private float speedWeapon;
    private StateManager stateManager;
    private ThrowWeapon.StateWeapon stateWeapon;

    /*Ability1 - WeaponCirle*/
    [SerializeField] private GameObject weaponCircleObject;

    /*Ability2 - Bullet plus*/
    [SerializeField] private SkillObjects skillObjects;
    public event EventHandler OnPlayerChooseAbility2;

    /*Ability3 - Chasing Weapon*/
    public event EventHandler OnPlayerChooseAbility3;

    /*Ability4 - Continues Attack*/
    public event EventHandler OnplayerChooseAbility4;

    /*Ability5 - CrossAttack*/

    /*Ability8 - Growing Bullet*/
    public event EventHandler OnPlayerChooseAbility8;

    /*Ability9 - Piering */
    public event EventHandler OnPlayerChooseAbility9;

    /*Ability 10 - Revive*/
    public event EventHandler OnPlayerChooseAbility10;

    /*Ability 11 - Start Bigger*/
    public event EventHandler OnPlayerChooseAbility11;
    
    /*Ability 12 - Tripple Arrow*/
    public event EventHandler OnPlayerChooseAbility12;

    /*Ability 13 - Move Fast*/
    public event EventHandler OnPlayerChooseAbility13;
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
        Ability10();
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
            case 7:
                Ability7(); break;
            case 8:
                Ability8(); break;
            case 9:
                Ability9(); break;
            case 10:
                Ability10(); break;
            case 11:
                Ability11(); break;
            case 12:
                Ability12(); break;
            case 13:   
                Ability13(); break;
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
        OnPlayerChooseAbility2?.Invoke(this, EventArgs.Empty);
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
    private void Ability7() {
        CoinManager.Instance.isDoubleAward = true;
    }
    private void Ability8() {
        OnPlayerChooseAbility8?.Invoke(this, EventArgs.Empty);
    }
    private void Ability9() {
        OnPlayerChooseAbility9?.Invoke(this, EventArgs.Empty);
    }
    private void Ability10() {
        OnPlayerChooseAbility10?.Invoke(this, EventArgs.Empty);
    }

    private void Ability11() {
        OnPlayerChooseAbility11?.Invoke(this, EventArgs.Empty);
    }
    private void Ability12() {
        OnPlayerChooseAbility12?.Invoke(this, EventArgs.Empty);
    }

    private void Ability13() {
        OnPlayerChooseAbility13?.Invoke(this, EventArgs.Empty);
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
