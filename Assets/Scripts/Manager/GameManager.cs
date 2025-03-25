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

    [SerializeField] private int enemyQuantityWhenStart;
    [SerializeField] private int maxEnemyQuantity;
    [SerializeField] private GameObject enemyPrefab;

    [Header("Zombie Mode")]
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private float timeSpawnZombie;
    private float timeDuration;


    private int currentEnemyQuantity;
    private int activeEnemyQuantity;

    private bool isStartZombieMode = false;
    private void Awake() {
        if(instance == null) {
            instance = this;
        }
        else {
            Destroy(this.gameObject);
        }
        timeSpawnZombie = 1f;
    }

    private void Start() {
        if (SceneManager.GetActiveScene().buildIndex == 0) {
            HomePageManager.Instance.OnStartGame += GameManager_OnStartGame;
        }
        else {
            StartPanelManager.Instance.OnStartZombieMode += GameManager_OnStartZombieMode;
        }
    }

    private void Update() {
        if (SceneManager.GetActiveScene().buildIndex == 1 && isStartZombieMode) {
            if (currentEnemyQuantity < maxEnemyQuantity) {
                timeDuration += Time.deltaTime;
                if (timeDuration > timeSpawnZombie) {
                    SpawnEnemy();
                    currentEnemyQuantity++;
                }
            }
        }
    }
    private void GameManager_OnStartZombieMode(object sender, EventArgs e) {
        isStartZombieMode = true;
        SpawnEnemyWhenStartGame();
    }

    private void GameManager_OnStartGame(object sender, EventArgs e) {
        SpawnEnemyWhenStartGame();
    }


    private void SpawnEnemyWhenStartGame() {
        activeEnemyQuantity = maxEnemyQuantity;
        currentEnemyQuantity = enemyQuantityWhenStart;
        for (int i = 0; i < enemyQuantityWhenStart; i++) {

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
        Vector3 newPosition = GetRandomPosition();
        if (NavMesh.SamplePosition(newPosition, out NavMeshHit hit, .5f, NavMesh.AllAreas)) {
            if (SceneManager.GetActiveScene().buildIndex == 0)
                Instantiate(enemyPrefab, newPosition, Quaternion.identity);
            else
                Instantiate(zombiePrefab, newPosition, Quaternion.identity);
        }

    }

    public void DoZombieDead() {
        this.activeEnemyQuantity -= 1;
        OnEnemyQuantityDown?.Invoke(this, this.activeEnemyQuantity);
        if (this.activeEnemyQuantity == 0) {
            OnPlayerWin?.Invoke(this, EventArgs.Empty);
            Debug.Log("Player Win");
            return;
        }
    }

    public void DoEnemyDead() {
        this.activeEnemyQuantity -= 1;
        OnEnemyQuantityDown?.Invoke(this, this.activeEnemyQuantity);
        if(this.activeEnemyQuantity == 0) {
            Debug.Log("Player Win");
            OnPlayerWin?.Invoke(this, EventArgs.Empty);
            return;
        }
        if (activeEnemyQuantity-enemyQuantityWhenStart >= 0) {
            SpawnEnemy();
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
