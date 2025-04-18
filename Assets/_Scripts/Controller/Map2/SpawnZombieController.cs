using UnityEngine;
using UnityEngine.AI;

public class SpawnZombieController : MonoBehaviour
{
    private static SpawnZombieController instance;
    public static SpawnZombieController Instance { get => instance; }

    public GameObject zombiePrefab;
    public float validRadius;
    public LayerMask checkLayerMark;

    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    public ZombieController SpawnZombie() {
        Vector3 position = GetValidPosition();
        GameObject zombiePref = Instantiate(zombiePrefab, position, Quaternion.identity);
        ZombieController zombieContrl = zombiePref.GetComponent<ZombieController>();
        zombieContrl.SetUpZombie();
        return zombieContrl;
    }

    public Vector3 GetRandomPosition() {
        Vector3 newPosition = new Vector3(0, 0, 0);
        for (int i = 0; i < 30; i++) {
            float randX = Random.Range(-15, 15);
            float randZ = Random.Range(-15, 15);
            int randM = Random.Range(-1, 1) < 0 ? -1 : 1;
            newPosition = new Vector3(randX * randM, 0, randZ * randM);
            if (NavMesh.SamplePosition(newPosition, out NavMeshHit hit, .5f, NavMesh.AllAreas)) {
                newPosition = hit.position;
                break;
            }
        }
        return newPosition;
    }

    public Vector3 GetValidPosition() {
        Vector3 newPosition = GetRandomPosition();
        Collider[] enemyCollider = Physics.OverlapSphere(newPosition,
            validRadius, checkLayerMark);
        while (enemyCollider.Length != 0) {
            newPosition = GetRandomPosition();
            enemyCollider = Physics.OverlapSphere(newPosition,
            validRadius, checkLayerMark);
        }
        return newPosition;
    }
}
