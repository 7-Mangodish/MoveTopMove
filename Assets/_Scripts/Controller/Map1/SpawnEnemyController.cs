using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class SpawnController : MonoBehaviour
{
    private static SpawnController instance;
    public static SpawnController Instance { get => instance; }

    [Header("-----ENEMY-----")]
    public GameObject enemyPrefab;
    public float validRadius;
    public LayerMask checkLayerMark;

    [Header("-----GIFT-----")]
    public GameObject giftPrefab;
    public float timeSpawnGift;
    private List<GameObject> giftList;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

    }

    #region ----------ENEMY----------
    public EnemyController SpawnEnemy() {
        Vector3 position = GetValidPosition();
        GameObject enemyPref = Instantiate(enemyPrefab, position, Quaternion.identity);
        EnemyController enemyContrl = enemyPref.GetComponentInChildren<EnemyController>();
        enemyContrl.SetUpEnemy();
        return enemyContrl;
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
    #endregion

    #region ----------GIFT----------

    public void DoSpawnGift() {
        Invoke(nameof(SpawnGift), timeSpawnGift);
    }
    void SpawnGift() {
        Vector3 newPosition = GetValidPosition();
        newPosition.y = .2f;
        Instantiate(giftPrefab, newPosition, Quaternion.Euler(-90, 0, 0));
    }
    #endregion

}
