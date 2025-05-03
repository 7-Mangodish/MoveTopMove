using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private void Start() {
        StartCoroutine(Init());
    }
    IEnumerator Init() {
        DataManager.Instance.LoadData();
        yield return new WaitUntil(() => DataManager.Instance.isInit == true);

        //MaxManager.Instance.InitMaxManager();
        //yield return new WaitUntil(() => MaxManager.Instance.isInit == true);

        //FirebaseManager.Instance.InitFirebase();
        //yield return new WaitUntil(() => FirebaseManager.Instance.isInit == true);

        //AppsFlyermanager.Instance.InitAppsFlyer();

        //SceneManager.LoadScene(GameVariable.normalSceneName);

    }
}
