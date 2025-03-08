using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FloatingTextAboveCharacter : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private Transform worldSpaceCanvas;

    [SerializeField] private SkinnedMeshRenderer skinCharacter;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI nameCharacter;
    [SerializeField] private GameObject scoreTextPrefab;
    [SerializeField] private TextMeshProUGUI characterCoreText;
    private Transform mainCam;
    private Transform unit;
    private StateManager ownerStateManager;

    private void Awake() {
        ownerStateManager= GetComponentInParent<StateManager>();
        if(ownerStateManager == null) {
            Debug.Log("Null owwnerStateManager");
            return;
        }
        ownerStateManager.OnPlayerTakeScore += FloatingText_OnPlayerTakeScore;

        image.color = skinCharacter.material.color;
        nameCharacter.color =skinCharacter.material.color;
    }

    void Start()
    {
        mainCam = Camera.main.transform;
        unit = transform.parent;
        this.transform.SetParent(worldSpaceCanvas);
    }

    // Update is called once per frame
    void Update()
    {
        FloatingStatus();
    }

    private void FloatingStatus() {
        this.transform.rotation = Quaternion.LookRotation(transform.position - mainCam.transform.position);
        this.transform.position = unit.gameObject.transform.position + offset;
    }

    private void FloatingText_OnPlayerTakeScore(object sender, System.EventArgs e) {
        GameObject scorePrefab = Instantiate(scoreTextPrefab, this.transform.position, Quaternion.identity);
        scorePrefab.transform.SetParent(this.transform);
        scorePrefab.transform.position = this.transform.position + offset;

        characterCoreText.text = ownerStateManager.CurrentScore.ToString();
    }
}
