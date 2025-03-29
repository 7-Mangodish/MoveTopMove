using System;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
    bool isStart = false;
    bool isDead = false;
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
            isStart = true;
        }
        if (this.gameObject.CompareTag("Player")) {
            isStart = false;
        }

        nameContainer.transform.SetParent(canvas.transform);
        nameContainer.SetActive(false);

        stateManager.OnCharacterTakeScore += FloatingText_OnCharacterTakeScore;
        stateManager.OnCharacterDead += FloatingText_OnCharacterDeadOrWin;

        GameManager.Instance.OnPlayerWin += FloatingText_OnCharacterDeadOrWin;
        
        if(SceneManager.GetActiveScene().buildIndex == 0)
            HomePageManager.Instance.OnStartGame += FloatingText_OnStartGame;
        else {
            if (this.gameObject.CompareTag("Player")) {
                StartPanelManager.Instance.OnTurnOnSetting += Floating_OnSetting;
                StartPanelManager.Instance.OnTurnOffSetting += Floating_OnTurnOffSetting;
            }

            StartPanelManager.Instance.OnStartZombieMode += FloatingText_OnStartZombieMode;

        }
    }

    private void FixedUpdate() {
        if (isDead)
            return;
        if(isStart)
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

    private void FloatingText_OnCharacterTakeScore(object sender, int e) {
        if (this.gameObject.CompareTag("Player")) {
            GameObject scorePrefab = Instantiate(takeScorePrefab, canvas.transform);
            Destroy(scorePrefab, 1.5f);
        }
        characterScore.text = e.ToString();
    }

    private void FloatingText_OnStartZombieMode(object sender, EventArgs e) {
        nameContainer.gameObject.SetActive(true);
    }
    private void FloatingText_OnCharacterDeadOrWin(object sender, System.EventArgs e) {
        isDead = true;
        //Debug.Log(this.gameObject.transform.parent.name);
        Destroy(nameContainer.gameObject);
    }

    private void Floating_OnSetting(object sender, EventArgs e) {
        nameContainer.gameObject.SetActive(false);
    }

    private void Floating_OnTurnOffSetting(object sender, EventArgs e) {
        nameContainer.gameObject.SetActive(true);
    }
    private void FloatingText_OnStartGame(object sender, System.EventArgs e) {
        isStart = true;
    }
}