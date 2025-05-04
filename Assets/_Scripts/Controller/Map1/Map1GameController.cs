using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class Map1GameController : MonoBehaviour
{
    private static Map1GameController instance;
    public static Map1GameController Instance { get => instance; }


    private SoundController soundController;
    [Header("-----ZONE-----")]
    public GameObject zone1;
    public GameObject zone2;

    [Header("-----GAME_PLAY-----")]
    public int startEnemyCount;
    public int maxEnemyCount;
    public List<EnemyController> listEnemyController = new List<EnemyController>();
    private bool isPlayerWin = false;
    private bool isPlayerLose = false;

    [Header("-----ENEMY_STATUS-----")]
    public List<GameObject> listEnemyStatus = new List<GameObject>();
    public List<GameObject> listEnemyIndicator = new List<GameObject>();
    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else
            Destroy(this.gameObject);

        soundController = GetComponent<SoundController>();
    }

    private void Start() {
        CameraController.Instance.SetUpCamera();
        HomePageController.Instance.SetUpHomePage();
        SkinShopController.Instance.SetUpSkinShop();
        PlayerController.Instance.SetUpPlayer();
        WeaponShopController.Instance.InitWeaponShopData();

        StartCoroutine(StartGame());
        StartCoroutine(WinGame());
        StartCoroutine(LoseGame());
        StartCoroutine(Revive());
    }

    private void Update() {           
        if (!HomePageController.Instance.isStartGame) return;
        //
        PlayerController.Instance.GetPlayerInput();
        //
        for (int i = 0; i < listEnemyController.Count; i++) {
            if (listEnemyController[i] == null) {
                maxEnemyCount -= 1;
                GameUIController.Instance.DisplayEnemyCount(maxEnemyCount + 1);
                //
                if (maxEnemyCount >= startEnemyCount) {
                    EnemyController enemyCtrl = SpawnController.Instance.SpawnEnemy();
                    listEnemyController[i] = enemyCtrl;
                    listEnemyIndicator.Add(enemyCtrl.indicatorContainer);
                    listEnemyStatus.Add(enemyCtrl.characterStatus);
                }
                else
                    listEnemyController.RemoveAt(i);
            }
            else
                listEnemyController[i].EnemyBehaviour();
        }
        //
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
            if (listEnemyController[i] != null) {
                listEnemyController[i].UpdateEnemyStatus();
                listEnemyController[i].UpdateIndicator();
                //if (listEnemyController[i].isOutScreen) {
                //    TurnOnEnemyStatus();
                //    listEnemyController[i].UpdateIndicator();
                //}
                //else {
                //    TurnOffEnemyStatus();
                //}
            }
        }
        PlayerController.Instance.UpdatePlayerStatus();
    }
    IEnumerator StartGame() {
        if (DataManager.Instance.playerData.zone == 1) {
            zone1.gameObject.SetActive(true);
            zone2.gameObject.SetActive(false);
        }
        else {
            zone1.gameObject.SetActive(false);
            zone2.gameObject.SetActive(true);
        }
        //Spawn Enemy
        for (int i = 0; i < startEnemyCount; i++) {
            EnemyController enemyContrl = SpawnController.Instance.SpawnEnemy();
            listEnemyController.Add(enemyContrl);
            listEnemyIndicator.Add(enemyContrl.indicatorContainer);
            listEnemyStatus.Add(enemyContrl.characterStatus);
        }
        // Tat Status
        TurnOffEnemyStatus();
        PlayerController.Instance.playerStatus.gameObject.SetActive(false);
        //Show Banner
        MaxManager.Instance.ShowBannerAd();
        //
        yield return new WaitUntil(() => HomePageController.Instance.isStartGame);
        //
        GameUIController.Instance.TurnOnInGameUI();
        // Bat Status
        TurnOnEnemyStatus();
        PlayerController.Instance.playerStatus.gameObject.SetActive(true);
        //
        GameUIController.Instance.DisplayEnemyCount(maxEnemyCount + 1);
        CameraController.Instance.TurnOnGamePlayCamera();
        MaxManager.Instance.StopShowBannerAd();
        PlayerController.Instance.SetPlayerName();
        if (DataManager.Instance.playerData.zone == 2)  SpawnController.Instance.DoSpawnGift();
    }

    /*Set Up khi Win Game*/
    IEnumerator WinGame() {
        yield return new WaitUntil(() => (isPlayerWin && !isPlayerLose && HomePageController.Instance.isStartGame));
        TurnOffEnemyStatus() ;
        //
        GameUIController.Instance.TurnOnWinPanel();
        PlayerController.Instance.SetPlayerWinDance();
        CameraController.Instance.TurnOnPlayerWinCamera();
        //
        DataManager.Instance.playerData.zone += 1;
        //
        soundController.PlaySound(SoundData.SoundName.end_win);
    }
    /*Set Up khi Lose Game*/
    IEnumerator LoseGame() {
        yield return new WaitUntil(() => (!isPlayerWin && isPlayerLose && HomePageController.Instance.isStartGame));
        GameUIController.Instance.TurnOnReviveUI();
        GameUIController.Instance.SetUpPlayerRank(maxEnemyCount + 1);
        soundController.PlaySound(SoundData.SoundName.end_lose);
        TurnOffEnemyStatus();
    }
    /*Set Up khi Revive*/
    IEnumerator Revive() {
        yield return new WaitUntil(() => GameUIController.Instance.isRevived);
        Vector3 position = SpawnController.Instance.GetValidPosition();
        PlayerController.Instance.PlayerRevive(position);

        TurnOnEnemyStatus();
        isPlayerLose = false;
        StartCoroutine(LoseGame());
    }
    public void TurnOnEnemyStatus() {
        for (int i= listEnemyStatus.Count -1; i >= 0; i--) {
            if (listEnemyStatus[i] != null) listEnemyStatus[i].SetActive(true);
            else listEnemyStatus.Remove(listEnemyStatus[i]);
        }
        for (int i = listEnemyIndicator.Count - 1; i >= 0; i--) {
            if (listEnemyIndicator[i] != null) listEnemyIndicator[i].SetActive(true);
            else listEnemyIndicator.Remove(listEnemyIndicator[i]);
        }
    }
    public void TurnOffEnemyStatus() {
        for (int i = listEnemyStatus.Count - 1; i >= 0; i--) {
            if (listEnemyStatus[i] != null) listEnemyStatus[i].SetActive(false);
            else listEnemyStatus.Remove(listEnemyStatus[i]);
        }
        for (int i = listEnemyIndicator.Count - 1; i >= 0; i--) {
            if (listEnemyIndicator[i] != null) listEnemyIndicator[i].SetActive(false);
            else listEnemyIndicator.Remove(listEnemyIndicator[i]);
        }
    }
}
