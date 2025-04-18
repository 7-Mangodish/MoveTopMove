using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneController : MonoBehaviour
{
    public void LoadZombieMode() {
        SceneManager.LoadScene(GameVariable.zombieSceneName);
    }

    public void LoadNormalGameMode() {
        SceneManager.LoadScene(GameVariable.normalSceneName);
    }
}
