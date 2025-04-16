using Unity.VisualScripting;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    private static CoinManager instance;
    public static CoinManager Instance { get => instance; }
    [SerializeField] private int beginPlayerCoin;
    [SerializeField] private int winPlayerCoin;
    [SerializeField] private int losePlayerCoin;

    public bool isDoubleAward = false;
    private void Awake() {
        if(beginPlayerCoin != 0 && !PlayerPrefs.HasKey("PlayerCoin"))
            PlayerPrefs.SetInt("PlayerCoin", beginPlayerCoin);

        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    void Start() {
        //GameManager.Instance.OnPlayerWin += CoinManager_OnPlayerWin;
    }

    public bool PurchaseItem(int itemCost) {
        int playerCoin = PlayerPrefs.GetInt("PlayerCoin");
        if (playerCoin >= itemCost) {
            PlayerPrefs.SetInt("PlayerCoin", playerCoin -= itemCost);
            return true;
        }
        return false;
    }

    public void SaveCoin(int coinCount) {
        int coin = PlayerPrefs.GetInt("PlayerCoin");

        if (isDoubleAward)
            coinCount *= 2;

        PlayerPrefs.SetInt("PlayerCoin", coin + coinCount);
        isDoubleAward = false;
    }
    public int GetWinCoin() {
        return winPlayerCoin;
    }

    public int GetLoseCoin() {
        return losePlayerCoin;
    }
}
