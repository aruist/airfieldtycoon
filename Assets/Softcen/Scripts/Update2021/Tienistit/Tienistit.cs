using System;
using System.Collections.Generic;
using UnityEngine;
//using GoogleMobileAds;
//using GoogleMobileAds.Api;
//using UnityEngine.Advertisements;

public class Tienistit : MonoBehaviour
{
    public static Tienistit Instance = null;
    public static event Action<int, RewardResult> OnRewardVideoCompleted;
    public static event Action<int> OnInterstitialStatusChanged;

    public bool AdMobInterstitial_enabled = true;
    public bool AdMobReward_enabled = true;
    public bool UnityInterstitial_enabled = true;
    public bool UnityReward_enabled = true;
    [SerializeField]
    private bool adFree = false;
    private float adStartTime;
    private float adShowDelayTime = 600;

    private int m_rewardVideoId;

    public enum AdOperator
    {
        None = 0,
        AdMob = 1,
        Unity = 2,
        Any = 3
    };

    public enum RewardResult
    {
        Failed = 0,
        Skipped = 1,
        Finished = 2
    }

    private List<AdOperator> interstialList;
    private List<AdOperator> rewardList;
    private bool initOnlyOnce = false;
    [SerializeField]
    private TienistitAM tienistitAM;
    private TienistitU tienistitU;
    private bool eventHandlersRegistered;

    void Awake()
    {
        #if AD_DEBUG
        Debug.Log("Tienistit Awake");
        #endif
        if (Instance == null)
        {
            Instance = this;
            adStartTime = Time.time - adShowDelayTime;
            DontDestroyOnLoad(gameObject);
            interstialList = new List<AdOperator>();
            rewardList = new List<AdOperator>();
            tienistitAM = new TienistitAM();
            tienistitU = new TienistitU();
            TienistitPerustus.OnInterstitialStatusChanged += Tienistit_OnInterstitialStatusChanged;
            TienistitPerustus.OnRewardVideoCompleted += Tienistit_OnRewardVideo;
            eventHandlersRegistered = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Init()
    {
        if (initOnlyOnce)
        {
            return;
        }
        initOnlyOnce = true;

        tienistitAM.Init(AdMobInterstitial_enabled, AdMobReward_enabled, true);
        tienistitU.Init(AdMobInterstitial_enabled, AdMobReward_enabled, true);
    }

    public void SetadShowDelayTime(float t)
    {
        #if AD_DEBUG
        Debug.Log("Tienistit SetadShowDelayTime: " + t);
        #endif
        this.adShowDelayTime = t;
    }

    private void OnDisable()
    {
        DestroyBanner();
    }

    public void TuhoaMainokset()
    {
        #if AD_DEBUG
        Debug.Log("Tienistit TuhoaMainokset");
        #endif
        OnDestroy();
    }

    private void OnDestroy()
    {
        #if AD_DEBUG
        Debug.Log("Tienistit OnDestroy " + (tienistitAM != null));
        #endif
        if (IsInvoking("DelayedRewardCheck"))
            CancelInvoke("DelayedRewardCheck");
        if (eventHandlersRegistered) {
            TienistitPerustus.OnInterstitialStatusChanged -= Tienistit_OnInterstitialStatusChanged;
            TienistitPerustus.OnRewardVideoCompleted -= Tienistit_OnRewardVideo;
            eventHandlersRegistered = false;
            if (tienistitAM != null) {
                tienistitAM.DestroyAll();
                tienistitAM = null;
            }
            if (tienistitU != null) {
                tienistitU.DestroyAll();
                tienistitU = null;
            }
        }
    }

    public void SetAdFree(bool state)
    {
        #if AD_DEBUG
        Debug.Log("Tienistit SetAdFree: " + state);
        #endif
        adFree = state;
        tienistitAM.SetAdFree(state);
        tienistitU.SetAdFree(state);
    }

    private bool CanShowTimedAd()
    {
        if (!adFree)
        {
            if ((Time.time - adStartTime) >= adShowDelayTime)
            {
                return true;
            }
        }
        return false;
    }

    #region Interstitial
    /***********************************************/
    /****************** INTERSTITIAL****************/
    /***********************************************/
    public bool CanShowInterstitial()
    {
        if (CanShowTimedAd() && HasInterstitial())
        {
            return true;
        }
        return false;
    }

    public bool HasInterstitial(Tienistit.AdOperator adOperator = AdOperator.Any)
    {
        if ((adOperator == AdOperator.Any || adOperator == AdOperator.AdMob) &&
            tienistitAM.CanShowInterstitialAd())
        {
            return true;
        }
        if ((adOperator == AdOperator.Any || adOperator == AdOperator.Unity) &&
            tienistitU.CanShowInterstitialAd())
        {
            return true;
        }
        return false;
    }

    public void ShowInterstitial(bool useTime = true, Tienistit.AdOperator adOperator = AdOperator.Any)
    {
        if (useTime && CanShowTimedAd() || !useTime)
        {
            if (adOperator == AdOperator.Any)
            {
                interstialList.Clear();
                if (tienistitAM.CanShowInterstitialAd()) {
                    interstialList.Add(AdOperator.AdMob);
                }
                if (tienistitU.CanShowInterstitialAd()) {
                    interstialList.Add(AdOperator.Unity);
                }
                if (interstialList.Count > 0)
                {
                    int index = UnityEngine.Random.Range(0, interstialList.Count);
                    #if AD_DEBUG
                    string dbgStr = "Tienistit ShowInterstitial Ads: ";
                    for (int i=0; i < interstialList.Count; i++)
                    {
                        dbgStr += interstialList[i].ToString() + ", ";
                    }
                    dbgStr += "selected: " + interstialList[index].ToString();
                    Debug.Log(dbgStr);
                    #endif
                    ShowInterstitial(interstialList[index]);
                }
                #if AD_DEBUG
                else
                {
                    Debug.Log("Tienistit ShowInterstitial No ads found");
                }
                #endif
                return;
            }
            if (adOperator == AdOperator.AdMob && tienistitAM.CanShowInterstitialAd())
            {
                ShowInterstitial(AdOperator.AdMob);
                return;
            }
            if (adOperator == AdOperator.Unity && tienistitU.CanShowInterstitialAd())
            {
                ShowInterstitial(AdOperator.Unity);
                return;
            }
        }
    }

    private void ShowInterstitial(Tienistit.AdOperator adOperator = AdOperator.Any)
    {
        #if AD_DEBUG
        Debug.Log("Tienistit ShowInterstitial: " + adOperator.ToString());
        #endif
        if (adOperator == AdOperator.AdMob)
        {
            tienistitAM.ShowInterstitialAd();
            adStartTime = Time.time;
        }
        else if (adOperator == AdOperator.Unity)
        {
            tienistitU.ShowInterstitialAd();
            adStartTime = Time.time;
        }
    }

    private void Tienistit_OnRewardVideo(int id, Tienistit.RewardResult result) {
        #if AD_DEBUG
        Debug.Log("Tienistit Tienistit_OnRewardVideo id:" + id + ", result: " + result.ToString());
        #endif
        if (OnRewardVideoCompleted != null)
        {
            if (IsInvoking("DelayedRewardCheck"))
            {
                #if AD_DEBUG
                Debug.Log("Tienistit CancelInvoke DelayedRewardCheck");
                #endif
                CancelInvoke("DelayedRewardCheck");
            }
            //m_rewardEarned = true;
            OnRewardVideoCompleted(m_rewardVideoId, RewardResult.Finished);
        }

    }
    private void Tienistit_OnInterstitialStatusChanged(int state)
    {
        #if AD_DEBUG
        Debug.Log("Tienistit Tienistit_OnInterstitialStatusChanged " + state);
        #endif
        if (OnInterstitialStatusChanged != null)
        {
            OnInterstitialStatusChanged(state);
        }
    }

    #endregion

    #region Rewarded
    /***********************************************/
    /****************** REWARDED *******************/
    /***********************************************/
    public bool HasRewarded(Tienistit.AdOperator adOperator = AdOperator.Any)
    {
        if ((adOperator == AdOperator.Any || adOperator == AdOperator.AdMob) &&
            (tienistitAM.CanShowRewardedAd()))
        {
            #if AD_DEBUG
            Debug.Log("Tienistit HasRewarded AdMob found");
            #endif
            return true;
        }
        if ((adOperator == AdOperator.Any || adOperator == AdOperator.Unity) && tienistitU.CanShowRewardedAd())
        {
            #if AD_DEBUG
            Debug.Log("Tienistit HasRewarded Unity found");
            #endif
            return true;
        }
        return false;
    }
    public void ShowRewarded(int id, Tienistit.AdOperator adOperator = AdOperator.Any)
    {
        if (adOperator == AdOperator.Any)
        {
            rewardList.Clear();
            if (tienistitAM.CanShowRewardedAd()) {
                rewardList.Add(AdOperator.AdMob);
            }
            if (tienistitU.CanShowRewardedAd()) {
                rewardList.Add(AdOperator.Unity);
            }
            if (rewardList.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, rewardList.Count);
                    #if AD_DEBUG
                    string dbgStr = "Tienistit ShowRewarded Ads: ";
                    for (int i=0; i < rewardList.Count; i++)
                    {
                        dbgStr += rewardList[i].ToString() + ", ";
                    }
                    dbgStr += "selected: " + rewardList[index].ToString();
                    Debug.Log(dbgStr);
                    #endif
                ShowRewardedContinue(id, rewardList[index]);
            }
            #if AD_DEBUG
            else
            {
                Debug.Log("Tienistit ShowInterstitial No ads found");
            }
            #endif
            return;
        }

        if (adOperator == AdOperator.AdMob && tienistitAM.CanShowRewardedAd())
        {
            ShowRewardedContinue(id, AdOperator.AdMob);
            return;
        }
        if (adOperator == AdOperator.Unity && tienistitU.CanShowRewardedAd())
        {
            ShowRewardedContinue(id, AdOperator.Unity);
            return;
        }
    }
    private void ShowRewardedContinue(int id, Tienistit.AdOperator adOperator = AdOperator.Any)
    {
        #if AD_DEBUG
        Debug.Log("Tienistit ShowRewardedContinue(" + id + ", " + adOperator.ToString());
        #endif
        if (adOperator == AdOperator.AdMob)
        {
            m_rewardVideoId = id;
            tienistitAM.ShowRewardedAd(id);
            return;
        }
        if (adOperator == AdOperator.Unity)
        {
            m_rewardVideoId = id;
            tienistitU.ShowRewardedAd(id);
            return;
        }
        #if AD_DEBUG
        Debug.Log("Tienistit ShowRewardedContinue No ads found");
        #endif
    }
#endregion

#region Banner
    /*_______________________________________________
                       BANNER
    _________________________________________________*/
    //private bool unityBannerShow = false;
    public void DestroyBanner()
    {
        #if AD_DEBUG
        Debug.Log("Tienistit HideBanner() adFree: " + adFree);
        #endif
        if (tienistitAM != null) {
            tienistitAM.DestroyBannerAd();
        }
        //tienistitU.HideBanner();
    }

    public void HideBanner()
    {
        #if AD_DEBUG
        Debug.Log("Tienistit HideBanner() adFree: " + adFree);
        #endif
        if (tienistitAM != null) {
            tienistitAM.HideBanner();
        }
        if (tienistitU != null) {
            tienistitU.HideBanner();
        }
    }
    public void ShowBanner(Tienistit.AdOperator adOperator = AdOperator.Any)
    {
        #if AD_DEBUG
        Debug.Log("Tienistit ShowBanner() adFree: " + adFree + ", adOperator: " + adOperator.ToString());
        #endif
        if (!adFree)
        {
            if (adOperator == AdOperator.Unity)
            {
                tienistitAM.DestroyBannerAd();
                tienistitU.ShowBanner();
                return;
            }
            tienistitAM.LoadBannerAd();
        } else
        {
            DestroyBanner();
        }
    }
#endregion
}

