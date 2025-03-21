using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    public event EventHandler<int> OnEnemyQuantityDown;
    public event EventHandler OnPlayerLose;
    public event EventHandler OnPlayerWin;

    [SerializeField] private int enemyQuantityOnMatch;
    [SerializeField] private int maxEnemyQuantity;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject zombiePrefab;
    private HomePageManager homeManager;

    private void Awake() {
        if(instance == null) {
            instance = this;
        }
        else {
            Destroy(this.gameObject);
        }
    }

    private void Start() {
        if (SceneManager.GetActiveScene().buildIndex == 0) {
            HomePageManager.Instance.OnStartGame += GameManager_OnStartGame;
        }
        else {
            StartPanelManager.Instance.OnStartZombieMode += GameManager_OnStartZombieMode;
        }
    }

    private void GameManager_OnStartZombieMode(object sender, EventArgs e) {
        SpawnEnemyWhenStartGame();
        Invoke(nameof(SpawnEnemyWhenStartGame), .5f);
    }

    private void GameManager_OnStartGame(object sender, EventArgs e) {
        SpawnEnemyWhenStartGame();
    }


    private void SpawnEnemyWhenStartGame() {
        for (int i = 0; i < enemyQuantityOnMatch; i++) {

            Vector3 newPosition = GetRandomPosition();

            if (NavMesh.SamplePosition(newPosition, out NavMeshHit hit, .5f, NavMesh.AllAreas)) {
                if(SceneManager.GetActiveScene().buildIndex == 0)
                    Instantiate(enemyPrefab, newPosition, Quaternion.identity);
                else
                    Instantiate(zombiePrefab, newPosition, Quaternion.identity);

            }
        }
    }
    public void SpawnEnemy() {
        this.maxEnemyQuantity -= 1;
        OnEnemyQuantityDown?.Invoke(this, this.maxEnemyQuantity);
        if (maxEnemyQuantity - enemyQuantityOnMatch > 0) {

            Vector3 newPosition = GetRandomPosition();

            if (NavMesh.SamplePosition(newPosition, out NavMeshHit hit, .5f, NavMesh.AllAreas)) {
                Instantiate(enemyPrefab, newPosition, Quaternion.identity);
            }

        }
        else {
            Debug.Log("Player Win");
            OnPlayerWin?.Invoke(this, EventArgs.Empty);
        }
    }

    private Vector3 GetRandomPosition() {
        float randX = Random.Range(-3, 3);
        float randZ = Random.Range(-3, 3);
        int randM = Random.Range(-1, 1) < 0 ? -1 : 1;

        Vector3 newPosition = new Vector3(randX * randM, 0, randZ * randM);
        return newPosition;
    }

    public void PlayerLose() {
        Debug.Log("Player Lose");
        OnPlayerLose?.Invoke(this, EventArgs.Empty);
    }

    public int GetMaxEnemyQuantity() {
        return maxEnemyQuantity;
    }


}
