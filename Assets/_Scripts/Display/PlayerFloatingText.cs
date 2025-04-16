using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;  

public class PlayerFloatingText : MonoBehaviour
{
    
    [SerializeField] private RectTransform characterCanvas;
    [SerializeField] private Image characterScoreBackground;
    [SerializeField] private TextMeshProUGUI characterScoreText;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private GameObject textScoreEff;
    private SkinnedMeshRenderer characterSkinMesh;
    private StateManager stateManager;
    private PlayerFloatingText floatingText;
    private Camera mainCamera;

    private void Awake() {
        mainCamera = Camera.main;
        stateManager = GetComponent<StateManager>();
        characterSkinMesh = GetComponentInChildren<SkinnedMeshRenderer>();
        floatingText = this;

        stateManager.OnCharacterTakeScore += FloatingText_OnCharacterTakeScore;
    }

    private void Start() {    
        floatingText.enabled = false;

    }

    void Update()
    {
        characterCanvas.rotation = Quaternion.Euler(45, 0, 0);
    }
    private void OnEnable() {
        characterCanvas.gameObject.SetActive(true);
        characterScoreBackground.color = characterSkinMesh.material.color;
        characterNameText.color = characterSkinMesh.material.color;
        characterNameText.text = PlayerPrefs.GetString("PlayerName");
    }

    private void OnDisable() {
        characterCanvas.gameObject.SetActive(false);
    }
    private async void FloatingText_OnCharacterTakeScore(object sender, int e) {
        characterScoreText.text = e.ToString();
        textScoreEff.gameObject.SetActive(true);
        await Task.Delay(500);
        textScoreEff.gameObject.SetActive(false);
    }
    private void FloatingText_OnStartGame(object sender, System.EventArgs e) {
        floatingText.enabled = true;
    }
    
}
