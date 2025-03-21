using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraMove : MonoBehaviour
{
    private static CameraMove instance;
    public static CameraMove Instance { get { return instance; } }
    public CinemachineCamera camera1;
    public CinemachineCamera camera2;

    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    private void Start() {
        if(SceneManager.GetActiveScene().buildIndex == 0) {
            HomePageManager.Instance.OnStartGame += Camera_OnStartGame;
        }
    }

    private void Camera_OnStartGame(object sender, System.EventArgs e) {
        camera1.gameObject.SetActive(true);
    }
    public void UpdateCamera(float deltarange) {
        CinemachinePositionComposer cam = FindFirstObjectByType<CinemachinePositionComposer>();
        cam.CameraDistance += deltarange;
    }

}
