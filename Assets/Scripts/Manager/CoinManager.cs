using Unity.VisualScripting;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    private static CoinManager instance;
    public static CoinManager Instance { get => instance; }
    [SerializeField] private int beginPlayerCoin;

    private void Awake() {
        PlayerPrefs.SetInt("PlayerCoin", beginPlayerCoin);
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    void Start() {
        GameManager.Instance.OnPlayerWin += CoinManager_OnPlayerWin;
    }
    private void CoinManager_OnPlayerWin(object sender, System.EventArgs e) {
        beginPlayerCoin += 500;
        PlayerPrefs.SetInt("PlayerCoin", beginPlayerCoin);
    }

    public bool PurchaseItem(int itemCost) {
        int playerCoin = PlayerPrefs.GetInt("PlayerCoin");
        if (playerCoin >= itemCost) {
            PlayerPrefs.SetInt("PlayerCoin", playerCoin -= itemCost);
            return true;
        }
        return false;
    }
}
