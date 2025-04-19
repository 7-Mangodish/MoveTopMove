using System;
using UnityEngine;

public class MaxManager : MonoBehaviour
{
    private static MaxManager instance;
    public static MaxManager Instance { get { return instance; } }
    public bool isInit;

    private string mrecAdUnitId = "163f6d1603f93b2a";
    private string interAdUnitId = "a7641b2cd0514562";
    private string rewardedAdUnitId = "b14b72eb2d1e8ff3";
    private string bannerAdUnitId = "8f70e8f9ea5541a0";
    private string appOpenAdUnitId = "ca-app-pub-6409857233709298/6124157663";

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
        if (instance == null) {
            instance = this;
        }
        else
            Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject);
        isInit = false;
    }

    public void InitMaxManager() {
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration obj) => {
            //Debug.Log("MaxSdk is inited");

            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += Rewarded_OnAdDisplayEvent;// Duoc goi khi rewardAd hien thi
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += Rewarded_OnAdReceivedRewardEvent;// dc goi khi nguoi choi nhan dc phan thuong
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += Rewarded_OnAdHiddenEvent;// dc goi khi rewardAd bi dong


            MaxSdkCallbacks.AppOpen.OnAdHiddenEvent += AppOpen_OnAdHiddenEvent;// duoc goi khi OpenAppAd bi tat
            MaxSdkCallbacks.AppOpen.OnAdLoadedEvent += AppOpen_OnAdLoadedEvent;

            MaxSdk.LoadRewardedAd(rewardedAdUnitId);
            MaxSdk.LoadAppOpenAd(appOpenAdUnitId);
            MaxSdk.ShowMediationDebugger();
            isInit = true;
            //Invoke(nameof(ShowAppOpen), 2f);
        };
        MaxSdk.InitializeSdk();
    }

    #region -------------BANNER_ADS-------------
    public void ShowBannerAd() {
        MaxSdk.CreateBanner(bannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);
        MaxSdk.ShowBanner(bannerAdUnitId);
    }

    public void StopShowBannerAd() {
        MaxSdk.HideBanner(bannerAdUnitId);
    }
    #endregion
    #region ---------------REWARD_ADS---------------

    private void Rewarded_OnAdDisplayEvent(string arg1, MaxSdkBase.AdInfo arg2) {
        //Debug.Log("Ad load successfully");
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

    #region -------------APPOPEN_AD--------------
    public void ShowAppOpen() {
        if(MaxSdk.IsAppOpenAdReady(appOpenAdUnitId)) {
            MaxSdk.ShowAppOpenAd(appOpenAdUnitId);
        }
        else {
            MaxSdk.LoadAppOpenAd(appOpenAdUnitId);
        }
    }

    private void OnApplicationPause(bool pause) {
        if (!pause && MaxSdk.IsAppOpenAdReady(appOpenAdUnitId)) {
            ShowAppOpen();
        }
    }
    private void AppOpen_OnAdLoadedEvent(string arg1, MaxSdkBase.AdInfo arg2) {
        Debug.Log("AppOpenAd Loaded");
    }
    private void AppOpen_OnAdHiddenEvent(string arg1, MaxSdkBase.AdInfo arg2) {
        MaxSdk.LoadAppOpenAd(appOpenAdUnitId);
    }
    #endregion
}
