//using AppsFlyerSDK;
//using System.Collections.Generic;
//using UnityEngine;

//public class AppsflyerSKD : MonoBehaviour
//{
//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Start() {
//        AppsFlyer.setIsDebug(true);
//        //AppsFlyer.setCustomerUserId("someId");
//        AppsFlyer.initSDK("devkey", "appID");
//        AppsFlyer.startSDK();
//    }

//    public void HandlerClickEvent() {
//        Dictionary<string, string> eventValues = new Dictionary<string, string>();
//        eventValues.Add("af_revenue", "4.99");
//        eventValues.Add("af_currency", "USD");
//        eventValues.Add("af_item_id", "item_12345");

//        AppsFlyer.sendEvent("af_purchase", eventValues);
//    }
//}
