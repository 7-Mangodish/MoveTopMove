using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FloatingTextAboveCharacter : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    private Transform worldSpaceCanvas;

    [SerializeField] private SkinnedMeshRenderer skinCharacter;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI nameCharacter;
    [SerializeField] private GameObject scoreTextPrefab;
    [SerializeField] private TextMeshProUGUI characterScoreText;
    private Transform mainCam;
    private Transform unit;
    private StateManager ownerStateManager;

    private void Awake() {
        ownerStateManager= GetComponentInParent<StateManager>();
        if(ownerStateManager == null) {
            Debug.Log("Null owwnerStateManager");
            return;
        }
        ownerStateManager.OnCharacterTakeScore += FloatingText_OnCharacterTakeScore ;
        ownerStateManager.OnCharacterDead += FloatingText_OnCharacterDead;
        worldSpaceCanvas = GameObject.FindGameObjectWithTag("WorldSpaceCanvas").transform;

        image.color = skinCharacter.material.color;
        nameCharacter.color =skinCharacter.material.color;
    }



    void Start()
    {
        mainCam = Camera.main.transform;
        unit = transform.parent;
        Debug.Log(transform.parent.name);   
        this.transform.SetParent(worldSpaceCanvas);
        this.transform.rotation = Quaternion.LookRotation(transform.position - mainCam.transform.position);
        Debug.Log(this.transform.position);
        Debug.Log(mainCam.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        FloatingStatus();
    }

    private void FloatingStatus() {
        this.transform.position = unit.gameObject.transform.position + offset;
    }

    private void FloatingText_OnCharacterTakeScore(object sender, int newScore) {
        GameObject scorePrefab = Instantiate(scoreTextPrefab, this.transform.position, Quaternion.identity);
        scorePrefab.transform.SetParent(this.transform);
        scorePrefab.transform.position = this.transform.position + offset;

        characterScoreText.text = newScore.ToString();
    }

    private void FloatingText_OnCharacterDead(object sender, System.EventArgs e) {
        Destroy(this.gameObject);
    }
}
