using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FloatingTextAboveCharacter : MonoBehaviour {

    [SerializeField] private GameObject nameContainer;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI nameCharacterText;
    [SerializeField] private TextMeshProUGUI characterScore;
    [SerializeField] private GameObject takeScorePrefab;
    [SerializeField] private Vector3 offset;

    private SkinnedMeshRenderer render;
    private Camera mainCam;
    private StateManager stateManager;
    private GameObject canvas;
    private void Awake() {
        mainCam = Camera.main;
        stateManager = this.GetComponent<StateManager>();
        canvas = GameObject.FindGameObjectWithTag("Canvas");
     }



    private void Start() {
        render = GetComponentInChildren<SkinnedMeshRenderer>();
        backgroundImage.color = render.material.color;
        nameCharacterText.color = render.material.color;

        if (this.CompareTag("Enemy")) {
            int randomNum = (int)Random.Range(0, 100);
            nameCharacterText.text = "Enemy" + randomNum.ToString();
            this.gameObject.transform.parent.name = nameCharacterText.text;
        }

        nameContainer.transform.SetParent(canvas.transform);
        stateManager.OnCharacterLevelUp += FloatingText_OnCharacterLevelUp;
        stateManager.OnCharacterTakeScore += FloatingText_OnCharacterTakeScore;
    }

    private void FixedUpdate() {
        UpdateText();
    }
    private void UpdateText() {
        Vector3 characterOnscreen = mainCam.WorldToScreenPoint(this.transform.position);
        if (characterOnscreen.x < 0 || characterOnscreen.x > Screen.width
            || characterOnscreen.y < 0 || characterOnscreen.y > Screen.height) {
            nameContainer.gameObject.SetActive(false);
            return;
        }

        if(!nameContainer.gameObject.activeSelf) {
            nameContainer.gameObject.SetActive(true);

        }

        characterOnscreen.z = 0;
        nameContainer.GetComponent<RectTransform>().localPosition =
            characterOnscreen - new Vector3(Screen.width / 2, Screen.height / 2) + offset;
    }

    private void FloatingText_OnCharacterLevelUp(object sender, StateManager.OnCharacterLevelUpArg e) {
        offset.y += 60;
    }
    private void FloatingText_OnCharacterTakeScore(object sender, int e) {
        GameObject scorePrefab = Instantiate(takeScorePrefab, canvas.transform);
        Destroy(scorePrefab, 1.5f);
        characterScore.text = e.ToString();
    }

}