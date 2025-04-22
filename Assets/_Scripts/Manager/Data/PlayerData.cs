using UnityEngine;

public class PlayerPersonalData
{
    public string playerName;
    public string deviceModel;
    public int zone;
    public PlayerPersonalData() {
        playerName = "You";
        zone = 1;
    }

    public PlayerPersonalData(string deviceModel) {
        playerName = "You";
        this.deviceModel = deviceModel;
        zone = 1;
    }
}
