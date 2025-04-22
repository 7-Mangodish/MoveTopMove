using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ZombieGameController : MonoBehaviour
{
    [Header("-----SpawnZombieVar-----")]
    public float timeSpawnZombie;
    private float timeDuration;
    public int spawnedZombieCount;
    public int currentZombieCount;
    public int maxZombieCount;
    public List<ZombieController> listZombieController = new List<ZombieController>();

    private bool isPlayerLose = false;
    private bool isPlayerWin = false;
    void Start()
    {
        currentZombieCount = maxZombieCount;
        ZombieUIController.Instance.SetUpUIWhenStart();
        CameraController.Instance.SetUpCameraInZombieMode();
        PlayerController.Instance.GetComponent<LoadSkinWhenStart>().LoadSkin();
        PlayerController.Instance.SetUpPlayer();

        StartCoroutine(StartGame());
        StartCoroutine(WinGame());
        StartCoroutine(LoseGame());
        StartCoroutine(Revive());
    }

    void Update()
    {
        if (!ZombieUIController.Instance.isStartGame)
            return;

        PlayerController.Instance.GetPlayerInput();

        DoSpawnZombie();

        int zombieDieCount = listZombieController.RemoveAll(x => x == null);
        currentZombieCount -= zombieDieCount;
        ZombieUIController.Instance.DisplayZombieCount(currentZombieCount);

        for (int i=0; i<listZombieController.Count; ++i) {
            listZombieController[i].ZombieBehaviour();
        }

        if (currentZombieCount == 0 && PlayerController.Instance.gameObject.activeSelf) {
            isPlayerWin = true;
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
        PlayerController.Instance.UpdatePlayerStatus();
    }
    IEnumerator StartGame() {
        MaxManager.Instance.StopShowBannerAd();
        yield return new WaitUntil(() => ZombieUIController.Instance.isStartGame);
        SkillData data = DataManager.Instance.GetSkillData();
        PlayerController.Instance.ReferenceToSkillAndAbility(data); // Lay cac chi so skill

        StartCoroutine(ZombieUIController.Instance.ShowInstruction());
    }

    IEnumerator WinGame() {
        yield return new WaitUntil(() => (!isPlayerLose && isPlayerWin && ZombieUIController.Instance.isStartGame));
        ZombieUIController.Instance.TurnOnWinUI();
        PlayerController.Instance.SetPlayerWinDance();
        SoundManager.Instance.PlaySound(SoundManager.SoundName.end_win);
        CameraController.Instance.TurnOnZombieWinCamera();
    }
    IEnumerator LoseGame() {
        yield return new WaitUntil(() => (isPlayerLose && !isPlayerWin && ZombieUIController.Instance.isStartGame));
        ZombieUIController.Instance.TurnOnLoseUI();
        SoundManager.Instance.PlaySound(SoundManager.SoundName.end_lose);

    }

    IEnumerator Revive() {
        yield return new WaitUntil(() => ZombieUIController.Instance.isRevived);
        Vector3 newPosition = SpawnZombieController.Instance.GetValidPosition();
        PlayerController.Instance.PlayerRevive(newPosition);
        isPlayerLose = false;
        StartCoroutine(LoseGame());
    }
    private void DoSpawnZombie() {
        if (timeDuration >= timeSpawnZombie)
            timeDuration = timeSpawnZombie;
        else
            timeDuration += Time.deltaTime;

        if (timeDuration >= timeSpawnZombie) {
            timeDuration = 0f;
            if (spawnedZombieCount < maxZombieCount) {
                ZombieController zombieContrl = SpawnZombieController.Instance.SpawnZombie();
                listZombieController.Add(zombieContrl);
                spawnedZombieCount += 1;
            }
        }
    }
}
