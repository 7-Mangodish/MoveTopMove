using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    public event EventHandler<int> OnEnemyQuantityDown;

    [SerializeField] private int currentEnemyQuantity;
    [SerializeField] private GameObject enemyPrefab;
    private void Awake() {
        if(instance == null) {
            instance = this;
        }
        else {
            Destroy(this.gameObject);
        }
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void DecreaseEnemyQuantity() {
        this.currentEnemyQuantity -= 1;
        OnEnemyQuantityDown?.Invoke(this, this.currentEnemyQuantity);
    }

    public void SpawnEnemy() {
        if (currentEnemyQuantity > 0)
            Instantiate(enemyPrefab, this.transform.position, Quaternion.identity);
        else
            Debug.Log("Win");
    }
}
