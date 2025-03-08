using UnityEngine;
using UnityEngine.UI;

public class EnemyIndicator : MonoBehaviour
{
    [SerializeField] private Canvas targetCanvas;
    [SerializeField] private Image indicatorImage;
    private Camera mainCamera;
    private Sprite indicatorSprite;
    private void Awake() {
        indicatorSprite = indicatorImage.sprite;
        mainCamera = Camera.main;
    }
    void Start()
    {
        indicatorImage.transform.SetParent(targetCanvas.transform);
        indicatorImage.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateIndicator(); 
    }

    void UpdateIndicator() {
        Vector3 enemyPositionOnScreen = mainCamera.WorldToScreenPoint(this.transform.position);
        if(enemyPositionOnScreen.x <0 || enemyPositionOnScreen.x > Screen.width 
            || enemyPositionOnScreen.y <0 || enemyPositionOnScreen.y > Screen.height) {

            float spriteHalfWidth = (float)indicatorSprite.bounds.size.x / 2;
            float spriteHalfHeight = (float)indicatorSprite.bounds.size.y / 2;

            enemyPositionOnScreen.x = Mathf.Clamp(enemyPositionOnScreen.x, spriteHalfWidth, Screen.width-spriteHalfWidth);
            enemyPositionOnScreen.y = Mathf.Clamp(enemyPositionOnScreen.y, spriteHalfHeight, Screen.height-spriteHalfHeight);

            indicatorImage.gameObject.SetActive(true);
            indicatorImage.rectTransform.position = enemyPositionOnScreen;
        }
        else {
            indicatorImage.gameObject.SetActive(false);
        }
    }
}
