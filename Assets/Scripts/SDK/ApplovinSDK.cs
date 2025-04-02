using UnityEngine;

public class ApplovinSDK : MonoBehaviour
{
    private string bannerAdUnitId = "8f70e8f9ea5541a0";
    private string rewardedAdUnitId = "b14b72eb2d1e8ff3";
    private void Awake() {
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) => {
            Debug.Log("Max Applovin is Set Up");
        };

        MaxSdk.InitializeSdk();

        MaxSdkCallbacks.Banner.OnAdLoadedEvent += Banner_OnAdLoadedEvent;

        // Set Up Banner
        MaxSdk.CreateBanner(bannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);
        MaxSdk.ShowBanner(bannerAdUnitId);

        //Tai trc Reward Add
        MaxSdk.LoadRewardedAd(rewardedAdUnitId);


    }

    private void Banner_OnAdLoadedEvent(string arg1, MaxSdkBase.AdInfo arg2) {
        Debug.Log("Load successfully");
    }

    public void ShowRewardedAd() {
        if (MaxSdk.IsRewardedAdReady(rewardedAdUnitId)) {
            MaxSdk.ShowRewardedAd(rewardedAdUnitId);
        }
    }
}
