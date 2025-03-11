using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;  

public class FloatingText : MonoBehaviour
{
    
    [SerializeField] private RectTransform characterCanvas;
    [SerializeField] private Image characterScoreBackground;
    [SerializeField] private TextMeshProUGUI characterScoreText;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private GameObject takeScorePrefab;
    private SkinnedMeshRenderer characterSkinMesh;
    private StateManager stateManager;
    private Camera cam;

    private void Awake() {
        cam = Camera.main;
        stateManager = GetComponent<StateManager>();
        stateManager.OnCharacterLevelUp += FloatingText_OnCharacterLevelUp;
        stateManager.OnCharacterTakeScore += FloatingText_OnCharacterTakeScore;
        characterSkinMesh = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    void Start()
    {
        characterScoreBackground.color = characterSkinMesh.material.color;
        characterNameText.color = characterSkinMesh.material.color;
        if (this.CompareTag("Enemy")) {
            int randomNum = (int)Random.Range(0, 100);
            characterNameText.text = "Enemy" + randomNum.ToString();
            this.gameObject.transform.parent.name = characterNameText.text;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCanvas();
    }

    void UpdateCanvas() {
        characterCanvas.position = new Vector3(this.transform.position.x, characterCanvas.position.y, this.transform.position.z);
    }

    private void FloatingText_OnCharacterLevelUp(object sender, StateManager.OnCharacterLevelUpArg arg) {
        float newPosY = this.transform.position.y + arg.deltaPositionY * arg.currentLevel;
        characterCanvas.position += new Vector3(0, newPosY, 0);
        
    }

    private void FloatingText_OnCharacterTakeScore(object sender, int e) {
        GameObject scorePrefab = Instantiate(takeScorePrefab, this.transform.position, Quaternion.identity);
        scorePrefab.transform.SetParent(characterCanvas.transform);
        Destroy(scorePrefab, 1.5f);
        characterScoreText.text = e.ToString();
    }

}
