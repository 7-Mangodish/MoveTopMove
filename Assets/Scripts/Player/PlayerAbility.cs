using System;
using UnityEngine;
using static ThrowWeapon;

public class PlayerAbility : MonoBehaviour
{
    [SerializeField] private WeaponObjects weaponObjects;
    [SerializeField] private GameObject weapon;
    [SerializeField] private int currentAbility;
    private PlayerController playerController;
    private Action action;

    [Header("Ability0")]
    [SerializeField] private float speedWeapon;
    private StateManager stateManager;
    private ThrowWeapon.StateWeapon stateWeapon;

    [Header("Ability1")]
    [SerializeField] private GameObject weaponCircleObject;

    [Header("Ability2")]
    [SerializeField] private SkillObjects skillObjects;
    private void Awake() {
        stateManager = GetComponent<StateManager>();
        playerController = GetComponent<PlayerController>();
    }



    void Start()
    {
        //Xu li khi player chon ki nang
        StartPanelManager.Instance.OnPlayerSelectAbility += PlayerAbility_OnPlayerSelectAbility;

        //Xu li khi player Attack
        playerController.OnPlayerAttack += PlayerAbility_OnPlayerAttack;
        
        // Ability0
        stateWeapon = stateManager.GetStateWeapon();

        //Ability1
        //  Ability1();

        //action += Ability1;
    }

    private void PlayerAbility_OnPlayerSelectAbility(object sender, int e) {
        //currentAbility = e;
        action += Ability0;
    }
    private void PlayerAbility_OnPlayerAttack(object sender, System.EventArgs e) {
        //action();
        Debug.Log("Back Acttack");
        //Ability0();
    }
    
    /* Attack Back */
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
        weaponRb.GetComponent<ThrowWeapon>().SetStateWeapon(stateWeapon);

        //Nem vu khi theo huong
        if (weaponRb != null) {
            weaponRb.linearVelocity = -1 * this.transform.forward * speedWeapon * Time.fixedDeltaTime;
        }

    }

    /*Bullet around*/
    private void Ability1() {
        weaponCircleObject.gameObject.SetActive(true);
    }

    /*Bullet plus*/
    private void Ability2() {
        skillObjects.UpgradeWeaponCount();
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
