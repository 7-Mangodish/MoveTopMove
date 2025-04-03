using UnityEngine;

public class ApplovinSDK : MonoBehaviour {

    [SerializeField] private string mrecAdUnitId = "163f6d1603f93b2a";
    [SerializeField] private string interAdUnitId = "a7641b2cd0514562";
    [SerializeField] private string rewardedAdUnitId = "b14b72eb2d1e8ff3";
    [SerializeField] private string bannerAdUnitId = "8f70e8f9ea5541a0";
     private void Awake() {
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) => {
            Debug.Log("Max Applovin is Set Up");
            MaxSdk.ShowMediationDebugger();
        };

        MaxSdk.InitializeSdk();

        // SetUp Mrec
        MaxSdk.CreateMRec(mrecAdUnitId, MaxSdkBase.AdViewPosition.Centered);
        ShowMrecAd();

        // SetUp Inter
        MaxSdk.LoadInterstitial(interAdUnitId);
        ShowInterAd();

        // Set Up Banner
        MaxSdk.CreateBanner(bannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);
        ShowBannerAd();

        //Tai trc Reward Add
        MaxSdk.LoadRewardedAd(rewardedAdUnitId);
        ShowRewardedAd();




    }

    public void ShowMrecAd() {
        MaxSdk.ShowMRec(mrecAdUnitId);
    }

    public void ShowInterAd() {
        if(MaxSdk.IsInterstitialReady(interAdUnitId))
            MaxSdk.ShowInterstitial(interAdUnitId);
    }
    public void ShowRewardedAd() {
        if (MaxSdk.IsRewardedAdReady(rewardedAdUnitId)) {
            MaxSdk.ShowRewardedAd(rewardedAdUnitId);
        }
    }

    public void ShowBannerAd() {
        MaxSdk.ShowBanner(bannerAdUnitId);
    }
}
