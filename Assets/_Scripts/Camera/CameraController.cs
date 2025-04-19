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
    private SkillData skillData;

    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }



    #region ----------Normal_Mode----------
    public void SetUpCamera() {
        winingCamera.gameObject.SetActive(false);
        gamePlayCamera.gameObject.SetActive(false);
        shopCamera.gameObject.SetActive(false);
    }

    public void TurnOffPlayerShoppingCamera() {
        shopCamera.gameObject.SetActive(false);
    }

    public void TurnOnPlayerShoppingCamera() {
        shopCamera.gameObject.SetActive(true);
    }

    public async void TurnOnPlayerWinCamera() {
        await Task.Delay(500);
        if(winingCamera!= null)
            winingCamera.gameObject.SetActive(true);
    }

    public void TurnOnGamePlayCamera() {
        gamePlayCamera.gameObject.SetActive(true);
    }
    #endregion

    #region ----------Zombie_Mode-----------
    public void SetUpgradeSkillRangeCamera() {
            UpdateDistanceCamera(.75f);
    }

    public void SetUpCameraInZombieMode() {
        skillData = DataManager.Instance.GetSkillData();
        float ind = skillData.range - skillData.BEGIN_RANGE;
        ind /= .5f;
        if (ind > 0)
            UpdateDistanceCamera(ind * .75f);

        winingCamera.gameObject.SetActive(false);
    }

    public void TurnOnZombieWinCamera() {
        winingCamera.gameObject.SetActive(true);
    }
    #endregion
    public void UpdateDistanceCamera(float deltarange) {
        CinemachinePositionComposer cam = FindFirstObjectByType<CinemachinePositionComposer>();
        cam.CameraDistance += deltarange;
    }

}
