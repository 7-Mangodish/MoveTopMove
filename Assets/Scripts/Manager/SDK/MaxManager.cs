using System;
using UnityEngine;

public class MaxManager : MonoBehaviour
{
    private static MaxManager instance;
    public static MaxManager Instance { get { return instance; } }

    private string mrecAdUnitId = "163f6d1603f93b2a";
    private string interAdUnitId = "a7641b2cd0514562";
    private string rewardedAdUnitId = "b14b72eb2d1e8ff3";
    private string bannerAdUnitId = "8f70e8f9ea5541a0";

    public event EventHandler<TypeReward> OnPlayerReceiveAward;
    public enum TypeReward {
        revive,
        weapon,
        weaponSkin,
        playerSkin,
        x3Coin

    }

    public TypeReward typeReward;
    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }
    void Start()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration obj) => {
            Debug.Log("MaxSdk is inited");

            MaxSdk.LoadRewardedAd(rewardedAdUnitId);
            MaxSdk.ShowMediationDebugger();
        };
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += Rewarded_OnAdDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += Rewarded_OnAdReceivedRewardEvent; ;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += Rewarded_OnAdHiddenEvent;

        MaxSdk.InitializeSdk();
        //SetUpBannerAd();
    }

    public void SetUpBannerAd() {
        MaxSdk.CreateBanner(bannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);
        MaxSdk.ShowBanner(bannerAdUnitId);
    }

    #region RewardAd

    private void Rewarded_OnAdDisplayEvent(string arg1, MaxSdkBase.AdInfo arg2) {
        Debug.Log("Ad load successfully");
        MaxSdk.LoadRewardedAd(rewardedAdUnitId); // Load lai quang cao
        Time.timeScale = 0;
    }
    public void ShowRewardAd() {
        if (MaxSdk.IsRewardedAdReady(rewardedAdUnitId)) {
            MaxSdk.ShowRewardedAd(rewardedAdUnitId);
        }
    }
    private void Rewarded_OnAdHiddenEvent(string arg1, MaxSdkBase.AdInfo arg2) {
        Time.timeScale = 1;
    }
    private void Rewarded_OnAdReceivedRewardEvent(string arg1, MaxSdkBase.Reward arg2, MaxSdkBase.AdInfo arg3) {
        Debug.Log("Receive Reward");
        Time.timeScale = 1;
        OnPlayerReceiveAward?.Invoke(this, typeReward);
    }

    public void SetTypeReward(TypeReward t) {
        typeReward = t;
    }
    #endregion

}
