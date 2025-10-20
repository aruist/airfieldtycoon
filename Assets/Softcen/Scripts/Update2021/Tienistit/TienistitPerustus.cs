using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class TienistitPerustus
{
    public static event Action<int> OnInterstitialStatusChanged;
    public static event Action<int, Tienistit.RewardResult> OnRewardVideoCompleted;
    protected bool _bannerEnabled;
    protected bool _rewardEnabled;
    protected bool _interstitialEnabled;
    protected bool _isInitialized;
    protected bool _rewardGranted;
    protected int _rewardVideoId;

    public abstract void DestroyAll();
    public abstract bool CanShowInterstitialAd();
    public abstract void ShowInterstitialAd();
    public abstract void ShowBanner();
    public abstract void HideBanner();
    public abstract bool CanShowRewardedAd();
    public abstract void ShowRewardedAd(int id);

    public virtual void Init(bool initInterstitial, bool initReward, bool initBanner) 
    {
        #if AD_DEBUG
        Debug.Log("TienistitPerustus Init. interstitial " + initInterstitial + ", reward: " + initReward + ", banner" + initBanner);
        #endif
        _bannerEnabled = initBanner;
        _rewardEnabled = initReward;
        _interstitialEnabled = initInterstitial;
    }

    public void SetAdFree(bool state)
    {
        #if AD_DEBUG
        Debug.Log("TienistitPerustus SetAdFree: " + state);
        #endif
        if (state)
        {
            DestroyAll();
        }
    }

    protected void InterstitialStatusChanged(int status) 
    {
        #if AD_DEBUG
        Debug.Log("TienistitPerustus InterstitialStatusChanged status: " + status);
        #endif
        if (OnInterstitialStatusChanged != null) {
            OnInterstitialStatusChanged(status);
        }            
    }

    protected void RewardVideoCompleted(int id, Tienistit.RewardResult result)
    {
        #if AD_DEBUG
        Debug.Log("TienistitPerustus RewardVideoCompleted id: " + id + ", result: " + result.ToString());
        #endif
        if (OnRewardVideoCompleted != null)
        {
            OnRewardVideoCompleted(id, result);
        }

    }
}