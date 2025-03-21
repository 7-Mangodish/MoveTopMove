using UnityEngine;
using static ThrowWeapon;

public class WeaponCircle : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private WeaponObjects weaponObjects;
    [SerializeField] private float baseRadius;
    private StateManager stateManager;
    void Start()
    {
        stateManager = playerTransform.gameObject.GetComponent<StateManager>();
        stateManager.OnCharacterLevelUp += WeaponCircle_OnCharacterLevelUp;
        SetUpWeapon();
    }



    // Update is called once per frame
    void Update()
    {
        RotateAbility();
    }
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Zombie")) {
            AnimationControl animationControl = other.GetComponent<AnimationControl>();
            animationControl.PlayDeadEff();
            stateManager.AddScore();

            Destroy(other.gameObject);
        }
    }

    private void WeaponCircle_OnCharacterLevelUp(object sender, StateManager.OnCharacterLevelUpArg e) {
        Debug.Log("Level Up");
    }
    void RotateAbility() {
        float scaledRadius = baseRadius * playerTransform.localScale.x;
        float angle = Time.time * 180; 

        Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad),0, Mathf.Sin(angle * Mathf.Deg2Rad)) * scaledRadius;
        this.transform.position = playerTransform.position + offset;

        this.transform.RotateAround(this.transform.position,Vector3.up, 360*Time.deltaTime);
        //this.transform.RotateAround(this.transform.position, Vector3.up, 720 * Time.deltaTime);
        //this.transform.RotateAround(playerTransform.position, Vector3.up, 90 * Time.deltaTime);
    }

    private void SetUpWeapon() {
        int curWeapon = PlayerPrefs.GetInt("CurWeapon");
        Debug.Log("Vu khi duoc chon: " + curWeapon);

        WeaponShopManager.SaveData data = WeaponShopManager.Instance.GetWeaponData(curWeapon);
        Mesh mesh = weaponObjects.GetMeshWeapon(curWeapon, data.skinIndex);
        Material[] materials = weaponObjects.GetListMaterials(curWeapon, data.skinIndex);

        this.GetComponent<MeshFilter>().mesh = mesh;
        this.GetComponent<MeshRenderer>().materials = materials;
    }
}
