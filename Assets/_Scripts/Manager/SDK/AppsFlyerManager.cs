using AppsFlyerSDK;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AppsFlyermanager : MonoBehaviour
{
    private static AppsFlyermanager instance;
    public static AppsFlyermanager Instance { get => instance; }

    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
        DontDestroyOnLoad(gameObject);
    }
    
    public void InitAppsFlyer() {
        AppsFlyer.setIsDebug(true);

        AppsFlyer.initSDK("devkey", "appID");
        AppsFlyer.startSDK();
    }

    public void TrackTestEvent() {
        // Test Event
        AppsFlyer.sendEvent("test_event", null);

        // Khoi tao tham so
        Dictionary<string, string> eventValues = new Dictionary<string, string>();
        eventValues.Add("param1", "value1");
        eventValues.Add("param2", "value2");
        AppsFlyer.sendEvent("test_event_with_params", eventValues);

        //// Su kien mua hang
        //Dictionary<string, string> purchaseValues = new Dictionary<string, string>();
        //purchaseValues.Add(AFInAppEvents.REVENUE, "4.99");
        //purchaseValues.Add(AFInAppEvents.CURRENCY, "USD");
        //AppsFlyer.sendEvent(AFInAppEvents.PURCHASE, purchaseValues);

        //Debug.Log("Event's sent");
    }
}
