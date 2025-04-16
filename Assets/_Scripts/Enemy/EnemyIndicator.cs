using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyIndicator : MonoBehaviour
{
    [Header("Position")]
    private GameObject targetCanvas;
    [SerializeField] private GameObject indicatorContainer;

    [Header("Color")]
    [SerializeField] private Image indicatorIcon;
    [SerializeField] private Image indicatorScoreContainer;
    [SerializeField] private TextMeshProUGUI indicatorScoreText;
    [SerializeField] private SkinnedMeshRenderer skinCharacter;
    private Camera mainCamera;
    private StateManager stateManager;

    bool isDead = false;
    private void Awake() {
        //indicatorSprite = indicatorImage.sprite;
        targetCanvas = GameObject.FindGameObjectWithTag("Canvas");

        mainCamera = Camera.main;
        stateManager = GetComponent<StateManager>();
    }



    void Start()
    {

        indicatorIcon.color = skinCharacter.material.color;
        indicatorScoreContainer.color = skinCharacter.material.color;


        indicatorContainer.transform.SetParent(targetCanvas.transform);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateIndicator(); 
    }

    void UpdateIndicator() {
        Vector3 enemyPositionOnScreen = mainCamera.WorldToScreenPoint(this.transform.position);
        if (isDead)
            return;
        if (enemyPositionOnScreen.z < 0)
            return;
        //Debug.Log(enemyPositionOnScreen);
        if (enemyPositionOnScreen.x <0 || enemyPositionOnScreen.x > Screen.width 
            || enemyPositionOnScreen.y <0 || enemyPositionOnScreen.y > Screen.height) {

            indicatorScoreText.text = stateManager.CurrentScore.ToString();
            //Debug.Log(sizeDelta);
            enemyPositionOnScreen.x = Mathf.Clamp(enemyPositionOnScreen.x, 0, Screen.width - 150);
            enemyPositionOnScreen.y = Mathf.Clamp(enemyPositionOnScreen.y, 0, Screen.height - 150);
            enemyPositionOnScreen.z = 0;

            Vector3 direct = enemyPositionOnScreen - new Vector3(Screen.width/2, Screen.height/2, 0);
            float angle = Mathf.Atan2(direct.y, direct.x) * Mathf.Rad2Deg;
            angle = (angle < 0) ? (angle + 360) : angle;

            indicatorContainer.gameObject.SetActive(true);
            indicatorContainer.GetComponent<RectTransform>().position = enemyPositionOnScreen;

            indicatorIcon.rectTransform.rotation = Quaternion.Euler(0, 0, angle-90);


        }
        else {
            indicatorContainer.gameObject.SetActive(false);
        }
    }
    private async void IndicatorEnemy_OnCharacterDead(object sender, System.EventArgs e) {
        isDead = true;
        await Task.Delay(100);
        Destroy(indicatorContainer);
    }
}
