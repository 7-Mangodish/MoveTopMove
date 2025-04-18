using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnemyFloatingText : MonoBehaviour {

    public GameObject nameContainer;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI nameCharacterText;
    [SerializeField] private TextMeshProUGUI characterScore;
    [SerializeField] private Vector3 offset;
    private RectTransform nameContainerRectTransform;

    private SkinnedMeshRenderer render;
    private Camera mainCam;
    private StateManager stateManager;
    private GameObject canvas;
    bool isStart = false;
    private void Awake() {
        mainCam = Camera.main;
        stateManager = this.GetComponent<StateManager>();
        canvas = GameObject.FindGameObjectWithTag("Canvas");
     }

    private void Start() {
        render = GetComponentInChildren<SkinnedMeshRenderer>();
        backgroundImage.color = render.material.color;
        nameCharacterText.color = render.material.color;

        int randomNum = (int)Random.Range(0, 100);
        nameCharacterText.text = "Enemy" + randomNum.ToString();
        this.gameObject.transform.parent.name = nameCharacterText.text;

        nameContainer.transform.SetParent(canvas.transform);     
        nameContainerRectTransform = nameContainer. gameObject.GetComponent<RectTransform>();
    }

    private void FixedUpdate() {
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

        if (!nameContainer.gameObject.activeSelf) {
            nameContainer.gameObject.SetActive(true);

        }

        characterOnscreen.z = 0;
        //nameContainer.GetComponent<RectTransform>().localPosition =
        //    characterOnscreen - new Vector3(Screen.width / 2, Screen.height / 2) + offset;

        nameContainerRectTransform.position = characterOnscreen + offset;
    }
}