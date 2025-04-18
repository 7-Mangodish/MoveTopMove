using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieGameController : MonoBehaviour
{
    public float timeSpawnZombie;
    private float timeDuration;
    public int currentZombieCount;
    public int maxZombieCount;
    public List<ZombieController> listZombieController = new List<ZombieController>();

    void Start()
    {
        ZombieUIController.Instance.SetUpUIWhenStart();
        PlayerController.Instance.SetUpPlayer();
        CameraController.Instance.SetUpCameraInZombieMode();
        StartCoroutine(StartGame());
    }

    void Update()
    {
        if (!ZombieUIController.Instance.isStartGame)
            return;

        PlayerController.Instance.GetPlayerInput();

        DoSpawnZombie();

        int zombieDieCount = listZombieController.RemoveAll(x => x == null);
        maxZombieCount -= zombieDieCount;
        ZombieUIController.Instance.DisplayZombieCount(maxZombieCount);

        for (int i=0; i<listZombieController.Count; ++i) {
            listZombieController[i].ZombieBehaviour();
        }

    }

    private void FixedUpdate() {
        PlayerController.Instance.PlayerBehaviour();
    }
    IEnumerator StartGame() {
        MaxManager.Instance.StopShowBannerAd();
        yield return new WaitUntil(() => ZombieUIController.Instance.isStartGame);
        PlayerController.Instance.ReferenceToSkill(); // Lay cac chi so skill
    }

    private void DoSpawnZombie() {
        if (timeDuration >= timeSpawnZombie)
            timeDuration = timeSpawnZombie;
        else
            timeDuration += Time.deltaTime;

        if (timeDuration >= timeSpawnZombie) {
            timeDuration = 0f;
            if (currentZombieCount < maxZombieCount) {
                ZombieController zombieContrl = SpawnZombieController.Instance.SpawnZombie();
                listZombieController.Add(zombieContrl);
                currentZombieCount += 1;
            }
        }
    }
}
