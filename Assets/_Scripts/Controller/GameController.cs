using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour {
    private static GameController instance;
    public static GameController Instance { get { return instance; } }

    public event EventHandler<int> OnEnemyQuantityDown;
    public event EventHandler OnPlayerLose;
    public event EventHandler OnPlayerWin;
    public event EventHandler<Vector3> OnPlayerRevive;

    [SerializeField] private int enemyQuantityWhenStart;
    [SerializeField] private int maxEnemyQuantity;
    [SerializeField] private GameObject enemyPrefab;

    [Header("Player Revive")]
    [SerializeField] private float checkReviveRadius;
    [SerializeField] private LayerMask enemyLayerMask;

    [Header("Zombie Mode")]
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private GameObject zombieBossPrefab;
    [SerializeField] private float timeSpawnZombie;
    private float timeDuration;

    private int activeEnemyQuantity;
    private int currentEnemyQuantity;

    private bool isStartZombieMode = false;
    private bool isPlayerLose;
    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(this.gameObject);
        }
    }

    private void Start() {
        if (SceneManager.GetActiveScene().name == GameVariable.normalSceneName) {
        }
        else {
            //StartPanelManager.Instance.OnStartZombieMode += GameController_OnStartZombieMode;
        }
    }

    private void Update() {
        if (SceneManager.GetActiveScene().name == GameVariable.zombieSceneName && isStartZombieMode && !isPlayerLose) {
            if (currentEnemyQuantity < maxEnemyQuantity) {
                timeDuration += Time.deltaTime;
                if (timeDuration > timeSpawnZombie) {
                    SpawnEnemy();
                }
            }
        }
    }


    private void GameManager_OnStartGame(object sender, EventArgs e) {
        Debug.Log("Start Game");
        SpawnEnemyWhenStartGame();
    }
    private void GameController_OnStartZombieMode(object sender, EventArgs e) {
        SpawnZombieWhenStartGame();
        isStartZombieMode = true;
        int day = PlayerPrefs.GetInt("ZombieDayVictory");
        if (day % 5 == 4) {
            activeEnemyQuantity += 1;
            Vector3 bossPosition = GetValidPosition();
            Instantiate(zombieBossPrefab, bossPosition, Quaternion.identity);
        }

    }
    private void SpawnZombieWhenStartGame() {
        activeEnemyQuantity = maxEnemyQuantity;
        currentEnemyQuantity = enemyQuantityWhenStart;
        for (int i = 0; i < enemyQuantityWhenStart; i++) {
            Vector3 newPosition = GetValidPosition();
            Instantiate(zombiePrefab, newPosition, Quaternion.identity);
        }
    }

    private void SpawnEnemyWhenStartGame() {
        activeEnemyQuantity = maxEnemyQuantity;
        for (int i = 0; i < enemyQuantityWhenStart; i++) {
            Vector3 newPosition = GetValidPosition();
            //Debug.Log(newPosition);
            Instantiate(enemyPrefab, newPosition, Quaternion.identity);
        }
    }
    public void SpawnEnemy() {
        currentEnemyQuantity++;
        Vector3 newPosition = GetValidPosition();

        if (SceneManager.GetActiveScene().name == GameVariable.normalSceneName)
            Instantiate(enemyPrefab, newPosition, Quaternion.identity);
        else
            Instantiate(zombiePrefab, newPosition, Quaternion.identity);

    }

    public void DoPlayerRevive() {
        Vector3 newPosition = GetValidPosition();
        OnPlayerRevive?.Invoke(this, newPosition);
        isPlayerLose = false;
    }
    public void DoZombieDead() {
        this.activeEnemyQuantity -= 1;
        OnEnemyQuantityDown?.Invoke(this, this.activeEnemyQuantity);
        if (this.activeEnemyQuantity == 0) {
            OnPlayerWin?.Invoke(this, EventArgs.Empty);
            SoundManager.Instance.PlaySound(SoundManager.SoundName.end_win);

            Debug.Log("Player Win");
            return;
        }
    }
    public void DoEnemyDead() {
        this.activeEnemyQuantity -= 1;
        OnEnemyQuantityDown?.Invoke(this, this.activeEnemyQuantity);
        if (this.activeEnemyQuantity == 0) {
            OnPlayerWin?.Invoke(this, EventArgs.Empty);
            SoundManager.Instance.PlaySound(SoundManager.SoundName.end_win);
            return;
        }
        if (activeEnemyQuantity - enemyQuantityWhenStart >= 0) {
            SpawnEnemy();
        }
    }
    private Vector3 GetRandomPosition() {
        Vector3 newPosition = new Vector3(0, 0, 0);
        for (int i = 0; i < 30; i++) {
            float randX = Random.Range(-15, 15);
            float randZ = Random.Range(-15, 15);
            int randM = Random.Range(-1, 1) < 0 ? -1 : 1;
            newPosition = new Vector3(randX * randM, 0, randZ * randM);
            if (NavMesh.SamplePosition(newPosition, out NavMeshHit hit, .5f, NavMesh.AllAreas)) {
                //Debug.Log(hit.position);
                newPosition = hit.position;
                break;
            }
        }

        return newPosition;

    }

    private Vector3 GetValidPosition() {
        Vector3 newPosition = GetRandomPosition();
        Collider[] enemyCollider = Physics.OverlapSphere(newPosition,
            checkReviveRadius, enemyLayerMask);
        while (enemyCollider.Length != 0) {
            newPosition = GetRandomPosition();
            enemyCollider = Physics.OverlapSphere(newPosition,
            checkReviveRadius, enemyLayerMask);
        }
        return newPosition;
    }

    public void PlayerLose() {
        Debug.Log("Player Lose");
        OnPlayerLose?.Invoke(this, EventArgs.Empty);
        SoundManager.Instance.PlaySound(SoundManager.SoundName.end_lose);

        isPlayerLose = true;
    }
    public int GetMaxEnemyQuantity() {
        return maxEnemyQuantity;
    }
}
