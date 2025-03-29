using Unity.VisualScripting;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    private static CoinManager instance;
    public static CoinManager Instance { get => instance; }
    [SerializeField] private int beginPlayerCoin;
    [SerializeField] private int winPlayerCoin;

    public bool isDoubleAward = false;
    private void Awake() {
        if(beginPlayerCoin != 0)
            PlayerPrefs.SetInt("PlayerCoin", beginPlayerCoin);

        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    void Start() {
        //GameManager.Instance.OnPlayerWin += CoinManager_OnPlayerWin;
    }
    //private void CoinManager_OnPlayerWin(object sender, System.EventArgs e) {
    //    int coin = PlayerPrefs.GetInt("PlayerCoin");
    //    int tmpCoint = winPlayerCoin;

    //    if (isDoubleAward)
    //        tmpCoint = winPlayerCoin * 2;

    //    PlayerPrefs.SetInt("PlayerCoin", coin + tmpCoint);
    //    isDoubleAward = false;
    //}

    public bool PurchaseItem(int itemCost) {
        int playerCoin = PlayerPrefs.GetInt("PlayerCoin");
        if (playerCoin >= itemCost) {
            PlayerPrefs.SetInt("PlayerCoin", playerCoin -= itemCost);
            return true;
        }
        return false;
    }

    public void SaveCoin() {
        int coin = PlayerPrefs.GetInt("PlayerCoin");
        int tmpCoint = winPlayerCoin;

        if (isDoubleAward)
            tmpCoint = winPlayerCoin * 2;

        PlayerPrefs.SetInt("PlayerCoin", coin + tmpCoint);
        isDoubleAward = false;
    }
    public int GetWinCoin() {
        return winPlayerCoin;
    }
}
