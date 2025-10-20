using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

class TienistitU : TienistitPerustus, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    private bool interstitialLoaded;
    private bool rewardedLoaded;
    private bool bannerLoaded;

    public override void Init(bool initInterstitial, bool initReward, bool initBanner) {
        #if AD_DEBUG
        Debug.Log("TienistitU Init. gameId: " + TienistitId.unity_gameId());
        bool testMode = true;
        #else
        bool testMode = false;
        #endif

        base.Init(initInterstitial, initReward, initBanner);
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(TienistitId.unity_gameId(), testMode, this);
        }        
    }

    public override void DestroyAll()
    {
        #if AD_DEBUG
        Debug.Log("TienistitU DestroyAll");
        #endif
        Advertisement.Banner.Hide(true);
    }
    public override bool CanShowInterstitialAd()
    {
        bool result = false;
        if (_isInitialized && _interstitialEnabled) {
            result = interstitialLoaded;
        }
        #if AD_DEBUG
        Debug.Log("TienistitU CanShowInterstitialAd " + result);
        #endif
        return result;
    }
    public override void ShowInterstitialAd()
    {
        #if AD_DEBUG
        Debug.Log("TienistitU ShowInterstitialAd. id: " + TienistitId.unity_interstialId());
        #endif
        if (!_isInitialized && !_interstitialEnabled) {
            #if AD_DEBUG
            Debug.Log($"TienistitU ShowInterstitialAd skipped. initialized: {_isInitialized}, enabled: {_interstitialEnabled}");
            #endif
            return;
        }
        Advertisement.Show(TienistitId.unity_interstialId(), this);
    }
    public override void ShowBanner()
    {
        #if AD_DEBUG
        Debug.Log("TienistitU ShowBanner _adUnitId: " + TienistitId.unity_bannerId());
        #endif
        if (!_isInitialized || !_bannerEnabled) {
            #if AD_DEBUG
            Debug.Log($"TienistitU ShowBanner skipped. initialized: {_isInitialized}, enabled: {_bannerEnabled}");
            #endif
            return;
        }

        // Set up options to notify the SDK of show events:
        BannerOptions options = new BannerOptions
        {
            clickCallback = OnBannerClicked,
            hideCallback = OnBannerHidden,
            showCallback = OnBannerShown
        };
 
        // Show the loaded Banner Ad Unit:
        Advertisement.Banner.Show(TienistitId.unity_bannerId(), options);
        
    }
    public override void HideBanner()
    {
        #if AD_DEBUG
        Debug.Log("TienistitU HideBanner");
        #endif
        if (_isInitialized && _bannerEnabled && Advertisement.Banner.isLoaded) {
            Advertisement.Banner.Hide();
        }
    }
    public override bool CanShowRewardedAd()
    {
        bool result = false;
        if (_isInitialized && _rewardEnabled) {
            result = rewardedLoaded;
        }
        #if AD_DEBUG
        Debug.Log("TienistitU CanShowRewardedAd " + result);
        #endif
        return result;
    }
    public override void ShowRewardedAd(int id)
    {
        #if AD_DEBUG
        Debug.Log("TienistitU ShowRewardedAd id: " + id + ", _adUnitId: " + TienistitId.unity_rewardId());
        #endif
        if (!_isInitialized || !_rewardEnabled) {
            #if AD_DEBUG
            Debug.Log($"TienistitU ShowRewardedAd skipped. initialized: {_isInitialized}, enabled: {_bannerEnabled}");
            #endif
            return;
        }
        if (rewardedLoaded) {
            _rewardVideoId = id;
            Advertisement.Show(TienistitId.unity_rewardId(), this);
            return;
        }
        #if AD_DEBUG
        Debug.Log("TienistitU ShowRewardedAd reward ad not loaded");
        #endif        
    }
    private void LoadBanner() {
        if (!_isInitialized || !_bannerEnabled) {
            return;
        }
        #if AD_DEBUG
        Debug.Log("TienistitU LoadBanner id: " + TienistitId.unity_bannerId());
        #endif
        // Set up options to notify the SDK of load events:
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };
 
        bannerLoaded = false;
        // Load the Ad Unit with banner content:
        Advertisement.Banner.Load(TienistitId.unity_bannerId(), options);        
    }
    private void LoadRewardedAd() {
        #if AD_DEBUG
        Debug.Log("TienistitU LoadRewardedAd id: " + TienistitId.unity_rewardId());
        #endif
        rewardedLoaded = false;
        Advertisement.Load(TienistitId.unity_rewardId(), this);
    }

    private void LoadInterstitialAd() {
        #if AD_DEBUG
        Debug.Log("TienistitU LoadInterstitialAd id: " + TienistitId.unity_interstialId());
        #endif
        interstitialLoaded = false;
        Advertisement.Load(TienistitId.unity_interstialId(), this);
    }

    public void OnInitializationComplete()
    {
        #if AD_DEBUG
        Debug.Log("TienistitU Unity Ads initialization complete.");
        #endif
        _isInitialized = true;
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        LoadInterstitialAd();
        LoadRewardedAd();
        LoadBanner();
    }
 
    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        #if AD_DEBUG
        Debug.Log($"TienistitU Unity Ads Initialization Failed: {error.ToString()} - {message}");
        #endif
        _isInitialized = false;
        interstitialLoaded = false;
        rewardedLoaded = false;
    }
    // Implement Load Listener and Show Listener interface methods: 
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        #if AD_DEBUG
        Debug.Log("TienistitU OnUnityAdsAdLoaded adUnitId: " + adUnitId);
        #endif
        if (adUnitId.Equals(TienistitId.unity_interstialId())) {
            interstitialLoaded = true;
        }
        else if (adUnitId.Equals(TienistitId.unity_rewardId())) {
            rewardedLoaded = true;
        }
    }
 
    public void OnUnityAdsFailedToLoad(string _adUnitId, UnityAdsLoadError error, string message)
    {
        #if AD_DEBUG
        Debug.Log($"TienistitU Error loading Ad Unit: {_adUnitId} - {error.ToString()} - {message}");
        #endif
        // Optionally execute code if the Ad Unit fails to load, such as attempting to try again.
        if (_adUnitId.Equals(TienistitId.unity_interstialId())) {
            interstitialLoaded = false;
        }
        else if (_adUnitId.Equals(TienistitId.unity_rewardId())) {
            rewardedLoaded = false;
        }
    }
 
    public void OnUnityAdsShowFailure(string _adUnitId, UnityAdsShowError error, string message)
    {
        #if AD_DEBUG
        Debug.Log($"TienistitU Error showing Ad Unit {_adUnitId}: {error.ToString()} - {message}");
        #endif
        // Optionally execute code if the Ad Unit fails to show, such as loading another ad.
        if (_adUnitId.Equals(TienistitId.unity_rewardId()))
        {
            #if AD_DEBUG
            Debug.Log("TienistitU Unity Ads Reward Failed");
            #endif
            RewardVideoCompleted(_rewardVideoId, Tienistit.RewardResult.Failed);
            LoadRewardedAd();
        }
        else if (_adUnitId.Equals(TienistitId.unity_interstialId())) {
            #if AD_DEBUG
            Debug.Log("TienistitU Unity Ads Interstitial Failed");
            #endif
            InterstitialStatusChanged(3);
            LoadInterstitialAd();
        }        
    }
 
    public void OnUnityAdsShowStart(string _adUnitId) { 
        #if AD_DEBUG
        Debug.Log("TienistitU OnUnityAdsShowStart id: " + _adUnitId);
        #endif

    }
    public void OnUnityAdsShowClick(string _adUnitId) { 
        #if AD_DEBUG
        Debug.Log("TienistitU OnUnityAdsShowClick id: " + _adUnitId);
        #endif

    }
    public void OnUnityAdsShowComplete(string _adUnitId, UnityAdsShowCompletionState showCompletionState) { 
        #if AD_DEBUG
        Debug.Log("TienistitU OnUnityAdsShowComplete id: " + _adUnitId);
        #endif
        if (_adUnitId.Equals(TienistitId.unity_rewardId()))
        {
            if (showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED)) {
                #if AD_DEBUG
                Debug.Log("TienistitU Unity Ads Rewarded Ad Completed");
                #endif
                // Grant a reward.
                RewardVideoCompleted(_rewardVideoId, Tienistit.RewardResult.Finished);
                LoadRewardedAd();
            }
            else {
                #if AD_DEBUG
                Debug.Log("TienistitU Unity Ads Rewarded Ad NOT Completed");
                #endif
                RewardVideoCompleted(_rewardVideoId, Tienistit.RewardResult.Skipped);
                LoadRewardedAd();
            }
        }
        else if (_adUnitId.Equals(TienistitId.unity_interstialId())) {
            #if AD_DEBUG
            Debug.Log("TienistitU Unity Ads Interstitial Completed");
            #endif
            InterstitialStatusChanged(3);
            LoadInterstitialAd();
        }        
    }

    void OnBannerLoaded()
    {
        #if AD_DEBUG
        Debug.Log("TienistitU Banner loaded");
        #endif
        bannerLoaded = true;
    }
 
    // Implement code to execute when the load errorCallback event triggers:
    void OnBannerError(string message)
    {
        #if AD_DEBUG
        Debug.Log($"TienistitU Banner Error: {message}");
        #endif
        // Optionally execute additional code, such as attempting to load another ad.
        bannerLoaded = false;
    }

    void OnBannerClicked() { 
        #if AD_DEBUG
        Debug.Log("TienistitU OnBannerClicked");
        #endif
    }
    void OnBannerShown() { 
        #if AD_DEBUG
        Debug.Log("TienistitU OnBannerShown");
        #endif
    }
    void OnBannerHidden() { 
        #if AD_DEBUG
        Debug.Log("TienistitU OnBannerHidden");
        #endif
    }            
}