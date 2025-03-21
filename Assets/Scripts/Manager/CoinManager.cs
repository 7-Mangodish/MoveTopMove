using UnityEngine;

public class CoinManager : MonoBehaviour
{
    private static CoinManager instance;
    public static CoinManager Instance;
    private int playerCoin;

    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    void Start() {
        GameManager.Instance.OnPlayerWin += CoinManager_OnPlayerWin;
        PlayerPrefs.SetInt("PlayerCoin", 500);
    }
    private void CoinManager_OnPlayerWin(object sender, System.EventArgs e) {
        playerCoin += 500;
        PlayerPrefs.SetInt("PlayerCoin", playerCoin);
    }
    
    //public bool PurchaseItem(int itemCost) {
    //    playerCoin = PlayerPrefs.GetInt("PlayerCoin");
    //    if ( playerCoin >= itemCost) {
    //        PlayerPrefs.SetInt("PlayerCoin",playerCoin -= itemCost);
    //        return true;
    //    }
    //    return false;
    //}
}
