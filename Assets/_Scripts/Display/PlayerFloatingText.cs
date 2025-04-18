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
        characterScoreBackground.color = characterSkinMesh.material.color;
        characterNameText.color = characterSkinMesh.material.color;
        characterNameText.text = PlayerPrefs.GetString("PlayerName");
        characterCanvas.gameObject.SetActive(false);

    }

    void Update()
    {
        characterCanvas.rotation = Quaternion.Euler(45, 0, 0);
    }


    private async void FloatingText_OnCharacterTakeScore(object sender, int e) {
        characterScoreText.text = e.ToString();
        textScoreEff.gameObject.SetActive(true);
        await Task.Delay(500);
        textScoreEff.gameObject.SetActive(false);
    }
    
}
