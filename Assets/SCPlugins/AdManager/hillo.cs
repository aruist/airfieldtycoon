#if EI_KAYTOSSA
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using ChartboostSDK;
using GoogleMobileAds.Api;
using UnityEngine.Advertisements;
using Beebyte.Obfuscator;
//using System.Runtime.InteropServices.WindowsRuntime;

public class hillo : MonoBehaviour {
    public static hillo Instance = null;

    //public static event Action<int> OnRewardVideoCompleted;
    //public static event Action<int> OnInterstitialStatusChanged;

    public string UnityRewardedVideoID = "rewardedVideo";
    public int m_ApplovinRewardVideoCount = 1;
#if USE_APPLOVING
    private int m_CurrentApplovinRewardVideoCount = 0;
#endif

    //public static AdManager Instance = null;
    public bool useRandomInterstitial = false;
    public string[] adMobKeywords;
    public bool useRewardVideo = false;
    public bool useRewardApplovin = true;
    public bool useRewardChartboost = true;
    public bool useRewardUnityAds = true;

    public bool useAmazonBanner = false;
    public int showFirstAmazonInterstitialCount = 3;
    public bool startBannerAwake = true;
    public bool startChartboost = true;
    private float m_adStartTime;
    private int m_rewardVideoId;

    private float m_InterstitialShowTime;
    private RewardBasedVideoAd rewardBasedVideo;

    public List<int> completedRewardList;
    public bool m_AdManager_OnInterstitialStatusChanged = false;

    public enum SCAdPosition
    {
        Bottom = 0,
        Top = 1,
    }
    private enum AdOperator
    {
        None = 0,
        AdMob = 1,
        iAd = 2,
        AmazonAds = 3
    };
    public enum InterstitialOperators
    {
        None,
        AdMob,
        Chartboost,
        Amazon,
        Applovin,
        UnityAds
    }
    private List<InterstitialOperators> interstialList;
    private List<InterstitialOperators> rewardVideoList;

    private InterstitialOperators m_LastShowedInterstitial;

    public SCAdPosition m_adPosition;
    private bool m_Hide = false;

    private AdOperator _currentOperator;
    private ScreenOrientation currentScreenOrientation;

    private float adWaitTimer;

    private bool adsInitialized = false;
    private bool adMobRewardedRegistered = false;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
#if SOFTCEN_DEBUG
        Debug.Log("hillo: Awake()");
#endif
        Instance = this;
        completedRewardList = new List<int> ();

        m_InterstitialShowTime = GameConsts.InterstitialShowTime;
        m_LastShowedInterstitial = InterstitialOperators.None;
        currentScreenOrientation = Screen.orientation;
        _currentOperator = AdOperator.None;
        m_adStartTime = Time.time; // - GameConsts.chartBoostShowTime;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
#if UNITY_ANDROID
        MobileAds.Initialize(M4hVva1c.ZTGjqBkg(afxh3lw.L_23sd.lvl6));
#endif
#if UNITY_IOS
        MobileAds.Initialize(M4hVva1c.ZTGjqBkg(afxh3lw.L_23sd.lvl7));
#endif


        this.rewardBasedVideo = RewardBasedVideoAd.Instance;
        interstialList = new List<InterstitialOperators>();
        rewardVideoList = new List<InterstitialOperators>();
        m_Hide = false;
#if SOFTCEN_DEBUG
        Debug.Log("hillo: Awake AdFree: " + GameManager.Instance.IsAdFree());
#endif

    if (GameManager.Instance.IsAdFree() && !useRewardVideo)
            return;

        ChartboostInit();

        if (!GameManager.Instance.IsAdFree())
        {
            RequestAdMobInterstitial();
        }

#if SOFTCEN_AMAZON
        if (!GameManager.Instance.IsAdFree() )
        {

    		InitAmazonAds();
	    	RequestAmazonInterstitial();
        }
#endif
        AppLovinInit();


        if (!GameManager.Instance.IsAdFree() && startBannerAwake)
            ShowBanner();

        adsInitialized = true;
        adMobRewardedRegistered = true;
        rewardBasedVideo.OnAdLoaded += RewardBasedVideo_OnAdLoaded;
        rewardBasedVideo.OnAdFailedToLoad += RewardBasedVideo_OnAdFailedToLoad;
        rewardBasedVideo.OnAdOpening += RewardBasedVideo_OnAdOpening;
        rewardBasedVideo.OnAdStarted += RewardBasedVideo_OnAdStarted;
        rewardBasedVideo.OnAdRewarded += RewardBasedVideo_OnAdRewarded;
        rewardBasedVideo.OnAdClosed += RewardBasedVideo_OnAdClosed;
        rewardBasedVideo.OnAdLeavingApplication += RewardBasedVideo_OnAdLeavingApplication;
        this.RequestRewardedVideo();
    }

    void RewardBasedVideo_OnAdLeavingApplication (object sender, EventArgs e)
    {
        // Called when the ad click caused the user to leave the application.
        Debug.Log ("RewardBasedVideo_OnAdLeavingApplication");
    }

    void RewardBasedVideo_OnAdClosed (object sender, EventArgs e)
    {
        // Called when the ad is closed.
        Debug.Log ("RewardBasedVideo_OnAdClosed");
        m_AdManager_OnInterstitialStatusChanged = true;
        /*if (OnInterstitialStatusChanged != null)
        {
            OnInterstitialStatusChanged(3);
        }*/
        this.RequestRewardedVideo();
    }

    void RewardBasedVideo_OnAdRewarded (object sender, Reward e)
    {
        // Called when the user should be rewarded for watching a video.
        Debug.Log ("RewardBasedVideo_OnAdRewarded");
        RewardVideoFinished ();
    }

    void RewardBasedVideo_OnAdStarted (object sender, EventArgs e)
    {
        // Called when the ad starts to play.
        Debug.Log ("RewardBasedVideo_OnAdStarted");
    }

    void RewardBasedVideo_OnAdOpening (object sender, EventArgs e)
    {
        // Called when an ad is shown.
        Debug.Log ("RewardBasedVideo_OnAdOpening");
    }

    void RewardBasedVideo_OnAdFailedToLoad (object sender, AdFailedToLoadEventArgs e)
    {
        // Called when an ad request failed to load.
        Debug.Log ("RewardBasedVideo_OnAdFailedToLoad");
    }

    void RewardBasedVideo_OnAdLoaded (object sender, EventArgs e)
    {
        // Called when an ad request has successfully loaded. 
        Debug.Log ("RewardBasedVideo_OnAdLoaded");
    }

    private void RequestRewardedVideo()
    {
        // Android: ca-app-pub-7159985667273944~5497049911
        // Android rewarded: ca-app-pub-7159985667273944/2318240497

        // ios:
        // withApplicationID: ca-app-pub-7159985667273944~9449571519
        // rewarded ios: ca-app-pub-7159985667273944/7247790087
        #if UNITY_ANDROID
        // Test id: string adUnitId = "ca-app-pub-3940256099942544/5224354917";
        string adUnitId = "ca-app-pub-7159985667273944/2318240497";
        #elif UNITY_IPHONE
        //string adUnitId = "ca-app-pub-3940256099942544/1712485313"; // Test ID
        string adUnitId = "ca-app-pub-7159985667273944/7247790087";
        #else
        string adUnitId = "unexpected_platform";
        #endif

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded video ad with the request.
        this.rewardBasedVideo.LoadAd(request, adUnitId);
    }

    private void AppLovinInit()
    {
#if UNITY_IOS && USE_APPLOVING && !UNITY_EDITOR
        AppLovin.SetSdkKey("J17ysbIAbMhUp7nTKFsmh8a6Sbaeraw9c15TdNeDkKiXSU_okrXQz7XPk63ThziJ_pocAQrvmXQC0P-lFWADdh");
#endif
#if USE_APPLOVING && !UNITY_EDITOR
        AppLovin.SetUnityAdListener(gameObject.name);
        AppLovin.InitializeSdk();

        if (!GameManager.Instance.IsAdFree() )
            AppLovin.PreloadInterstitial();

		if (useRewardVideo && useRewardApplovin)
			AppLovin.LoadRewardedInterstitial();
#endif
    }

    private void ChartboostInit()
    {
#if USE_CHARTBOOST
        if (!GameManager.Instance.IsAdFree() && startChartboost)
            Chartboost.cacheInterstitial(CBLocation.Default);
        if (useRewardVideo && useRewardChartboost)
            Chartboost.cacheRewardedVideo(CBLocation.Default);
#endif

    }

    private void RewardVideoFinished()
    {
        completedRewardList.Add (m_rewardVideoId);
        //if (OnRewardVideoCompleted != null)
        //{
        //    OnRewardVideoCompleted(m_rewardVideoId);
        //}
    }

    void Update()
    {
        if (Screen.orientation != currentScreenOrientation)
        {
#if SOFTCEN_DEBUG
            Debug.Log("hillo: Screen orientation changed screen: " + Screen.orientation.ToString() + ", current: " + currentScreenOrientation.ToString());
#endif
        currentScreenOrientation = Screen.orientation;
            DeleteBanners();
            ResetAds();
            return;
        }

#if SOFTCEN_AMAZON
		UpdateAmazon();
#endif
        /*
        #if !DISABLE_IAD && (UNITY_IOS || UNITY_IPHONE)
                    IOSUpdate();
        #endif
                    */
    }

    void OnEnable()
    {
        SubscribeChartBoostEvents();
    }

    void OnDisable()
    {
        UnSubscribeChartBoostEvents();
        DestroyAdMobBanner();
    }

    void OnDestroy()
    {
        DestroyAdMobBanner();
        DestroyAdMobInterstitial();
        DeleteBanners();
        UnSubscribeAmazonAdsEvents();
        if (adMobRewardedRegistered) {
            adMobRewardedRegistered = false;
            rewardBasedVideo.OnAdLoaded -= RewardBasedVideo_OnAdLoaded;
            rewardBasedVideo.OnAdFailedToLoad -= RewardBasedVideo_OnAdFailedToLoad;
            rewardBasedVideo.OnAdOpening -= RewardBasedVideo_OnAdOpening;
            rewardBasedVideo.OnAdStarted -= RewardBasedVideo_OnAdStarted;
            rewardBasedVideo.OnAdRewarded -= RewardBasedVideo_OnAdRewarded;
            rewardBasedVideo.OnAdClosed -= RewardBasedVideo_OnAdClosed;
            rewardBasedVideo.OnAdLeavingApplication -= RewardBasedVideo_OnAdLeavingApplication;
        }

    }

    void OnApplicationPause(bool paused)
    {
#if SOFTCEN_DEBUG
            string str = "hillo: OnApplicationPause: " + paused.ToString();
#if USE_CHARTBOOST
            str += ", Chartboost.isAnyViewVisible: " + Chartboost.isAnyViewVisible();
            str += ", Chartboost.isImpressionVisible: " + Chartboost.isImpressionVisible();
            str += ", Chartboost.hasInterstitial: " + Chartboost.hasInterstitial(CBLocation.Default);
#endif
            str += ", Time.timeScale: " + Time.timeScale;
            Debug.Log(str);
#endif
    if (!adsInitialized)
    return;

        if (paused)
        {
            DeleteBanners();
        }
        else
        {
            ResetAds();
        }
    }

    public void SetInterstitialShowTime(float t)
    {
        m_InterstitialShowTime = t;
    }

    public bool hasRewardVideo(InterstitialOperators optype)
    {
        bool retVal = false;
        switch (optype)
        {
            case InterstitialOperators.AdMob:
                retVal = rewardBasedVideo.IsLoaded ();
                break;
            case InterstitialOperators.Applovin:
#if USE_APPLOVING && !UNITY_EDITOR
			    retVal = AppLovin.IsIncentInterstitialReady();
#endif
                break;
            case InterstitialOperators.Chartboost:
                retVal = Chartboost.hasRewardedVideo(CBLocation.Default);
                break;
            case InterstitialOperators.UnityAds:
                retVal = UnityEngine.Advertisements.Advertisement.IsReady(UnityRewardedVideoID);
                break;

        }
        return retVal;
    }
    public bool hasRewardVideo()
    {
        bool retVal = false;
        if (rewardBasedVideo.IsLoaded()) {
            return true;
        }
#if SOFTCEN_DEBUG
        string dbgStr = "";
#endif
#if UNITY_EDITOR
#if SOFTCEN_DEBUG
        dbgStr += ", Editor SKIP";
#endif
        retVal = true;
#endif
#if USE_UNITYADS
            if (UnityEngine.Advertisements.Advertisement.IsReady(UnityRewardedVideoID))
            {
#if SOFTCEN_DEBUG
                dbgStr += ", UnityADS ready";
#endif
            retVal = true;
            }
#endif
#if USE_CHARTBOOST
            if (Chartboost.hasRewardedVideo(CBLocation.Default))
            {
#if SOFTCEN_DEBUG
                dbgStr += ", CB Reward redy";
#endif
            retVal = true;
            }
            else
                Chartboost.cacheRewardedVideo(CBLocation.Default);
#endif
#if USE_APPLOVING && !UNITY_EDITOR
			if (AppLovin.IsIncentInterstitialReady() && m_CurrentApplovinRewardVideoCount < m_ApplovinRewardVideoCount) {
#if SOFTCEN_DEBUG
			dbgStr += ",Applovin Reward redy";
#endif
                retVal = true;
			}
#endif

#if SOFTCEN_DEBUG
        Debug.Log("hillo: hasRewardVideo " + retVal + dbgStr);
#endif
        return retVal;
    }

#if SOFTCEN_DEBUG
private InterstitialOperators devLastUsedInterstitil = InterstitialOperators.None;
#endif
public void showRewardVideo(InterstitialOperators op, int rewardVideoId) {
    #if SOFTCEN_DEBUG
        devLastUsedInterstitil = op;
    #endif
    if (op == InterstitialOperators.AdMob) {
        if (rewardBasedVideo.IsLoaded ()) {
            rewardBasedVideo.Show();
        }
    } else if (op == InterstitialOperators.Applovin) {
        #if USE_APPLOVING && !UNITY_EDITOR
        if (useRewardApplovin && AppLovin.IsIncentInterstitialReady()) {
        AppLovin.ShowRewardedInterstitial();
        }
        #endif
    } else if (op == InterstitialOperators.Chartboost) {
        if (Chartboost.hasRewardedVideo(CBLocation.Default)) {
            Chartboost.showRewardedVideo(CBLocation.Default);
                    
        }
        
    } else if (op == InterstitialOperators.UnityAds) {
        if (UnityEngine.Advertisements.Advertisement.IsReady(UnityRewardedVideoID)) {
            var options = new ShowOptions { resultCallback = HandleUnityAdsShowResult };
            Advertisement.Show(UnityRewardedVideoID, options);
                      
        }
        
    }
}

    public void showRewardVideo(int rewardVideoId)
    {
		#if SOFTCEN_DEBUG
        bool unityAdsReady = false;
        bool cbReady = false;
        bool applovinRedy = false;
		#endif
        rewardVideoList.Clear();
        m_rewardVideoId = rewardVideoId;

        if (rewardBasedVideo.IsLoaded()) {
            rewardVideoList.Add(InterstitialOperators.AdMob);
            rewardVideoList.Add(InterstitialOperators.AdMob);
            rewardVideoList.Add(InterstitialOperators.AdMob);
        }

#if USE_UNITYADS
        if (useRewardUnityAds && UnityEngine.Advertisements.Advertisement.IsReady(UnityRewardedVideoID))
        {
		#if SOFTCEN_DEBUG
            unityAdsReady = true;
		#endif
            rewardVideoList.Add(InterstitialOperators.UnityAds);
            rewardVideoList.Add(InterstitialOperators.UnityAds);
            rewardVideoList.Add(InterstitialOperators.UnityAds);
        }
#endif
#if USE_CHARTBOOST
        if (useRewardChartboost && Chartboost.hasRewardedVideo(CBLocation.Default))
        {
		#if SOFTCEN_DEBUG
            cbReady = true;
		#endif
            rewardVideoList.Add(InterstitialOperators.Chartboost);
            rewardVideoList.Add(InterstitialOperators.Chartboost);
            rewardVideoList.Add(InterstitialOperators.Chartboost);
        }
#endif

#if USE_APPLOVING && !UNITY_EDITOR
		if (useRewardApplovin && AppLovin.IsIncentInterstitialReady() && m_CurrentApplovinRewardVideoCount < m_ApplovinRewardVideoCount) {
			rewardVideoList.Add(InterstitialOperators.Applovin);
		#if SOFTCEN_DEBUG
            applovinRedy = true;
		#endif
		}
#endif
#if SOFTCEN_DEBUG
     Debug.Log("hillo: showRewardVideo " + rewardVideoId
      + ", UnityAds: " + unityAdsReady
      + ", CB: " + cbReady
      + ", Applovin: " + applovinRedy
       );
#endif
    bool videoStarted = false;
    if (rewardVideoList.Count > 0)
    {
        int index = UnityEngine.Random.Range(0, rewardVideoList.Count);
        InterstitialOperators choosedOp = rewardVideoList [index];
    // TODO: remove this
    #if SOFTCEN_DEBUG
    if (devLastUsedInterstitil != InterstitialOperators.None) {
        if (devLastUsedInterstitil == InterstitialOperators.AdMob && rewardBasedVideo.IsLoaded()) {
                    choosedOp = InterstitialOperators.AdMob;
        } else if (devLastUsedInterstitil == InterstitialOperators.Applovin) {
            #if USE_APPLOVING && !UNITY_EDITOR
            if (useRewardApplovin && AppLovin.IsIncentInterstitialReady() && m_CurrentApplovinRewardVideoCount < m_ApplovinRewardVideoCount) {
            choosedOp = InterstitialOperators.Applovin;
            }
            #endif
        }
        else if (devLastUsedInterstitil == InterstitialOperators.Chartboost && Chartboost.hasRewardedVideo(CBLocation.Default)) {
                    choosedOp = InterstitialOperators.Chartboost;
        }
        else if (devLastUsedInterstitil == InterstitialOperators.UnityAds && UnityEngine.Advertisements.Advertisement.IsReady(UnityRewardedVideoID)) {
            choosedOp = InterstitialOperators.UnityAds;
        }
    }
    #endif

        switch (choosedOp)
        {
            case InterstitialOperators.AdMob:
                rewardBasedVideo.Show();
                videoStarted = true;
                break;
            case InterstitialOperators.Chartboost:
#if USE_CHARTBOOST
#if SOFTCEN_DEBUG
                Debug.Log("hillo: Show CHARTBOOST RewardVideo");
#endif
                Chartboost.showRewardedVideo(CBLocation.Default);
                videoStarted = true;
#endif
                break;

            case InterstitialOperators.UnityAds:
#if USE_UNITYADS
#if SOFTCEN_DEBUG
                Debug.Log("hillo: Show UNITYADS RewardVideo");
#endif
                var options = new ShowOptions { resultCallback = HandleUnityAdsShowResult };
                    Advertisement.Show(UnityRewardedVideoID, options);
                    videoStarted = true;
#endif
                    break;

                case InterstitialOperators.Applovin:
#if USE_APPLOVING
#if SOFTCEN_DEBUG
                    Debug.Log("hillo: Show APPLOVIN RewardVideo");
#endif
                    m_CurrentApplovinRewardVideoCount++;
                    AppLovin.ShowRewardedInterstitial();
                    videoStarted = true;
#endif
                    break;
            }
        }
        if (videoStarted)
        {
#if SOFTCEN_DEBUG
            Debug.Log("hillo: Reward video started!");
#endif
        }
#if UNITY_EDITOR && SOFTCEN_DEBUG
        else
        {
            RewardVideoFinished();
        }
#endif
    }

#if USE_UNITYADS
    private void HandleUnityAdsShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("hillo: The ad was successfully shown.");
                //
                // YOUR CODE TO REWARD THE GAMER
                // Give coins etc.
                RewardVideoFinished();
                break;
            case ShowResult.Skipped:
                Debug.Log("hillo: The ad was skipped before reaching the end.");
                m_rewardVideoId = -1;
                RewardVideoFinished();
                break;
            case ShowResult.Failed:
                Debug.LogError("hillo: The ad failed to be shown.");
                m_rewardVideoId = -1;
                RewardVideoFinished();
                break;
        }
    }
#endif
    public void ShowBanner()
    {
        if (GameManager.Instance.IsAdFree())
            return;

        if (_currentOperator == AdOperator.None)
        {
#if SOFTCEN_AMAZON
			_currentOperator = AdOperator.AmazonAds;
#endif
#if UNITY_ANDROID && !SOFTCEN_AMAZON && !DISABLE_ADMOB
            _currentOperator = AdOperator.AdMob;
#endif
#if UNITY_IOS
			_currentOperator = AdOperator.iAd;
#endif
        }

#if SOFTCEN_DEBUG
        Debug.Log("hillo: ADS ShowBanner " + _currentOperator.ToString());
#endif

        m_Hide = false;

        switch (_currentOperator)
        {
            case AdOperator.AdMob:
                RequestAdMobBanner();
                break;
            case AdOperator.AmazonAds:
                RequestAmazonBanner();
                break;
            case AdOperator.iAd:
                RequestAdMobBanner();
                break;
        }
    }

    public bool BannerActive { get { return !m_Hide; } }

    public void HideBanner()
    {
#if SOFTCEN_DEBUG
        Debug.Log("hillo: HideBanner " + _currentOperator.ToString());
#endif
        m_Hide = true;

        switch (_currentOperator)
        {
            case AdOperator.AdMob:
                DestroyAdMobBanner();
                break;
            case AdOperator.AmazonAds:
                HideAmazonAd();
                break;
            case AdOperator.iAd:
                break;
        }
    }

    private void DeleteBanners()
    {
#if SOFTCEN_DEBUG
        Debug.Log("hillo: DeleteBanners");
#endif
        DestroyAdMobBanner();
        HideAmazonAd();
    }

    public void PermanentlyDisableAds()
    {
#if SOFTCEN_DEBUG
        Debug.Log("hillo: PermanentlyDisableAds");
#endif
        DestroyAdMobBanner();
        DestroyAdMobInterstitial();
        _currentOperator = AdOperator.None;
        m_Hide = true;
        HideAmazonAd();
    }

    private void ResetAds()
    {
#if SOFTCEN_DEBUG
        Debug.Log("hillo: ResetAds _currentOperator:" + _currentOperator.ToString());
#endif
        if (m_Hide)
        {
#if SOFTCEN_DEBUG
            Debug.Log("hillo: ResetAds Skipping");
#endif
            return;
        }

        // TODO: Amazon and IOS Reset
        switch (_currentOperator)
        {
            case AdOperator.None:
                ShowBanner();
                break;

            case AdOperator.AdMob:
                DestroyAdMobBanner();
                RequestAdMobBanner();
                break;
            case AdOperator.AmazonAds:
                LoadAmazonAd();
                break;
            case AdOperator.iAd:
                RequestAdMobBanner();
                break;

            default:
#if SOFTCEN_DEBUG
                Debug.LogWarning("hillo: ResetAds NOT IMPLEMENTED! " + _currentOperator.ToString());
#endif
                break;
        }
    }

    public bool HasInterstitial()
    {
        if (HasInterstitial(InterstitialOperators.AdMob))
            return true;
        if (HasInterstitial(InterstitialOperators.Amazon))
            return true;
        if (HasInterstitial(InterstitialOperators.Applovin))
            return true;
        if (HasInterstitial(InterstitialOperators.Chartboost))
            return true;

        return false;
    }

    public bool HasInterstitial(InterstitialOperators optype)
    {
        bool retval = false;
        switch (optype)
        {
            case InterstitialOperators.AdMob:
                if (admobInterstitial != null)
                {
                    retval = admobInterstitial.IsLoaded();
                }
                break;

            case InterstitialOperators.Amazon:
#if SOFTCEN_AMAZON
                retval = IsAmazonInterstialReady();
#endif
                break;
            case InterstitialOperators.Applovin:
#if USE_APPLOVING && !UNITY_EDITOR
                retval = AppLovin.HasPreloadedInterstitial();
#endif
                break;
            case InterstitialOperators.Chartboost:
#if USE_CHARTBOOST
                retval = Chartboost.hasInterstitial(CBLocation.Default);
#endif
                break;
            case InterstitialOperators.UnityAds:
#if SOFTCEN_DEBUG
                Debug.LogWarning("hillo: Not implemented");
#endif
                break;
        }
        return retval;
    }

    public bool CanShowInterstitialAd(bool useTime)
    {
#if SOFTCEN_DEBUG
        Debug.Log("hillo: CanShowInterstitialAd useTime: " + useTime + ", isAdFree: " + GameManager.Instance.IsAdFree());
#endif
        if (!GameManager.Instance.IsAdFree())
        {
            bool retVal = true;
            float currentTime = Time.time;
            if (useTime)
            {
                if ((currentTime - m_adStartTime) < m_InterstitialShowTime)
                {
                    retVal = false;
                }
            }
#if SOFTCEN_DEBUG
            Debug.Log("hillo: CanShowInterstitialAd deltaTime: " + (currentTime - m_adStartTime) + " < " + m_InterstitialShowTime + ", RETVAL: " + retVal);
#endif
            return retVal;
        }
        return false;

    }



#region GoogleAdMob
    private BannerView admobBannerView;
    private InterstitialAd admobInterstitial;

    //private bool admobBannerEventsSubscribed = false;
    private bool admobInterstitialEventsSubscribed = false;

    private void RequestAdMobBanner()
    {
        if (GameManager.Instance.IsAdFree() || GameConsts.amhix2wb == "")
        {
#if SOFTCEN_DEBUG
            Debug.Log("hillo: GOOGLE BANNER RequestAdMobBanner Skip because adfree or AdmobID not set");
#endif
            _currentOperator = AdOperator.None;
            return;
        }

#if !DISABLE_ADMOB
        if (admobBannerView != null)
        {
#if SOFTCEN_DEBUG
            Debug.Log("hillo: GOOGLE BANNER RequestAdMobBanner Show");
#endif
            admobBannerView.Show();
            return;
        }
        GoogleMobileAds.Api.AdPosition adPos;
        if (m_adPosition == SCAdPosition.Bottom)
        {
            adPos = GoogleMobileAds.Api.AdPosition.Bottom;
        }
        else
        {
            adPos = GoogleMobileAds.Api.AdPosition.Top;
        }
#if SOFTCEN_DEBUG
        Debug.Log("hillo: GOOGLE BANNER RequestAdMobBanner New");
#endif
        _currentOperator = AdOperator.AdMob;
        #if UNITY_IOS
        //if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneX) {
        //    admobBannerView = new BannerView(GameConsts.amhix2wb, AdSize.SmartBanner, 0, 728); // 688
        //} else {
            admobBannerView = new BannerView(GameConsts.amhix2wb, AdSize.SmartBanner, adPos);
        //}
        #else
        admobBannerView = new BannerView(GameConsts.amhix2wb, AdSize.SmartBanner, adPos);
        #endif
        admobBannerView.LoadAd(createAdMobAdRequest());

#else
		_currentOperator = AdOperator.None;
#endif
    }

    private void RequestAdMobInterstitial()
    {
#if !DISABLE_ADMOB
        if (admobInterstitial != null)
            DestroyAdMobInterstitial();
#if SOFTCEN_DEBUG
        Debug.Log("hillo: GOOGLE INTERSTITIAL RequestAdMobInterstitial New");
#endif
        admobInterstitial = new InterstitialAd(GameConsts.amhix2wi);
        if (!admobInterstitialEventsSubscribed)
        {
            admobInterstitialEventsSubscribed = true;
            admobInterstitial.OnAdLoaded += HandleAdMobInterstitialAdLoaded;
            admobInterstitial.OnAdFailedToLoad += HandleAdMobInterstitialAdFailedToLoad;
            admobInterstitial.OnAdClosed += HandleAdMobInterstitialAdClosed;
            admobInterstitial.OnAdOpening += AdmobInterstitial_AdOpened;
        }
        admobInterstitial.LoadAd(createAdMobAdRequest());
#endif
    }


    private void DestroyAdMobInterstitial()
    {
        if (admobInterstitial != null)
        {
#if SOFTCEN_DEBUG
            Debug.Log("hillo: GOOGLE INTERSTITIAL DestroyAdMobInterstitial");
#endif
            if (admobInterstitialEventsSubscribed)
            {
                admobInterstitialEventsSubscribed = false;
                admobInterstitial.OnAdLoaded -= HandleAdMobInterstitialAdLoaded;
                admobInterstitial.OnAdFailedToLoad -= HandleAdMobInterstitialAdFailedToLoad;
                admobInterstitial.OnAdClosed -= HandleAdMobInterstitialAdClosed;
                admobInterstitial.OnAdOpening -= AdmobInterstitial_AdOpened;
            }
            admobInterstitial.Destroy();
            admobInterstitial = null;
        }
    }

    private void ShowAdMobInterstitial()
    {
        if (admobInterstitial != null)
        {
            if (admobInterstitial.IsLoaded())
            {
#if SOFTCEN_DEBUG
                Debug.Log("hillo: GOOGLE INTERSTITIAL ShowAdMobInterstitial");
#endif
                m_adStartTime = Time.time;
                m_LastShowedInterstitial = InterstitialOperators.AdMob;
                admobInterstitial.Show();
            }
        }
    }

    private void AdmobInterstitial_AdOpened(object sender, EventArgs e)
    {
        m_AdManager_OnInterstitialStatusChanged = true;
        /*if (OnInterstitialStatusChanged != null)
        {
            OnInterstitialStatusChanged(0);
        }*/
    }

    void HandleAdMobInterstitialAdClosed(object sender, EventArgs e)
    {
#if SOFTCEN_DEBUG
        Debug.Log("hillo: GOOGLE INTERSTITIAL HandleAdMobInterstitialAdClosed");
#endif
        m_AdManager_OnInterstitialStatusChanged = true;
        /*if (OnInterstitialStatusChanged != null)
        {
            OnInterstitialStatusChanged(1);
        }*/

        RequestAdMobInterstitial();
    }

    void HandleAdMobInterstitialAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
#if SOFTCEN_DEBUG
        Debug.Log("hillo: GOOGLE INTERSTITIAL HandleAdMobInterstitialAdFailedToLoad");
#endif
    }

    void HandleAdMobInterstitialAdLoaded(object sender, EventArgs e)
    {
#if SOFTCEN_DEBUG
        Debug.Log("hillo: GOOGLE INTERSTITIAL HandleAdMobInterstitialAdLoaded");
#endif
    }

    private AdRequest createAdMobAdRequest()
    {
        AdRequest request = new AdRequest.Builder()
#if SOFTCEN_DEBUG
            .AddTestDevice(AdRequest.TestDeviceSimulator)
            .AddTestDevice("B1C7958170E35E796CE2606E00EE3658") // Android
            .AddTestDevice("A9A49E1EF7AE2AAA6ECE0F1A5FF638EA") // Android
            .AddTestDevice("405408742C2FCB884206B2E6FBD27798") // Android New
            .AddTestDevice("6728F97B9F105B95A782D36EBE332E36") // Galaxy S4
            .AddTestDevice("eb6075ef9d4e5caebb4aa9e1833980e5") // IOS
            .AddTestDevice("961561C7EBC127C25D41A47A2D6BCA1B") // Galaxy S4 softcen.test
            .AddTestDevice("8d76fb1fdef21089d52356477277eeba") // iPad mini
#endif
            .AddKeyword("game")
            .Build();

        if (adMobKeywords != null)
        {
            for (int i = 0; i < adMobKeywords.Length; i++)
            {
                request.Keywords.Add(adMobKeywords[i]);
            }
        }
        return request;
    }

    /*void HandleAdMobBannerAdFailedToLoad (object sender, AdFailedToLoadEventArgs e)
    {
#if SOFTCEN_DEBUG
        Debug.Log("hillo: GOOGLE BANNER HandleAdMobBannerAdFailedToLoad");
#endif
    }

    void HandleAdMobBannerAdLoaded (object sender, EventArgs e)
    {
#if SOFTCEN_DEBUG
        Debug.Log("hillo: GOOGLE BANNER HandleAdMobBannerAdLoaded");
#endif
    }*/

    private void DestroyAdMobBanner()
    {
        if (admobBannerView != null)
        {
#if SOFTCEN_DEBUG
            Debug.Log("hillo: GOOGLE BANNER DestroyAdMobBanner");
#endif
        /*if (admobBannerEventsSubscribed)
            {
                admobBannerEventsSubscribed = false;
                //admobBannerView.AdLoaded -= HandleAdMobBannerAdLoaded;
                //admobBannerView.AdFailedToLoad -= HandleAdMobBannerAdFailedToLoad;
            }*/
        admobBannerView.Hide();
            admobBannerView.Destroy();
            admobBannerView = null;
        }
    }
        // End of GoogleAdMob
#endregion


#if USE_CHARTBOOST
    private bool CBEventsSubscribed = false;
#endif
    private void UnSubscribeChartBoostEvents()
    {
#if USE_CHARTBOOST
        if (CBEventsSubscribed)
        {
            CBEventsSubscribed = false;
            Chartboost.didFailToLoadInterstitial -= didFailToLoadInterstitial;
            Chartboost.didDismissInterstitial -= didDismissInterstitial;
            Chartboost.didCloseInterstitial -= didCloseInterstitial;
            Chartboost.didClickInterstitial -= didClickInterstitial;
            Chartboost.didCacheInterstitial -= didCacheInterstitial;
            Chartboost.shouldDisplayInterstitial -= shouldDisplayInterstitial;
            #if !UNITY_ANDROID
            Chartboost.didDisplayInterstitial -= didDisplayInterstitial;
            #endif
            //Chartboost.didFailToLoadMoreApps -= didFailToLoadMoreApps;
            //Chartboost.didDismissMoreApps -= didDismissMoreApps;
            //Chartboost.didCloseMoreApps -= didCloseMoreApps;
            //Chartboost.didClickMoreApps -= didClickMoreApps;
            //Chartboost.didCacheMoreApps -= didCacheMoreApps;
            //Chartboost.shouldDisplayMoreApps -= shouldDisplayMoreApps;
            //Chartboost.didDisplayMoreApps -= didDisplayMoreApps;
            Chartboost.didFailToRecordClick -= didFailToRecordClick;
            Chartboost.didFailToLoadRewardedVideo -= didFailToLoadRewardedVideo;
            Chartboost.didDismissRewardedVideo -= didDismissRewardedVideo;
            Chartboost.didCloseRewardedVideo -= didCloseRewardedVideo;
            Chartboost.didClickRewardedVideo -= didClickRewardedVideo;
            Chartboost.didCacheRewardedVideo -= didCacheRewardedVideo;
            Chartboost.shouldDisplayRewardedVideo -= shouldDisplayRewardedVideo;
            Chartboost.didCompleteRewardedVideo -= didCompleteRewardedVideo;
            //Chartboost.didDisplayRewardedVideo -= didDisplayRewardedVideo;
            Chartboost.didCacheInPlay -= didCacheInPlay;
            Chartboost.didFailToLoadInPlay -= didFailToLoadInPlay;
            Chartboost.didPauseClickForConfirmation -= didPauseClickForConfirmation;
            Chartboost.willDisplayVideo -= willDisplayVideo;
#if UNITY_IPHONE
			Chartboost.didCompleteAppStoreSheetFlow -= didCompleteAppStoreSheetFlow;
#endif
        }
#endif
    }
    private void SubscribeChartBoostEvents()
    {
#if USE_CHARTBOOST
        if (!CBEventsSubscribed)
        {
            CBEventsSubscribed = true;
            Chartboost.didFailToLoadInterstitial += didFailToLoadInterstitial;
            Chartboost.didDismissInterstitial += didDismissInterstitial;
            Chartboost.didCloseInterstitial += didCloseInterstitial;
            Chartboost.didClickInterstitial += didClickInterstitial;
            Chartboost.didCacheInterstitial += didCacheInterstitial;
            Chartboost.shouldDisplayInterstitial += shouldDisplayInterstitial;
        #if !UNITY_ANDROID
            Chartboost.didDisplayInterstitial += didDisplayInterstitial;
        #endif
            //Chartboost.didFailToLoadMoreApps += didFailToLoadMoreApps;
            //Chartboost.didDismissMoreApps += didDismissMoreApps;
            //Chartboost.didCloseMoreApps += didCloseMoreApps;
            //Chartboost.didClickMoreApps += didClickMoreApps;
            //Chartboost.didCacheMoreApps += didCacheMoreApps;
            //Chartboost.shouldDisplayMoreApps += shouldDisplayMoreApps;
            //Chartboost.didDisplayMoreApps += didDisplayMoreApps;
            Chartboost.didFailToRecordClick += didFailToRecordClick;
            Chartboost.didFailToLoadRewardedVideo += didFailToLoadRewardedVideo;
            Chartboost.didDismissRewardedVideo += didDismissRewardedVideo;
            Chartboost.didCloseRewardedVideo += didCloseRewardedVideo;
            Chartboost.didClickRewardedVideo += didClickRewardedVideo;
            Chartboost.didCacheRewardedVideo += didCacheRewardedVideo;
            Chartboost.shouldDisplayRewardedVideo += shouldDisplayRewardedVideo;
            Chartboost.didCompleteRewardedVideo += didCompleteRewardedVideo;
            //Chartboost.didDisplayRewardedVideo += didDisplayRewardedVideo;
            Chartboost.didCacheInPlay += didCacheInPlay;
            Chartboost.didFailToLoadInPlay += didFailToLoadInPlay;
            Chartboost.didPauseClickForConfirmation += didPauseClickForConfirmation;
            Chartboost.willDisplayVideo += willDisplayVideo;
#if UNITY_IPHONE
			Chartboost.didCompleteAppStoreSheetFlow += didCompleteAppStoreSheetFlow;
#endif
        }
#endif
    }

    private InterstitialOperators SeletInterstitialOperator()
    {
        InterstitialOperators op = InterstitialOperators.None;
        bool cbready = false;
        bool admobready = false;
        if (admobInterstitial != null)
        {
            admobready = admobInterstitial.IsLoaded();
        }
		#if SOFTCEN_DEBUG
        bool amazonready = false;
		#endif
        bool applovingready = false;
        interstialList.Clear();

        if (admobready)
        {
            interstialList.Add(InterstitialOperators.AdMob);
            //interstialList.Add(InterstitialOperators.AdMob);
            //interstialList.Add(InterstitialOperators.AdMob);
        }
#if USE_APPLOVING && !UNITY_EDITOR
        applovingready = AppLovin.HasPreloadedInterstitial();
        if (applovingready) {
            interstialList.Add(InterstitialOperators.Applovin);
            interstialList.Add(InterstitialOperators.Applovin);
        }
#endif
#if UNITY_IOS && USE_CHARTBOOST
		if (Chartboost.isAnyViewVisible() == true && Chartboost.isImpressionVisible() == false) 
        {
#if SOFTCEN_DEBUG
			Debug.Log("hillo: Chartboost bug! Skip Chartboost");
#endif
		}
		else {
		cbready = Chartboost.hasInterstitial(CBLocation.Default);
			if (cbready) {
				interstialList.Add(InterstitialOperators.Chartboost);
				interstialList.Add(InterstitialOperators.Chartboost);
				interstialList.Add(InterstitialOperators.Chartboost);
				interstialList.Add(InterstitialOperators.Chartboost);
				interstialList.Add(InterstitialOperators.Chartboost);
			}
		}

#endif
#if !UNITY_IOS && USE_CHARTBOOST
        cbready = Chartboost.hasInterstitial(CBLocation.Default);
        if (cbready)
        {
            interstialList.Add(InterstitialOperators.Chartboost);
            interstialList.Add(InterstitialOperators.Chartboost);
            interstialList.Add(InterstitialOperators.Chartboost);
            interstialList.Add(InterstitialOperators.Chartboost);
            interstialList.Add(InterstitialOperators.Chartboost);
        }

#endif

#if SOFTCEN_AMAZON
		amazonready = IsAmazonInterstialReady();
        if (amazonready) {
            interstialList.Add(InterstitialOperators.Amazon);
        }
        if (amazonready && _amazonInterstitialCounter < showFirstAmazonInterstitialCount) {
            op = InterstitialOperators.Amazon;
        }
        else {
            if (interstialList.Count > 0) {
                int index = UnityEngine.Random.Range(0, interstialList.Count);
                op = interstialList[index];
            }
            else {
                op = InterstitialOperators.None;
            }
        }
#endif

#if !SOFTCEN_AMAZON
        if (useRandomInterstitial)
        {
            if (interstialList.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, interstialList.Count);
#if SOFTCEN_DEBUG
                Debug.Log("hillo: RANDOM ADS 0 - " + interstialList.Count
                    + ", rand: " + index);
#endif
            op = interstialList[index];
            }
            else
            {
                op = InterstitialOperators.None;
            }
        }
        else
        {
            if (cbready)
                op = InterstitialOperators.Chartboost;
            else if (applovingready)
                op = InterstitialOperators.Applovin;
            else if (admobready)
                op = InterstitialOperators.AdMob;
            else if (m_LastShowedInterstitial != InterstitialOperators.None)
            {
#if SOFTCEN_DEBUG
            Debug.Log("hillo: No Interstitial to show");
#endif
        }
    }
#endif

#if SOFTCEN_DEBUG
        Debug.Log("hillo: **** SeletInterstitialOperator"
                    + ", CB: " + cbready
                    + ", AdMob: " + admobready
                    + ", amazon: " + amazonready
                    + ", apploving: " + applovingready
                    + ", SELECTED: " + op.ToString()
                    + " ****"
                    );
#endif
        return op;
    }

    private void ShowChartboostInterstitial()
    {
#if USE_CHARTBOOST
        m_adStartTime = Time.time;
        m_LastShowedInterstitial = InterstitialOperators.Chartboost;
        Chartboost.showInterstitial(CBLocation.Default);
#endif
    }

    public void ShowInterstitial()
    {
        ShowInterstitial(true);
    }

    /// <summary>
    /// Shows interstitial ad
    /// </summary>
    /// <param name="useTime">use ad timer to enable interstitial show.</param>
    public void ShowInterstitial(bool useTime, InterstitialOperators manualOp = InterstitialOperators.None)
    {
        if (CanShowInterstitialAd(useTime) == false)
            return;
        InterstitialOperators op = manualOp;

        if (op == InterstitialOperators.None)
            op = SeletInterstitialOperator();

#if SOFTCEN_DEBUG
        Debug.Log("hillo: ShowInterstitial! " + op.ToString());
#endif
        switch (op)
        {
            case InterstitialOperators.None:
                break;
            case InterstitialOperators.Chartboost:
                m_adStartTime = Time.time;
                ShowChartboostInterstitial();
                break;
            case InterstitialOperators.AdMob:
                m_adStartTime = Time.time;
                ShowAdMobInterstitial();
                break;
            case InterstitialOperators.Amazon:
                m_adStartTime = Time.time;
                ShowAmazonInterstitial();
                break;
            case InterstitialOperators.Applovin:
#if USE_APPLOVING && !UNITY_EDITOR
			m_adStartTime = Time.time;
			AppLovin.ShowInterstitial();
#endif
                break;
            default:
#if SOFTCEN_DEBUG
                Debug.LogWarning("hillo: NOT Implemented!");
#endif
                break;
        }
        /*
#if USE_CHARTBOOST
		bool skip = false;
#if SOFTCEN_AMAZON
		if (m_LastShowedInterstitial != InterstitialOperators.Amazon && _AmazonInterstitialLoaded) {
			m_LastShowedInterstitial = InterstitialOperators.Amazon;
			_AmazonInterstitialLoaded = false;
			AmazonAds.showInterstitialAd();
			return;
		}
#endif

		if (m_LastShowedInterstitial == InterstitialOperators.Chartboost && admobInterstitial != null ) 
		{
			if (admobInterstitial.IsLoaded() )
				skip = true;
		}
		if (!skip && Chartboost.hasInterstitial(CBLocation.Default)) {
#if SOFTCEN_DEBUG
			Debug.Log("hillo: Chartboost Interstitial loaded. Show...");
#endif
			m_adStartTime = Time.time;
			m_LastShowedInterstitial = InterstitialOperators.Chartboost;
			Chartboost.showInterstitial(CBLocation.Default);
			return;
		}
		else {
#if SOFTCEN_DEBUG
			Debug.Log("hillo: Chartboost Interstitial not ready or skip: " + skip.ToString());
#endif
		}

#endif

#if SOFTCEN_APPLOVING
		if (AppLovin.HasPreloadedInterstitial()) {
			m_adStartTime = Time.time;
			AppLovin.ShowInterstitial();
			return;
		}
#endif

		if (AdMobInterstitial != null) {
			if (AdMobInterstitial.IsLoaded()) {
#if SOFTCEN_DEBUG
				Debug.Log("hillo: AdMob Interstitial loaded. Show...");
#endif
				m_adStartTime = Time.time;
				m_LastShowedInterstitial = InterstitialOperators.AdMob;
				AdMobInterstitial.Show();
				return;
			}
			else {
#if SOFTCEN_DEBUG
				Debug.Log("hillo: AdMob Interstitial not loaded");
#endif
			}
		}
*/
    }



#region AmazonAds
    private void ShowAmazonInterstitial()
    {
#if SOFTCEN_AMAZON
		if (IsAmazonInterstialReady()) {
#if SOFTCEN_DEBUG
			Debug.Log("hillo: AMAZON ShowAmazonInterstitial");
#endif
			m_adStartTime = Time.time;
			m_LastShowedInterstitial = InterstitialOperators.Amazon;
            _amazonInterstitialCounter = Math.Min(_amazonInterstitialCounter+1,100);
			amazonMobileAds.ShowInterstitialAd();
		}
#endif
    }
    private void RequestAmazonBanner()
    {
#if SOFTCEN_AMAZON
		if (amazonMobileAds == null)
			InitAmazonAds();
		// Create Banner
#if SOFTCEN_DEBUG
		Debug.Log("hillo: AMAZON RequestAmazonBanner");
#endif
		Placement placement = new Placement();
		placement.Dock = Dock.BOTTOM;
		placement.HorizontalAlign = HorizontalAlign.CENTER;
		placement.AdFit = AdFit.FIT_AD_SIZE;
		amazonBannerAd = amazonMobileAds.CreateFloatingBannerAd(placement);
		LoadAmazonAd();
#else
        RequestAdMobBanner();
#endif
    }

#if SOFTCEN_AMAZON
	private int _amazonInterstitialCounter = 0;

	private float _amazonRefreshTimer;
	private bool _AmazonBannerHided;
	private IAmazonMobileAds amazonMobileAds;
	private Ad amazonBannerAd = null;
	private Ad amazonInterstitialAd = null;
	private bool amazonEventSubscribed = false;
		
	void UpdateAmazon() {
		if (_currentOperator == AdOperator.AmazonAds && !_AmazonBannerHided) {
			_amazonRefreshTimer += Time.deltaTime;
			if (_amazonRefreshTimer >= GameConsts.Ads.AmazonAdsRefresh) {
				LoadAmazonAd();
			}
		}
	}

	private bool IsAmazonInterstialReady() {
		if (amazonMobileAds != null) {
			IsReady response = amazonMobileAds.IsInterstitialAdReady();
			return response.BooleanValue;
		}
		return false;
	}
		
	private void AmazonAdLoaded(Ad args)
	{
		//long identifier = args.Identifier;
#if SOFTCEN_DEBUG
		Debug.Log("hillo: AMAZON AmazonAdLoaded, adtype "+args.AdType.ToString());
#endif
        if (args.AdType == AdType.FLOATING && (GameManager.Instance.IsAdFree() || _AmazonBannerHided)) {
            HideAmazonAd();
        }
	}
		
	private void AmazonAdFailedToLoad(Ad args)
	{
		//long identifier = args.Identifier;
			
#if SOFTCEN_DEBUG
		Debug.Log("hillo: AMAZON AmazonAdFailedToLoad, adtype: " + args.AdType.ToString());
#endif
        if (args.AdType == AdType.FLOATING) {
			HideAmazonAd();
			RequestAdMobBanner();
		}
	}
		
	private void AmazonAdDismissed(Ad args) {
		//long identifier = args.Identifier;
#if SOFTCEN_DEBUG
		Debug.Log("hillo: AMAZON AmazonAdDismissed, adtype: " + args.AdType.ToString());
#endif
        if (args.AdType == AdType.INTERSTITIAL)
        {
            if (OnInterstitialStatusChanged != null)
            {
#if SOFTCEN_DEBUG
                Debug.Log("hillo: AMAZON admanager OnInterstitialStatusChanged");
#endif
                OnInterstitialStatusChanged(10);
            }
        }
		if (amazonMobileAds != null && args.AdType == AdType.INTERSTITIAL) {
			amazonMobileAds.LoadInterstitialAd();
		}
	}


	private void RequestAmazonInterstitial() {
		if (amazonMobileAds == null)
			InitAmazonAds();
			
		// Create Interstitial
#if SOFTCEN_DEBUG
		Debug.Log("hillo: AMAZON RequestAmazonInterstitial");
#endif
		amazonInterstitialAd = amazonMobileAds.CreateInterstitialAd();
		if (amazonInterstitialAd.AdType == AdType.INTERSTITIAL) {
				
		}
		amazonMobileAds.LoadInterstitialAd();

	}
		
	private void InitAmazonAds() {
        if (GameManager.Instance.IsAdFree())
			return;
			
#if SOFTCEN_DEBUG
		Debug.Log("hillo: AMAZON InitAmazonAds");
#endif
		amazonMobileAds = AmazonMobileAdsImpl.Instance;
			
		ApplicationKey key = new ApplicationKey();
		key.StringValue = GameConsts.Ads.AmazonAdsAppKey;
		amazonMobileAds.SetApplicationKey(key);

#if SOFTCEN_DEBUG
			ShouldEnable enable = new ShouldEnable();
			enable.BooleanValue = true;
			amazonMobileAds.EnableTesting(enable);
#endif
		SubscribeAmazonAdsEvents();
	}
	// end of SOFTCEN_AMAZON
#endif

    private void HideAmazonAd()
    {
#if SOFTCEN_AMAZON
#if SOFTCEN_DEBUG
		Debug.Log("hillo: AMAZON HideAmazonAd AdFree: " + GameManager.Instance.IsAdFree());
#endif
		_AmazonBannerHided = true;
		//AmazonAds.removeAds();
		if (amazonMobileAds != null && amazonBannerAd != null) {
			amazonMobileAds.CloseFloatingBannerAd(amazonBannerAd);
		}
#endif
    }

    private void LoadAmazonAd()
    {
#if SOFTCEN_AMAZON
#if SOFTCEN_DEBUG
		Debug.Log("hillo: AMAZON LoadAmazonAd");
#endif
        if (GameManager.Instance.IsAdFree()) {
#if SOFTCEN_DEBUG
			Debug.Log("hillo: AMAZON LoadAmazonAd ADFREE! Hide");
#endif
			HideAmazonAd();
            return;
        }
		if (amazonMobileAds != null && amazonBannerAd != null) {
			LoadingStarted response = amazonMobileAds.LoadAndShowFloatingBannerAd(amazonBannerAd);
			if (!response.BooleanValue) {
#if SOFTCEN_DEBUG
				Debug.Log("hillo: AMAZON LoadAndShowFloatingBannerAd failed");
#endif
				HideAmazonAd();
				RequestAdMobBanner();
			}
			else {
#if SOFTCEN_DEBUG
				Debug.Log("hillo: AMAZON LoadAndShowFloatingBannerAd started");
#endif
			}
		}
		_AmazonBannerHided = false;
		_amazonRefreshTimer = 0f;
		_currentOperator = AdOperator.AmazonAds;
#endif
    }

    private void UnSubscribeAmazonAdsEvents()
    {
#if SOFTCEN_AMAZON
		if (amazonMobileAds != null && amazonEventSubscribed) {
#if SOFTCEN_DEBUG
			Debug.Log("hillo: AMAZON UnSubscribeAmazonAdsEvents");
#endif
			amazonMobileAds.RemoveAdLoadedListener(AmazonAdLoaded);
			amazonMobileAds.RemoveAdFailedToLoadListener(AmazonAdFailedToLoad);
			amazonMobileAds.RemoveAdDismissedListener(AmazonAdDismissed);
		}
#endif
    }

    private void SubscribeAmazonAdsEvents()
    {
#if SOFTCEN_AMAZON
		if (amazonMobileAds!=null && !amazonEventSubscribed) {
#if SOFTCEN_DEBUG
			Debug.Log("hillo: AMAZON SubscribeAmazonAdsEvents");
#endif
			amazonEventSubscribed = true;
			amazonMobileAds.AddAdLoadedListener(AmazonAdLoaded);
			amazonMobileAds.AddAdFailedToLoadListener(AmazonAdFailedToLoad);
			amazonMobileAds.AddAdDismissedListener(AmazonAdDismissed);
        }
#endif
    }

    // End of AmazonAds
#endregion

#if USE_APPLOVING
    void onAppLovinEventReceived(string ev)
    {
#if SOFTCEN_DEBUG
        Debug.Log("hillo: onAppLovinEventReceived: " + ev);
#endif
        if (ev.Contains("DISPLAYEDINTER"))
        {
            m_AdManager_OnInterstitialStatusChanged = true;
            /*if (OnInterstitialStatusChanged != null)
            {
                OnInterstitialStatusChanged(6);
            }*/
        }
        else if (ev.Contains("HIDDENINTER"))
        {
            m_AdManager_OnInterstitialStatusChanged = true;
            /*if (OnInterstitialStatusChanged != null)
            {
                OnInterstitialStatusChanged(7);
            }*/
            if (!GameManager.Instance.IsAdFree())
                AppLovin.PreloadInterstitial();

        }
        else if (ev.Contains("REWARDAPPROVED"))
        {
            completedRewardList.Add (m_rewardVideoId);
            /*if (OnRewardVideoCompleted != null)
            {                
                OnRewardVideoCompleted(m_rewardVideoId);
            }*/
        }
        else if (ev.Contains("REWARDREJECTED") || ev.Contains("USERCLOSEDEARLY") || ev.Contains("REWARDTIMEOUT"))
        {
            completedRewardList.Add (-1);
            /*if (OnRewardVideoCompleted != null)
            {
                OnRewardVideoCompleted(-1);
            }*/
        }
        else if (ev.Contains("HIDDENREWARDED") && m_CurrentApplovinRewardVideoCount < m_ApplovinRewardVideoCount)
        {
            AppLovin.LoadRewardedInterstitial();
        }
    }
#endif

#if USE_CHARTBOOST
    void didFailToLoadInterstitial(CBLocation location, CBImpressionError error)
    {
#if SOFTCEN_DEBUG
        AddLog(string.Format("didFailToLoadInterstitial: {0} at location {1}", error, location));
#endif
        m_AdManager_OnInterstitialStatusChanged = true;

        /*if (OnInterstitialStatusChanged != null)
        {
            OnInterstitialStatusChanged(3);
        }*/
    }

    void didDismissInterstitial(CBLocation location)
    {
#if SOFTCEN_DEBUG
        AddLog("didDismissInterstitial: " + location);
#endif
    }

    void didCloseInterstitial(CBLocation location)
    {
#if SOFTCEN_DEBUG
        AddLog("didCloseInterstitial: " + location);
#endif
        m_AdManager_OnInterstitialStatusChanged = true;

        /*if (OnInterstitialStatusChanged != null)
        {
            OnInterstitialStatusChanged(3);
        }*/

    }

    void didClickInterstitial(CBLocation location)
    {
#if SOFTCEN_DEBUG
        AddLog("didClickInterstitial: " + location);
#endif
    }

    void didCacheInterstitial(CBLocation location)
    {
#if SOFTCEN_DEBUG
        AddLog("didCacheInterstitial: " + location);
#endif
    }

    bool shouldDisplayInterstitial(CBLocation location)
    {
        // return true if you want to allow the interstitial to be displayed
        bool showInterstitial = true;
        if (GameManager.Instance.IsAdFree())
            showInterstitial = false;
#if SOFTCEN_DEBUG
        AddLog("shouldDisplayInterstitial @" + location + " : " + showInterstitial);
#endif
        return showInterstitial;
    }

    #if !UNITY_ANDROID
    void didDisplayInterstitial(CBLocation location)
    {
        #if SOFTCEN_DEBUG
        AddLog("didDisplayInterstitial: " + location);
        #endif
        m_AdManager_OnInterstitialStatusChanged = true;

        /*if (OnInterstitialStatusChanged != null)
        {
            OnInterstitialStatusChanged(4);
        }*/

    }
    #endif

    void didFailToLoadMoreApps(CBLocation location, CBImpressionError error)
    {
#if SOFTCEN_DEBUG
        AddLog(string.Format("didFailToLoadMoreApps: {0} at location: {1}", error, location));
#endif
    }

    void didDismissMoreApps(CBLocation location)
    {
#if SOFTCEN_DEBUG
        AddLog(string.Format("didDismissMoreApps at location: {0}", location));
#endif
    }

    void didCloseMoreApps(CBLocation location)
    {
#if SOFTCEN_DEBUG
        AddLog(string.Format("didCloseMoreApps at location: {0}", location));
#endif
    }

    void didClickMoreApps(CBLocation location)
    {
#if SOFTCEN_DEBUG
        AddLog(string.Format("didClickMoreApps at location: {0}", location));
#endif
    }

    void didCacheMoreApps(CBLocation location)
    {
#if SOFTCEN_DEBUG
        AddLog(string.Format("didCacheMoreApps at location: {0}", location));
#endif
    }

    bool shouldDisplayMoreApps(CBLocation location)
    {
        bool showMoreApps = false;
#if SOFTCEN_DEBUG
        AddLog(string.Format("shouldDisplayMoreApps at location: {0}: {1}", location, showMoreApps));
#endif
        return showMoreApps;
    }

    void didDisplayMoreApps(CBLocation location)
    {
#if SOFTCEN_DEBUG
        AddLog("didDisplayMoreApps: " + location);
#endif
    }

    void didFailToRecordClick(CBLocation location, CBClickError error)
    {
#if SOFTCEN_DEBUG
        AddLog(string.Format("didFailToRecordClick: {0} at location: {1}", error, location));
#endif
    }

    void didFailToLoadRewardedVideo(CBLocation location, CBImpressionError error)
    {
#if SOFTCEN_DEBUG
        AddLog(string.Format("didFailToLoadRewardedVideo: {0} at location {1}", error, location));
#endif
    }

    void didDismissRewardedVideo(CBLocation location)
    {
#if SOFTCEN_DEBUG
        AddLog("didDismissRewardedVideo: " + location);
#endif
    }

    void didCloseRewardedVideo(CBLocation location)
    {
#if SOFTCEN_DEBUG
        AddLog("didCloseRewardedVideo: " + location);
#endif
    }

    void didClickRewardedVideo(CBLocation location)
    {
#if SOFTCEN_DEBUG
        AddLog("didClickRewardedVideo: " + location);
#endif
    }

    void didCacheRewardedVideo(CBLocation location)
    {
#if SOFTCEN_DEBUG
        AddLog("didCacheRewardedVideo: " + location);
#endif
    }

    bool shouldDisplayRewardedVideo(CBLocation location)
    {
        bool showRewardedVideo = useRewardVideo;
#if SOFTCEN_DEBUG
        AddLog("shouldDisplayRewardedVideo @" + location + " : " + showRewardedVideo);
#endif
        return showRewardedVideo;
    }

    void didCompleteRewardedVideo(CBLocation location, int reward)
    {
#if SOFTCEN_DEBUG
        AddLog(string.Format("didCompleteRewardedVideo: reward {0} at location {1}", reward, location));
#endif
        RewardVideoFinished();
        Chartboost.cacheRewardedVideo(CBLocation.Default);
    }
		
    void didCacheInPlay(CBLocation location)
    {
#if SOFTCEN_DEBUG
        AddLog("didCacheInPlay called: " + location);
#endif
    }

    void didFailToLoadInPlay(CBLocation location, CBImpressionError error)
    {
#if SOFTCEN_DEBUG
        AddLog(string.Format("didFailToLoadInPlay: {0} at location: {1}", error, location));
#endif
    }

    void didPauseClickForConfirmation()
    {
#if SOFTCEN_DEBUG
        AddLog("didPauseClickForConfirmation called");
#endif
        //activeAgeGate = true;
    }

    void willDisplayVideo(CBLocation location)
    {
#if SOFTCEN_DEBUG
        AddLog("willDisplayVideo: " + location);
#endif
    }
#endif

#if UNITY_IPHONE
	void didCompleteAppStoreSheetFlow() {
#if SOFTCEN_DEBUG
		AddLog("didCompleteAppStoreSheetFlow");
#endif
	}
#endif

    void AddLog(string text)
    {
#if SOFTCEN_DEBUG
        Debug.Log("hillo: CB " + text);
#endif
    }

}
#endif
