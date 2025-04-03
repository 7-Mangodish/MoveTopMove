using UnityEngine;


public class IntructionPanelManager : MonoBehaviour
{
    private static IntructionPanelManager instance;
    public static IntructionPanelManager Instance { get { return instance; } }

    [SerializeField] private GameObject intructionPanel;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else
            Destroy(this.gameObject);
    }
    public void TurnOnPanel() {
        intructionPanel.gameObject.SetActive(true);
    }

    public void TurnOffPanel() {
        intructionPanel.gameObject.SetActive(false);
    }
}
