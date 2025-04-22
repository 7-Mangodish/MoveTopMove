using System.Threading.Tasks;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;

public class CharacterStatus : MonoBehaviour
{
    public static float standardDistance;
    private Vector3 standardScale = new Vector3(0.5f, 0.4f, 0.5f);

    private Vector3 standardCharacterScale;
    private Vector3 standardEulerRot; 
    public GameObject characterStatus;
    public TextMeshPro characterName;
    public TextMeshPro characterScore;
    public SkinnedMeshRenderer characterSkin;

    [Header("-----Player Take Score Effect-----")]
    public GameObject textScoreEffect;
    private StateManager stateManager;


    private void Awake() {
        SetUpParam();
        stateManager = this.GetComponent<StateManager>();
        stateManager.OnCharacterTakeScore += StateManager_OnCharacterTakeScore;
    }



    private void Start() {
        SetUpColor();
    }
    void LateUpdate() {
        UpdateEnemyStatus();
    }

    public void SetUpParam() {
        if (this.gameObject.CompareTag("Player")) {
            Vector3 camToTextVector = characterStatus.transform.position - Camera.main.transform.position;
            standardDistance = Vector3.Dot(camToTextVector, Camera.main.transform.forward);
            Debug.Log(standardDistance);

        }
        standardEulerRot = characterStatus.transform.rotation.eulerAngles;
        standardCharacterScale = this.transform.localScale;

    }

    private void SetUpColor() {
        characterStatus.GetComponent<SpriteRenderer>().color = characterSkin.material.color;
        characterName.color = characterSkin.material.color;
    }

    private void UpdateEnemyStatus() {
        characterStatus.transform.rotation = Quaternion.Euler(standardEulerRot);

        Vector3 camToStatus = characterStatus.transform.position - Camera.main.transform.position;
        float distanceOnCameraForward = Vector3.Dot(camToStatus, Camera.main.transform.forward);

        float propotion = (distanceOnCameraForward / GameVariable.STD_DISTANCE);
        Vector3 newScale = propotion * GameVariable.STD_SCALE;
        
        if(standardCharacterScale != this.transform.localScale) {
            float scaleFactor = this.transform.localScale.x / standardCharacterScale.x;
            newScale /= scaleFactor;
        }
        characterStatus.transform.localScale = newScale;
    }

    private async void StateManager_OnCharacterTakeScore(object sender, int e) {
        characterScore.text = e.ToString();

        if (this.gameObject.CompareTag(GameVariable.PLAYER_TAG)) {
            textScoreEffect.gameObject.SetActive(true);
            await Task.Delay(500);
            textScoreEffect.gameObject.SetActive(false);
        }

    }
}
