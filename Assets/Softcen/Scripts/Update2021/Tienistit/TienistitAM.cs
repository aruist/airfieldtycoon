using System;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;

class TienistitAM : TienistitPerustus
{
    //public static event Action<int> OnInterstitialStatusChanged;
    //public static event Action<int, Tienistit.RewardResult> OnRewardVideoCompleted;

    private InterstitialAd _interstitialAd;
    private BannerView _bannerView;
    private RewardedAd _rewardedAd;
    //private bool _bannerEnabled;
    //private bool _rewardEnabled;
    //private bool _interstitialEnabled;
    //private bool _isInitialized;
    //private bool _rewardGranted;
    //private int _rewardVideoId;
    private bool _bannerShowDelayed;

    public override void Init(bool initInterstitial, bool initReward, bool initBanner) {
        #if AD_DEBUG
        Debug.Log("TienistitAM Init");
        #endif
        base.Init(initInterstitial, initReward, initBanner);
        // When true all events raised by GoogleMobileAds will be raised
        // on the Unity main thread. The default value is false.
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        // Google AdMob:
        MobileAds.SetiOSAppPauseOnBackground(true);

        List<string> deviceIds = new List<string>() { AdRequest.TestDeviceSimulator };
        #if AD_DEBUG
        RequestConfiguration requestConfiguration = new RequestConfiguration();
        MobileAds.SetRequestConfiguration(requestConfiguration);
        #if UNITY_ANDROID
        requestConfiguration.TestDeviceIds.Add("551201A6EFE2230E52EF79158FAAE7B4"); // Samsung S8        
        #elif UNITY_IPHONE
        requestConfiguration.TestDeviceIds.Add("01b4cbc3806ef65f028a09ab27a1d500"); // Ari's iPhone ASI
        requestConfiguration.TestDeviceIds.Add("578bfe765db1300bf172f71310f39fbe");
        requestConfiguration.TestDeviceIds.Add("625ff9e0004fcd3ea05e4d0084db5bbe");
        #endif
        #endif

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            /*if (OnInterstitialStatusChanged == null) {
                _isInitialized = false;
                return;
            }*/
            // This callback is called once the MobileAds SDK is initialized.
            #if AD_DEBUG
            Dictionary<string, AdapterStatus> map = initStatus.getAdapterStatusMap();
            foreach (KeyValuePair<string, AdapterStatus> keyValuePair in map) {
                string className = keyValuePair.Key;
                AdapterStatus status = keyValuePair.Value;
                Debug.Log("TienistitAM MobileAds.Initialize className: " + className + ", status:  " + status.Description);
            }
            #endif
            _isInitialized = true;
            if (_bannerShowDelayed) {
                _bannerShowDelayed = false;
                LoadBannerAd();
            }
            if (_interstitialEnabled) {
                LoadInterstitialAd();
            }
            if (_rewardEnabled) {
                LoadRewardedAd();
            }
        });
    }
    public override void DestroyAll()
    {
        #if AD_DEBUG
        Debug.Log("TienistitAM DestroyAll");
        #endif
        DestroyInterstialAd();
        DestroyBannerAd(); 
        DestroyRewaredAd();
    }
// ----------------------------------------------------------------------------
// ----------------------------------------------------------------------------
#region InterstitialAd
    public override bool CanShowInterstitialAd() {
        bool result = false;
        if (_isInitialized && _interstitialEnabled && _interstitialAd != null) {
            result = _interstitialAd.CanShowAd();
        }
        #if AD_DEBUG
        Debug.Log("TienistitAM CanShowInterstitialAd" + result);
        #endif
        return result;
    }
    /// <summary>
    /// Loads the interstitial ad.
    /// </summary>
    private void LoadInterstitialAd()
    {
        if (!_interstitialEnabled) {
            return;            
        }
        // Clean up the old ad before loading a new one.
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        } 

        #if AD_DEBUG
        Debug.Log("TienistitAM Loading the interstitial ad. id: " + TienistitId.gp_interstitialId());
        #endif

        // create our request used to load the ad.
        var adRequest = new AdRequest();
        adRequest.Keywords.Add("game");
        adRequest.Keywords.Add("idle");
        adRequest.Keywords.Add("rpg");
        adRequest.Keywords.Add("action");

        // send the request to load the ad.
        InterstitialAd.Load(TienistitId.gp_interstitialId(), adRequest,(InterstitialAd ad, LoadAdError error) =>
        {
            // if error is not null, the load request failed.
            if (error != null || ad == null)
            {
                #if AD_DEBUG
                Debug.LogError("TienistitAM interstitial ad failed to load an ad " +
                                 "with error : " + error);
                #endif
                return;
            }

            #if AD_DEBUG
            Debug.Log("TienistitAM Interstitial ad loaded with response : "
                        + ad.GetResponseInfo());
            #endif

            _interstitialAd = ad;
            RegisterEventHandlers(_interstitialAd);
        });
    }
    /// <summary>
    /// Shows the interstitial ad.
    /// </summary>
    public override void ShowInterstitialAd()
    {
        if (!_interstitialEnabled) {
            return;
        }
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            #if AD_DEBUG
            Debug.Log("TienistitAM Showing interstitial ad.");
            #endif
            _interstitialAd.Show();
        }
        else
        {
            #if AD_DEBUG
            Debug.LogError("TienistitAM Interstitial ad is not ready yet.");
            #endif
        }
    }

    public void DestroyInterstialAd() {
        if (_interstitialAd != null) {
            #if AD_DEBUG
            Debug.Log("TienistitAM DestroyInterstialAd");
            #endif
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }
    }

    private void RegisterEventHandlers(InterstitialAd ad)
    {
        #if AD_DEBUG
        Debug.Log("TienistitAM RegisterEventHandlers InterstitialAd.");
        #endif

        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            #if AD_DEBUG
            Debug.Log(String.Format("TienistitAM Interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
            #endif
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            #if AD_DEBUG
            Debug.Log("TienistitAM Interstitial ad recorded an impression.");
            #endif
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            #if AD_DEBUG
            Debug.Log("TienistitAM Interstitial ad was clicked.");
            #endif
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            #if AD_DEBUG
            Debug.Log("TienistitAM Interstitial ad full screen content opened.");
            #endif
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            #if AD_DEBUG
            Debug.Log("TienistitAM Interstitial ad full screen content closed.");
            #endif
            // Reload the ad so that we can show another as soon as possible.
            InterstitialStatusChanged(3);
            LoadInterstitialAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            #if AD_DEBUG
            Debug.LogError("TienistitAM Interstitial ad failed to open full screen content " +
                        "with error : " + error);
            #endif

            // Reload the ad so that we can show another as soon as possible.
            InterstitialStatusChanged(3);
            LoadInterstitialAd();            
        };
    }
#endregion

// ----------------------------------------------------------------------------
// ----------------------------------------------------------------------------
#region BannerAd
    /// <summary>
    /// Creates a 320x50 banner at top of the screen.
    /// </summary>
    public void CreateBannerView()
    {
        if (!_isInitialized) {
            return;
        }
        #if AD_DEBUG
        Debug.Log("TienistitAM Creating banner view id: " + TienistitId.gp_bannerId() + ", banner enabled: " + _bannerEnabled);
        #endif

        // If we already have a banner, destroy the old one.
        if (_bannerView != null)
        {
            DestroyBannerAd();
        }

        if (_bannerEnabled) {
            _bannerView = new BannerView(TienistitId.gp_bannerId(), AdSize.Banner, AdPosition.Bottom);
            #if AD_DEBUG
            ListenToBannerAdEvents();
            #endif
        }
    }
    /// <summary>
    /// Creates the banner view and loads a banner ad.
    /// </summary>
    public void LoadBannerAd()
    {
        #if AD_DEBUG
        Debug.Log($"TienistitAM Loading banner ad. Banner enabled: {_bannerEnabled}, _isInitialized: {_isInitialized}");
        #endif
        if (_bannerEnabled && !_isInitialized) {
            _bannerShowDelayed = true;
        }
        if (!_bannerEnabled || !_isInitialized) {
            return;
        }
        // create an instance of a banner view first.
        if(_bannerView == null)
        {
            CreateBannerView();
        }
        // create our request used to load the ad.
        var adRequest = new AdRequest();
        //adRequest.Keywords.Add("unity-admob-sample");

        // send the request to load the ad.
        _bannerView.LoadAd(adRequest);
    }

    public override void ShowBanner() {
        if (!_bannerEnabled) {
            return;
        }
        #if AD_DEBUG
        Debug.Log("TienistitAM ShowBanner " + _bannerView != null);
        #endif
        if(_bannerView != null) {
            _bannerView.Show();
        }
    }
    public override void HideBanner() {
        #if AD_DEBUG
        Debug.Log("TienistitAM HideBanner " + _bannerView != null);
        #endif
        if(_bannerView != null) {
            _bannerView.Hide();
        }
    }
    /// <summary>
    /// Destroys the ad.
    /// </summary>
    public void DestroyBannerAd()
    {
        #if AD_DEBUG
        Debug.Log($"TienistitAM DestroyBannerAd, _bannerView: {(_bannerView != null)}");
        #endif
        if (_bannerView != null)
        {
            _bannerView.Destroy();
            _bannerView = null;
        }
    }
    public void DestroyBanner()
    {
        DestroyBannerAd();
    }

    /// <summary>
    /// listen to events the banner may raise.
    /// </summary>
    #if AD_DEBUG
    private void ListenToBannerAdEvents()
    {
        // Raised when an ad is loaded into the banner view.
        _bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("TienistitAM Banner view loaded an ad with response : "
                + _bannerView.GetResponseInfo());
        };
        // Raised when an ad fails to load into the banner view.
        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.Log("TienistitAM Banner view failed to load an ad with error : "
                + error);
        };
        // Raised when the ad is estimated to have earned money.
        _bannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("TienistitAM Banner view paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        _bannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log("TienistitAM Banner view recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        _bannerView.OnAdClicked += () =>
        {
            Debug.Log("Banner view was clicked.");
        };
        // Raised when an ad opened full screen content.
        _bannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("TienistitAM Banner view full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        _bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("TienistitAM Banner view full screen content closed.");
        };
    }
    #endif    
#endregion                        

#region RewardedAd
    public override bool CanShowRewardedAd() {
        if (!_isInitialized || !_rewardEnabled) {
            return false;
        }
        if (_rewardedAd != null) {
            return _rewardedAd.CanShowAd();
        }
        return false;
    }

    private void LoadRewardedAd() {
        if (!_isInitialized) {
            return;
        }
        if (_rewardedAd != null) {
            _rewardedAd.Destroy();
            _rewardedAd = null;
            _rewardGranted = false;
        }
        #if AD_DEBUG
        Debug.Log("TienistitAM LoadRewardedAd. id: " + TienistitId.gp_rewardId());
        #endif
        var adRequest = new AdRequest();
        RewardedAd.Load(TienistitId.gp_rewardId(), adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            // If the operation failed with a reason.
            if (error != null)
            {
                #if AD_DEBUG
                Debug.LogError("TienistitAM Rewarded ad failed to load an ad with error : " + error);
                #endif
                return;
            }
            // If the operation failed for unknown reasons.
            // This is an unexpected error, please report this bug if it happens.
            if (ad == null)
            {
                #if AD_DEBUG
                Debug.LogError("TienistitAM Unexpected error: Rewarded load event fired with null ad and null error.");
                #endif
                return;
            }

            // The operation completed successfully.
            #if AD_DEBUG
            Debug.Log("TienistitAM Rewarded ad loaded with response : " + ad.GetResponseInfo());
            #endif
            _rewardedAd = ad;
            // Register to ad events to extend functionality.
            RegisterEventHandlers(ad);

        });
    }

    public override void ShowRewardedAd(int id) {
        if (!_isInitialized) {
            return;
        }
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            #if AD_DEBUG
            Debug.Log("TienistitAM Showing rewarded ad. id: " + id);
            #endif
            _rewardVideoId = id;
            _rewardGranted = false;
            _rewardedAd.Show((Reward reward) =>
            {
                _rewardGranted = true;
                #if AD_DEBUG
                Debug.Log(String.Format("TienistitAM Rewarded ad granted a reward: {0} {1}",
                                        reward.Amount,
                                        reward.Type));
                #endif
            });
        }
        else
        {
            #if AD_DEBUG
            Debug.LogError("TienistitAM Rewarded ad is not ready yet.");
            #endif
        }

    }

    public void DestroyRewaredAd() {
        if (_rewardedAd != null)
        {
            #if AD_DEBUG
            Debug.Log("TienistitAM Destroying rewarded ad.");
            #endif
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        #if AD_DEBUG
        Debug.Log("TienistitAM RegisterEventHandlers rewarded ad.");
        #endif
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            #if AD_DEBUG
            Debug.Log(String.Format("TienistitAM Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
            #endif
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            #if AD_DEBUG
            Debug.Log("TienistitAM Rewarded ad recorded an impression.");
            #endif
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            #if AD_DEBUG
            Debug.Log("TienistitAM Rewarded ad was clicked.");
            #endif
        };
        // Raised when the ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            #if AD_DEBUG
            Debug.Log("TienistitAM Rewarded ad full screen content opened.");
            #endif
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            #if AD_DEBUG
            Debug.Log("TienistitAM Rewarded ad full screen content closed.");
            #endif
            //if (OnRewardVideoCompleted != null)
            //{
                if (_rewardGranted) {
                    RewardVideoCompleted(_rewardVideoId, Tienistit.RewardResult.Finished);
                } else {
                    RewardVideoCompleted(_rewardVideoId, Tienistit.RewardResult.Skipped);
                }
            //}
            // Load another
            LoadRewardedAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            #if AD_DEBUG
            Debug.LogError("TienistitAM Rewarded ad failed to open full screen content with error : "
                + error);
            #endif
            //if (OnRewardVideoCompleted != null)
            //{
                RewardVideoCompleted(_rewardVideoId, Tienistit.RewardResult.Failed);
            //}
        };
    }

#endregion
}
