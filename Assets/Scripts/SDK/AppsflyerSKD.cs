using AppsFlyerSDK;
using System.Collections.Generic;
using UnityEngine;

public class AppsflyerSKD : MonoBehaviour {
    void Start() {

        // Bat che do Debug
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

        // Su kien mua hang
        Dictionary<string, string> purchaseValues = new Dictionary<string, string>();
        purchaseValues.Add(AFInAppEvents.REVENUE, "4.99");
        purchaseValues.Add(AFInAppEvents.CURRENCY, "USD");
        AppsFlyer.sendEvent(AFInAppEvents.PURCHASE, purchaseValues);

        Debug.Log("Event's sent");
    }
}
