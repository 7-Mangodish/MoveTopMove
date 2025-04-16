using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    private static CameraController instance;
    public static CameraController Instance { get { return instance; } }
    public CinemachineCamera gamePlayCamera;
    public CinemachineCamera homePageCamera;
    public CinemachineCamera shopCamera;
    public CinemachineCamera winingCamera;
    private SkillDataManager.SkillData skillData;

    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    public void SetUpCamera() {
        winingCamera.gameObject.SetActive(false);
        gamePlayCamera.gameObject.SetActive(false);
        shopCamera.gameObject.SetActive(false);
        HomePageController.Instance.OnShopping += Camera_OnShopping;
        HomePageController.Instance.OnOutShopping += Camera_OnOutShopping;
    }

    private void Camera_OnOutShopping(object sender, System.EventArgs e) {
        shopCamera.gameObject.SetActive(false);
    }

    private void Camera_OnShopping(object sender, System.EventArgs e) {
        shopCamera.gameObject.SetActive(true);
    }

    public async void TurnOnPlayerWinCamera() {
        await Task.Delay(500);
        if(winingCamera!= null)
            winingCamera.gameObject.SetActive(true);
    }

    private void Camera_OnPlayerUpgradeSkill(object sender, StartPanelManager.TypeSkill e) {
        if (e == StartPanelManager.TypeSkill.range)
            UpdateDistanceCamera(.75f);
    }

    private void SetUpCameraInZombieMode() {
        skillData = SkillDataManager.Instance.GetSkillData();
        float ind = skillData.range - skillData.BEGIN_RANGE;
        ind /= .5f;
        if (ind > 0)
            UpdateDistanceCamera(ind * .75f);
    }

    public void TurnOnGamePlayCamera() {
        gamePlayCamera.gameObject.SetActive(true);
    }
    public void UpdateDistanceCamera(float deltarange) {
        CinemachinePositionComposer cam = FindFirstObjectByType<CinemachinePositionComposer>();
        cam.CameraDistance += deltarange;
    }

}
