using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraMove : MonoBehaviour
{
    private static CameraMove instance;
    public static CameraMove Instance { get { return instance; } }
    [SerializeField] private SkillObjects skillObjects;
    public CinemachineCamera gamePlayCamera;
    public CinemachineCamera homePageCamera;
    public CinemachineCamera shopCamera;
    public CinemachineCamera winingCamera;

    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    private void Start() {
        GameManager.Instance.OnPlayerWin += Camera_OnPlayerWin;
        if(SceneManager.GetActiveScene().buildIndex == 0) {
            HomePageManager.Instance.OnStartGame += Camera_OnStartGame;
            HomePageManager.Instance.OnShopping += Camera_OnShopping;
            HomePageManager.Instance.OnOutShopping += Camera_OnOutShopping;
        }
        else {
            SetUpCameraInZombieMode();
            StartPanelManager.Instance.OnPlayerUpgradeSkill += Camera_OnPlayerUpgradeSkill;
        }
    }

    private void Camera_OnOutShopping(object sender, System.EventArgs e) {
        shopCamera.gameObject.SetActive(false);
    }

    private void Camera_OnShopping(object sender, System.EventArgs e) {
        shopCamera.gameObject.SetActive(true);
    }

    private async void Camera_OnPlayerWin(object sender, System.EventArgs e) {
        await Task.Delay(500);
        winingCamera.gameObject.SetActive(true);
    }

    private void Camera_OnPlayerUpgradeSkill(object sender, SkillObjects.TypeSkill e) {
        if (e == SkillObjects.TypeSkill.range)
            UpdateDistanceCamera(.75f);
    }

    private void SetUpCameraInZombieMode() {
        float ind = skillObjects.range - skillObjects.BEGIN_RANGE;
        ind /= .5f;
        if (ind > 0)
            UpdateDistanceCamera(ind * .75f);
    }

    private void Camera_OnStartGame(object sender, System.EventArgs e) {
        gamePlayCamera.gameObject.SetActive(true);
    }
    public void UpdateDistanceCamera(float deltarange) {
        CinemachinePositionComposer cam = FindFirstObjectByType<CinemachinePositionComposer>();
        cam.CameraDistance += deltarange;
    }

}
