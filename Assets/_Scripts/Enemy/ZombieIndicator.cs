using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ZombieIndicator : MonoBehaviour
{
    [Header("Position")]
    private GameObject targetCanvas;
    [SerializeField] private GameObject indicatorContainer;

    [Header("Color")]
    [SerializeField] private Image indicatorIcon;
    [SerializeField] private SkinnedMeshRenderer skinCharacter;
    private Camera mainCamera;
    private ZombieController zombieController;

    bool isDead = false;
    private void Awake() {
        //indicatorSprite = indicatorImage.sprite;
        targetCanvas = GameObject.FindGameObjectWithTag("Canvas");

        mainCamera = Camera.main;
        zombieController = GetComponent<ZombieController>();
        indicatorIcon.color = skinCharacter.material.color;
    }

    void Start() {
        indicatorContainer.transform.SetParent(targetCanvas.transform);
        indicatorContainer.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        UpdateIndicator();
    }

    void UpdateIndicator() {
        Vector3 enemyPositionOnScreen = mainCamera.WorldToScreenPoint(this.transform.position);
        if (isDead)
            return;
        if (enemyPositionOnScreen.z < 0)
            return;
        if (enemyPositionOnScreen.x < 0 || enemyPositionOnScreen.x > Screen.width
            || enemyPositionOnScreen.y < 0 || enemyPositionOnScreen.y > Screen.height) {

            //Debug.Log(sizeDelta);
            enemyPositionOnScreen.x = Mathf.Clamp(enemyPositionOnScreen.x, 0, Screen.width - 150);
            enemyPositionOnScreen.y = Mathf.Clamp(enemyPositionOnScreen.y, 0, Screen.height - 150);
            enemyPositionOnScreen.z = 0;

            Vector3 direct = enemyPositionOnScreen - new Vector3(Screen.width / 2, Screen.height / 2, 0);
            float angle = Mathf.Atan2(direct.y, direct.x) * Mathf.Rad2Deg;
            angle = (angle < 0) ? (angle + 360) : angle;

            indicatorContainer.gameObject.SetActive(true);
            indicatorContainer.GetComponent<RectTransform>().position = enemyPositionOnScreen;

            indicatorIcon.rectTransform.rotation = Quaternion.Euler(0, 0, angle - 90);


        }
        else {
            indicatorContainer.gameObject.SetActive(false);
        }
    }
}
