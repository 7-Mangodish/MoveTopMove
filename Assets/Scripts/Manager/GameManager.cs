using System;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    public event EventHandler<int> OnEnemyQuantityDown;
    public event EventHandler OnPlayerLose;

    [SerializeField] private int enemyQuantityOnMatch;
    [SerializeField] private int maxEnemyQuantity;
    [SerializeField] private GameObject enemyPrefab;
    private void Awake() {
        if(instance == null) {
            instance = this;
        }
        else {
            Destroy(this.gameObject);
        }
    }

    private void Start() {
        for(int i=0; i< enemyQuantityOnMatch; i++) {
            float randX = Random.Range(-3, 3);
            float randZ = Random.Range(-3, 3);
            int randM = Random.Range(-1, 1) < 0 ? -1 : 1;

            Vector3 newPosition = new Vector3(randX * randM, 0, randZ * randM);

            if (NavMesh.SamplePosition(newPosition, out NavMeshHit hit, .5f, NavMesh.AllAreas)) {
                Instantiate(enemyPrefab, newPosition, Quaternion.identity);
            }
        }
    }
    public void SpawnEnemy() {
        this.maxEnemyQuantity -= 1;
        OnEnemyQuantityDown?.Invoke(this, this.maxEnemyQuantity);
        if (maxEnemyQuantity - enemyQuantityOnMatch > 0) {
            float randX = Random.Range(-3, 3);
            float randZ = Random.Range(-3, 3);
            int randM = Random.Range(-1, 1) < 0 ? -1 : 1;

            Vector3 newPosition = new Vector3(randX * randM, 0, randZ * randM);

            if (NavMesh.SamplePosition(newPosition, out NavMeshHit hit, .5f, NavMesh.AllAreas)) {
                Instantiate(enemyPrefab, newPosition, Quaternion.identity);
            }

        }
        else
            Debug.Log("Player Win");
    }

    public void PlayerLose() {
        Debug.Log("Player Lose");
        OnPlayerLose?.Invoke(this, EventArgs.Empty);
    }

    public int GetMaxEnemyQuantity() {
        return maxEnemyQuantity;
    }
}
