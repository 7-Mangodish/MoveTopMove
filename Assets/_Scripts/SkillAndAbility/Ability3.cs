using UnityEngine;

public class Ability3 : MonoBehaviour
{
    [SerializeField] private LayerMask zombieMark;
    [SerializeField] private float chasingSpeed;
    private Transform weaponTransform;
    private SphereCollider sphereCollider;

    private void Awake() {
        weaponTransform = GetComponentInParent<Transform>();
    }
    private void Update() {
        if(CheckZombieInWeaponZone().Length > 0) {
            Collider zombie = CheckZombieInWeaponZone()[0];
            weaponTransform.position = Vector3.MoveTowards(weaponTransform.position, 
                zombie.transform.position, chasingSpeed * Time.deltaTime);
        }
    }

    Collider[] CheckZombieInWeaponZone() {
        return Physics.OverlapSphere(this.transform.position, sphereCollider.radius, zombieMark);
    }
    
}
