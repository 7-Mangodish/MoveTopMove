using UnityEngine;
public class ExampleHomeScreen : MonoBehaviour {

    private const string AppOpenAdUnitId = "ca-app-pub-6409857233709298/6124157663";
    void Start() {
        MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
        {
            MaxSdkCallbacks.AppOpen.OnAdHiddenEvent += OnAppOpenDismissedEvent;

            if(AppOpenManager.Instance != null) {
                Debug.Log("App open is null");
            }

            MaxSdk.ShowMediationDebugger();
            MaxSdk.LoadAppOpenAd(AppOpenAdUnitId);

            Invoke(nameof(ShowpenAppAd), 1.5f);
            
        };

        MaxSdk.InitializeSdk();
    }

    public void OnAppOpenDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {
        MaxSdk.LoadAppOpenAd(AppOpenAdUnitId);
        Debug.Log("Ad loaded");
    }

    private void OnApplicationPause(bool pauseStatus) {
        if (!pauseStatus) {
            AppOpenManager.Instance.ShowAdIfReady();
        }
    }

    private void ShowpenAppAd() {
        AppOpenManager.Instance.ShowAdIfReady();
    }
    public void TestAOA() {
        AppOpenManager.Instance.ShowAdIfReady();
    }
}

public class AppOpenManager {
    private static AppOpenManager instance = new AppOpenManager();
    public static AppOpenManager Instance { get { return instance; } }

    #if UNITY_IOS
      private const string AppOpenAdUnitId = "«iOS-ad-unit-ID»";
    #else // UNITY_ANDROID
        private const string AppOpenAdUnitId = "ca-app-pub-6409857233709298/6124157663";
    #endif

        public void ShowAdIfReady() {
            if (MaxSdk.IsAppOpenAdReady(AppOpenAdUnitId)) {
                MaxSdk.ShowAppOpenAd(AppOpenAdUnitId);
            }
            else {
                MaxSdk.LoadAppOpenAd(AppOpenAdUnitId);
            }
        }
}