using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class Map1GameController : MonoBehaviour
{
    private static Map1GameController instance;
    public static Map1GameController Instance { get => instance; }

    [Header("Zone")]
    public GameObject zone1;
    public GameObject zone2;

    [Header("-----GAME_PLAY-----")]
    public int startEnemyCount;
    public int maxEnemyCount;
    public List<EnemyController> listEnemyController = new List<EnemyController>();
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
        SkinShopController.Instance.SetUpSkinShop();
        PlayerController.Instance.SetUpPlayer();
        WeaponShopController.Instance.LoadWeapon();

        StartCoroutine(StartGame());
        StartCoroutine(WinGame());
        StartCoroutine(LoseGame());
        StartCoroutine(Revive());
    }

    private void Update() {
        if (!HomePageController.Instance.isStartGame)
            return;

        PlayerController.Instance.GetPlayerInput();

        for (int i = 0; i < listEnemyController.Count; i++) {
            if (listEnemyController[i] == null) {
                maxEnemyCount -= 1;
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

    private void LateUpdate() {
        for (int i = 0; i < listEnemyController.Count; i++) {
            if (listEnemyController[i] != null)
                listEnemyController[i].UpdateEnemyStatus();
        }
        PlayerController.Instance.UpdatePlayerStatus();
    }
    IEnumerator StartGame() {
        PlayerPersonalData data = DataManager.Instance.GetPlayerPersonalData();
        if (data.zone == 1) {
            zone1.gameObject.SetActive(true);
            zone2.gameObject.SetActive(false);
        }
        else {
            zone1.gameObject.SetActive(false);
            zone2.gameObject.SetActive(true);
        }

        for (int i = 0; i < startEnemyCount; i++) {
            EnemyController enemyContrl = SpawnEnemyController.Instance.SpawnEnemy();
            listEnemyController.Add(enemyContrl);
        }

        GameUIController.Instance.TurnOffFloatingText();
        MaxManager.Instance.ShowBannerAd();
        yield return new WaitUntil(() => HomePageController.Instance.isStartGame);

        GameUIController.Instance.TurnOnInGameUI();
        GameUIController.Instance.TurnOnFloatingText();
        GameUIController.Instance.DisplayEnemyCount(maxEnemyCount);

        CameraController.Instance.TurnOnGamePlayCamera();
        MaxManager.Instance.StopShowBannerAd();
        PlayerController.Instance.SetPlayerName();
    }

    /*Set Up khi Win Game*/
    IEnumerator WinGame() {
        yield return new WaitUntil(() => (isPlayerWin && !isPlayerLose && HomePageController.Instance.isStartGame));
        GameUIController.Instance.TurnOnWinPanel();
        PlayerController.Instance.SetPlayerWinDance();
        CameraController.Instance.TurnOnPlayerWinCamera();
        SoundManager.Instance.PlaySound(SoundManager.SoundName.end_win);
    }

    /*Set Up khi Lose Game*/
    IEnumerator LoseGame() {
        yield return new WaitUntil(() => (!isPlayerWin && isPlayerLose && HomePageController.Instance.isStartGame));
        GameUIController.Instance.TurnOnReviveUI();
        SoundManager.Instance.PlaySound(SoundManager.SoundName.end_lose);
    }

    IEnumerator Revive() {
        yield return new WaitUntil(() => GameUIController.Instance.isRevived);
        Vector3 position = SpawnEnemyController.Instance.GetValidPosition();
        PlayerController.Instance.PlayerRevive(position);

        GameUIController.Instance.TurnOnFloatingText();
        isPlayerLose = false;
        StartCoroutine(LoseGame());
    }

}
