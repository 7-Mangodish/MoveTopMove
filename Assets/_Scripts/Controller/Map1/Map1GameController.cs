using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Map1GameController : MonoBehaviour
{
    private static Map1GameController instance;
    public static Map1GameController Instance { get => instance; }

    public int startEnemyCount;
    public int maxEnemyCount;
    public List<EnemyController> listEnemyController = new List<EnemyController>();
    private GameObject[] listEnemyDisplay;

    private bool isPlayerWin = false;
    private bool isPlayerLose = false;
    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else
            Destroy(this.gameObject);
    }

    private void Start() {
        CameraController.Instance.SetUpCamera();
        HomePageController.Instance.SetUpHomePage();
        WeaponShopController.Instance.LoadWeapon();
        SkinShopController.Instance.SetUpSkinShop();

        StartCoroutine(StartGame());
        StartCoroutine(WinGame());
        StartCoroutine(LoseGame());
    }

    private void Update() {
        if (!HomePageController.Instance.isStartGame)
            return;

        PlayerController.Instance.GetPlayerInput();

        for (int i = 0; i < listEnemyController.Count; i++) {
            if (listEnemyController[i] == null) {
                GameUIController.Instance.DisplayEnemyCount(maxEnemyCount);
                if (maxEnemyCount >= startEnemyCount)
                    listEnemyController[i] = SpawnEnemyController.Instance.SpawnEnemy();
                else
                    listEnemyController.RemoveAt(i);

            }
            else
                listEnemyController[i].EnemyBehaviour();
        }


        if (listEnemyController.Count == 0  && PlayerController.Instance.gameObject.activeSelf) {
            isPlayerWin = true;
            return;
        }
        if (!PlayerController.Instance.gameObject.activeSelf) {
            isPlayerLose = true;
            return;
        }
    }

    private void FixedUpdate() {
        PlayerController.Instance.PlayerBehaviour();
    }

    // SetUp Start Game
    IEnumerator StartGame() {
        for (int i = 0; i < startEnemyCount; i++) {
            EnemyController enemyContrl = SpawnEnemyController.Instance.SpawnEnemy();
            enemyContrl.SetUpEnemy();
            listEnemyController.Add(enemyContrl);
        }

        listEnemyDisplay = GameObject.FindGameObjectsWithTag("EnemyDisplay");
        foreach (GameObject obs in listEnemyDisplay) {
            Debug.Log(obs);
            obs.SetActive(false);
        }

        yield return new WaitUntil(() => HomePageController.Instance.isStartGame);

        foreach (GameObject obs in listEnemyDisplay) {
            obs.SetActive(true);
        }
        GameUIController.Instance.TurnOnInGameUI();
        CameraController.Instance.TurnOnGamePlayCamera();
    }

    // SetUp khi Win Game
    IEnumerator WinGame() {
        yield return new WaitUntil(() => (isPlayerWin && !isPlayerLose && HomePageController.Instance.isStartGame));
        GameUIController.Instance.TurnOnWinPanel();
        PlayerController.Instance.SetPlayerWinDance();
        CameraController.Instance.TurnOnPlayerWinCamera();
        SoundManager.Instance.PlaySound(SoundManager.SoundName.end_win);
    }
    //SetUp khi Lose Game
    IEnumerator LoseGame() {
        yield return new WaitUntil(() => (!isPlayerWin && isPlayerLose && HomePageController.Instance.isStartGame));
        GameUIController.Instance.TurnOnReviveUI();
        SoundManager.Instance.PlaySound(SoundManager.SoundName.end_lose);
        Debug.Log("PlayerLose");
    }

}
