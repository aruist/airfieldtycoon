#if EI_KAYTOSSA
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

#if USE_CHARTBOOST
using ChartboostSDK;
#endif
using GoogleMobileAds.Api;
#if SOFTCEN_AMAZON
using com.amazon.mas.cpt.ads;
#endif
#if USE_UNITYADS
using UnityEngine.Advertisements;
#endif

//using Beebyte.Obfuscator;

namespace SoftcenSDK 
{
    public class AdManager : MonoBehaviour 
	{
		public static event Action<int> OnRewardVideoCompleted;
        public static event Action<int> OnInterstitialStatusChanged;

		public string UnityRewardedVideoID = "rewardedVideo";
		public int m_ApplovinRewardVideoCount = 1;
#if USE_APPLOVING
        private int m_CurrentApplovinRewardVideoCount = 0;
#endif

		public static AdManager Instance = null;
        public bool useRandomInterstitial = false;
        public string[] adMobKeywords;
		public bool useRewardVideo = false;
        public bool useRewardApplovin = true;
        public bool useRewardChartboost = true;
        public bool useRewardUnityAds = true;

		private bool debuglog;
		public bool useAmazonBanner = false;
        public int showFirstAmazonInterstitialCount = 3;
		public bool startBannerAwake = true;
		public bool startChartboost = true;
		private float m_adStartTime;
		private int m_rewardVideoId;

		private float m_InterstitialShowTime;

		public enum SCAdPosition {
			Bottom = 0,
			Top = 1,
		}
		private enum AdOperator {
			None = 0,
			AdMob = 1,
			AmazonAds = 2
		};
		public enum InterstitialOperators {
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
        //private BannerView adMobBannerView = null;

        private bool adsInitialized = false;

		void Awake() 
		{
#if SOFTCEN_DEBUG
			debuglog = true;
			Debug.Log("AdManager: Awake()");
#else
			debuglog = false;
#endif

			m_InterstitialShowTime = GameConsts.InterstitialShowTime;
			if (Instance == null) {
				m_LastShowedInterstitial = InterstitialOperators.None;
				Instance = this;
				currentScreenOrientation = Screen.orientation;
				_currentOperator = AdOperator.None;
				m_adStartTime = Time.time; // - GameConsts.chartBoostShowTime;
				DontDestroyOnLoad(gameObject);
			}
			else {
				Destroy(this.gameObject);
			}

		}

		void Start()
		{
            interstialList = new List<InterstitialOperators>();
			rewardVideoList = new List<InterstitialOperators>();
            m_Hide = false;
#if SOFTCEN_DEBUG
			if (debuglog) Debug.Log("AdManager: Awake AdFree: " + GameManager.Instance.IsAdFree());
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

        private void RewardVideoFinished() {
			if (OnRewardVideoCompleted != null) {
				OnRewardVideoCompleted(m_rewardVideoId);
			}
		}

		void Update() {
			if (Screen.orientation != currentScreenOrientation) {
#if SOFTCEN_DEBUG
				if (debuglog) Debug.Log("AdManager: Screen orientation changed screen: " + Screen.orientation.ToString() + ", current: " + currentScreenOrientation.ToString());
#endif
				currentScreenOrientation = Screen.orientation;
				DeleteBanners();
				ResetAds();
				return;
			}

#if SOFTCEN_AMAZON
			UpdateAmazon();
#endif
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
		}
		
		void OnApplicationPause(bool paused) 
		{
#if SOFTCEN_DEBUG
			if (debuglog) {
				string str = "AdManager: OnApplicationPause: " + paused.ToString ();
#if USE_CHARTBOOST
                str += ", Chartboost.isAnyViewVisible: " + Chartboost.isAnyViewVisible();
				str += ", Chartboost.isImpressionVisible: " + Chartboost.isImpressionVisible();
				str += ", Chartboost.hasInterstitial: " + Chartboost.hasInterstitial (CBLocation.Default);
#endif
				str += ", Time.timeScale: " + Time.timeScale;
				Debug.Log (str);
			}
#endif
            if (!adsInitialized)
                return;

			if (paused) {
				DeleteBanners();
			}
			else {
				ResetAds();
            }
        }

		public void SetInterstitialShowTime(float t) {
			m_InterstitialShowTime = t;
		}

		public bool hasRewardVideo() {
			bool retVal = false;
#if SOFTCEN_DEBUG
			string dbgStr = "";
#endif
#if UNITY_EDITOR
#if SOFTCEN_DEBUG
			if (debuglog) dbgStr += ", Editor SKIP";
#endif
			retVal = true;
#endif
#if USE_UNITYADS
			if (UnityEngine.Advertisements.Advertisement.IsReady(UnityRewardedVideoID)) {
#if SOFTCEN_DEBUG
				if (debuglog) dbgStr += ", UnityADS ready";
#endif
				retVal = true;
			}
#endif
#if USE_CHARTBOOST
			if (Chartboost.hasRewardedVideo(CBLocation.Default)) {
#if SOFTCEN_DEBUG
				if (debuglog) dbgStr += ", CB Reward redy";
#endif
				retVal = true;                
			}
			else
				Chartboost.cacheRewardedVideo(CBLocation.Default);
#endif
#if USE_APPLOVING && !UNITY_EDITOR
			if (AppLovin.IsIncentInterstitialReady() && m_CurrentApplovinRewardVideoCount < m_ApplovinRewardVideoCount) {
				//if (debuglog) dbgStr += ",Applovin Reward redy";
                retVal = true;
			}
#endif
#if SOFTCEN_DEBUG
			if (debuglog) Debug.Log("AdManager: hasRewardVideo " + retVal + dbgStr);
#endif
			return retVal;
		}

		public void showRewardVideo(int rewardVideoId) {
#if SOFTCEN_DEBUG
            bool unityAdsReady = false;
            bool cbReady = false;
            bool applovinRedy = false;
#endif
			rewardVideoList.Clear();
			m_rewardVideoId = rewardVideoId;
#if USE_UNITYADS
			if (useRewardUnityAds && UnityEngine.Advertisements.Advertisement.IsReady(UnityRewardedVideoID)) {
#if SOFTCEN_DEBUG
                unityAdsReady = true;
#endif
                rewardVideoList.Add(InterstitialOperators.UnityAds);
				rewardVideoList.Add(InterstitialOperators.UnityAds);
				rewardVideoList.Add(InterstitialOperators.UnityAds);
			}
#endif
#if USE_CHARTBOOST
                if (useRewardChartboost && Chartboost.hasRewardedVideo(CBLocation.Default)) {
#if SOFTCEN_DEBUG
                cbReady = true;
#endif
                rewardVideoList.Add(InterstitialOperators.Chartboost);
				rewardVideoList.Add(InterstitialOperators.Chartboost);
				rewardVideoList.Add(InterstitialOperators.Chartboost);
			}
#endif

#if USE_APPLOVING && !UNITY_EDITOR && SOFTCEN_DEBUG
			if (useRewardApplovin && AppLovin.IsIncentInterstitialReady() && m_CurrentApplovinRewardVideoCount < m_ApplovinRewardVideoCount) {
				rewardVideoList.Add(InterstitialOperators.Applovin);
                applovinRedy = true;
			}
#endif
#if SOFTCEN_DEBUG
			if (debuglog) Debug.Log("AdManager: showRewardVideo " + rewardVideoId 
                + ", UnityAds: " + unityAdsReady
                + ", CB: " + cbReady
                + ", Applovin: " + applovinRedy
                 );
#endif
            bool videoStarted = false;
			if (rewardVideoList.Count > 0) {
				int index = UnityEngine.Random.Range(0, rewardVideoList.Count);
				switch (rewardVideoList[index]) {

                    case InterstitialOperators.Chartboost:
#if USE_CHARTBOOST
#if SOFTCEN_DEBUG
						if (debuglog) Debug.Log("AdManager: Show CHARTBOOST RewardVideo");
#endif
					    Chartboost.showRewardedVideo(CBLocation.Default);
					    videoStarted = true;
#endif
    					break;

                    case InterstitialOperators.UnityAds:
#if USE_UNITYADS
#if SOFTCEN_DEBUG
						if (debuglog) Debug.Log("AdManager: Show UNITYADS RewardVideo");
#endif
						var options = new ShowOptions { resultCallback = HandleUnityAdsShowResult };
					    Advertisement.Show(UnityRewardedVideoID, options);
                        videoStarted = true;
#endif
                        break;

                    case InterstitialOperators.Applovin:
#if USE_APPLOVING
#if SOFTCEN_DEBUG
						if (debuglog) Debug.Log("AdManager: Show APPLOVIN RewardVideo");
#endif
					    m_CurrentApplovinRewardVideoCount++;
					    AppLovin.ShowRewardedInterstitial();
					    videoStarted = true;
#endif
                        break;
				}
			}
			if (videoStarted) {
#if SOFTCEN_DEBUG
				if (debuglog) Debug.Log("AdManager: Reward video started!");
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
#if SOFTCEN_DEBUG
				Debug.Log("AdManager: The ad was successfully shown.");
#endif
				//
				// YOUR CODE TO REWARD THE GAMER
				// Give coins etc.
				RewardVideoFinished();
				break;
			case ShowResult.Skipped:
#if SOFTCEN_DEBUG
				Debug.Log("AdManager: The ad was skipped before reaching the end.");
#endif
				m_rewardVideoId = -1;
				RewardVideoFinished();
                break;
            case ShowResult.Failed:
#if SOFTCEN_DEBUG
				Debug.LogError("AdManager: The ad failed to be shown.");
#endif
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

			if (_currentOperator == AdOperator.None) {
#if SOFTCEN_AMAZON
				_currentOperator = AdOperator.AmazonAds;
#endif
#if !SOFTCEN_AMAZON && !DISABLE_ADMOB
				_currentOperator = AdOperator.AdMob;
#endif
            }

#if SOFTCEN_DEBUG
			if (debuglog) Debug.Log("AdManager: ADS ShowBanner " + _currentOperator.ToString());
#endif
			m_Hide = false;

			switch(_currentOperator) {
			case AdOperator.AdMob:
				RequestAdMobBanner();
				break;
			case AdOperator.AmazonAds:
				RequestAmazonBanner();
				break;
			}
		}

        public bool BannerActive {  get { return !m_Hide;  } }

		public void HideBanner()
		{
#if SOFTCEN_DEBUG
			if (debuglog) Debug.Log("AdManager: HideBanner " + _currentOperator.ToString());
#endif
			m_Hide = true;
			
			switch(_currentOperator) {
			case AdOperator.AdMob:
				DestroyAdMobBanner();
				break;
			case AdOperator.AmazonAds:
				HideAmazonAd();
				break;
			}
		}

		private void DeleteBanners() {
#if SOFTCEN_DEBUG
			if (debuglog) Debug.Log("AdManager: DeleteBanners");
#endif
			DestroyAdMobBanner();
            HideAmazonAd();

		}

		public void PermanentlyDisableAds()
		{
#if SOFTCEN_DEBUG
			if (debuglog) Debug.Log("AdManager: PermanentlyDisableAds");
#endif
			DestroyAdMobBanner();
			DestroyAdMobInterstitial();
			_currentOperator = AdOperator.None;
			m_Hide = true;
            HideAmazonAd();

		}
		/*
		public bool IsInterstitialReady() {
			if (CanShowInterstitialAd(true)) {
#if USE_CHARTBOOST
				if (Chartboost.hasInterstitial(CBLocation.Default))
					return true;
#endif
				if (admobInterstitial != null) {
					if (admobInterstitial.IsLoaded()) {
						return true;
					}
				}
			}
			return false;
		}*/

		private void ResetAds() {
#if SOFTCEN_DEBUG
			if (debuglog) Debug.Log("AdManager: ResetAds _currentOperator:" + _currentOperator.ToString());
#endif
			if (m_Hide)
            {
#if SOFTCEN_DEBUG
				if (debuglog) Debug.Log("AdManager: ResetAds Skipping");
#endif
				return;
            }
			
			// TODO: Amazon
			switch(_currentOperator) {
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

			default:
#if SOFTCEN_DEBUG
				Debug.LogWarning("AdManager: ResetAds NOT IMPLEMENTED! " + _currentOperator.ToString());
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
            switch(optype)
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
					if (debuglog) Debug.LogWarning("AdManager: Not implemented");
#endif
					break;
            }
            return retval;
        }

		public bool CanShowInterstitialAd(bool useTime) {
#if SOFTCEN_DEBUG
			if (debuglog) Debug.Log("AdManager: CanShowInterstitialAd useTime: " + useTime + ", isAdFree: " +GameManager.Instance.IsAdFree());
#endif
			if (!GameManager.Instance.IsAdFree()) {
				bool retVal = true;
				float currentTime = Time.time;
				if (useTime) {
					if ((currentTime - m_adStartTime) < m_InterstitialShowTime) {
						retVal = false;
                    }
                }
#if SOFTCEN_DEBUG
				if (debuglog) Debug.Log("AdManager: CanShowInterstitialAd deltaTime: " + (currentTime - m_adStartTime) + " < " + m_InterstitialShowTime + ", RETVAL: " + retVal);
#endif
				return retVal;
			}
			return false;
			
		}



#region GoogleAdMob
		private BannerView admobBannerView = null;
		private InterstitialAd admobInterstitial;

        //private bool admobBannerEventsSubscribed = false;
        private bool admobInterstitialEventsSubscribed = false;

		private void RequestAdMobBanner() {
            if (GameManager.Instance.IsAdFree() || GameConsts.Ads.AdMobId == "")
            {
#if SOFTCEN_DEBUG
                if (debuglog) Debug.Log("AdManager: GOOGLE BANNER RequestAdMobBanner Skip because adfree or AdmobID not set");
#endif
				_currentOperator = AdOperator.None;
                return;
            }

#if !DISABLE_ADMOB
            if (admobBannerView != null) {
#if SOFTCEN_DEBUG
				if (debuglog) Debug.Log("AdManager: GOOGLE BANNER RequestAdMobBanner Show " + GameConsts.Ads.AdMobId);
#endif
				admobBannerView.Show();
				return;
			}
			GoogleMobileAds.Api.AdPosition adPos;
			if (m_adPosition == SCAdPosition.Bottom) {
				adPos = GoogleMobileAds.Api.AdPosition.Bottom;
			}
			else {
				adPos = GoogleMobileAds.Api.AdPosition.Top;
			}
#if SOFTCEN_DEBUG
			if (debuglog) Debug.Log("AdManager: GOOGLE BANNER RequestAdMobBanner New "  + GameConsts.Ads.AdMobId);
#endif
			_currentOperator = AdOperator.AdMob;
			admobBannerView = new BannerView(GameConsts.Ads.AdMobId, AdSize.SmartBanner, adPos);
			admobBannerView.OnAdLoaded += AdmobBannerView_OnAdLoaded;
			admobBannerView.OnAdFailedToLoad += AdmobBannerView_OnAdFailedToLoad;
            admobBannerView.LoadAd(createAdMobAdRequest());

#else
			_currentOperator = AdOperator.None;
#endif
        }

		void AdmobBannerView_OnAdLoaded (object sender, EventArgs e)
		{
#if SOFTCEN_DEBUG
			Debug.Log("AdmobBannerView_OnAdLoaded");
#endif
		}
		void AdmobBannerView_OnAdFailedToLoad (object sender, AdFailedToLoadEventArgs e)
		{
#if SOFTCEN_DEBUG
			Debug.Log("AdmobBannerView_OnAdFailedToLoad " + e.Message);
#endif
		}
        private void RequestAdMobInterstitial() {
#if !DISABLE_ADMOB
			if (admobInterstitial != null)
				DestroyAdMobInterstitial();
#if SOFTCEN_DEBUG
			if (debuglog) Debug.Log("AdManager: GOOGLE INTERSTITIAL RequestAdMobInterstitial New " + GameConsts.Ads.AdMobInterstitial);
#endif
			admobInterstitial = new InterstitialAd(GameConsts.Ads.AdMobInterstitial);
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


        private void DestroyAdMobInterstitial() {
			if (admobInterstitial != null) {
#if SOFTCEN_DEBUG
				if (debuglog) Debug.Log("AdManager: GOOGLE INTERSTITIAL DestroyAdMobInterstitial");
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

		private void ShowAdMobInterstitial() {
			if (admobInterstitial != null) {
				if (admobInterstitial.IsLoaded()) {
#if SOFTCEN_DEBUG
					if (debuglog) Debug.Log("AdManager: GOOGLE INTERSTITIAL ShowAdMobInterstitial");
#endif
					m_adStartTime = Time.time;
					m_LastShowedInterstitial = InterstitialOperators.AdMob;
					admobInterstitial.Show();
				}
			}
		}

        private void AdmobInterstitial_AdOpened(object sender, EventArgs e)
        {
            if (OnInterstitialStatusChanged != null)
            {
                OnInterstitialStatusChanged(0);
            }
        }

        void HandleAdMobInterstitialAdClosed (object sender, EventArgs e)
		{
#if SOFTCEN_DEBUG
			if (debuglog) Debug.Log("AdManager: GOOGLE INTERSTITIAL HandleAdMobInterstitialAdClosed");
#endif
			if (OnInterstitialStatusChanged != null)
            {
                OnInterstitialStatusChanged(1);
            }

            RequestAdMobInterstitial();
		}

		void HandleAdMobInterstitialAdFailedToLoad (object sender, AdFailedToLoadEventArgs e)
		{
#if SOFTCEN_DEBUG
			if (debuglog) Debug.Log("AdManager: GOOGLE INTERSTITIAL HandleAdMobInterstitialAdFailedToLoad");
#endif
        }

        void HandleAdMobInterstitialAdLoaded (object sender, EventArgs e)
		{
#if SOFTCEN_DEBUG
			if (debuglog) Debug.Log("AdManager: GOOGLE INTERSTITIAL HandleAdMobInterstitialAdLoaded");
#endif
		}

		private AdRequest createAdMobAdRequest() {
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

            for (int i=0; i < adMobKeywords.Length; i++) {
                request.Keywords.Add(adMobKeywords[i]);
            }
            return request;
		}
			
		private void DestroyAdMobBanner() {
			if (admobBannerView != null) {
#if SOFTCEN_DEBUG
				if (debuglog) Debug.Log("AdManager: GOOGLE BANNER DestroyAdMobBanner");
#endif
				admobBannerView.OnAdLoaded -= AdmobBannerView_OnAdLoaded;
				admobBannerView.OnAdFailedToLoad -= AdmobBannerView_OnAdFailedToLoad;

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
                Chartboost.didDisplayRewardedVideo -= didDisplayRewardedVideo;
#endif
				Chartboost.didFailToRecordClick -= didFailToRecordClick;
				Chartboost.didFailToLoadRewardedVideo -= didFailToLoadRewardedVideo;
				Chartboost.didDismissRewardedVideo -= didDismissRewardedVideo;
				Chartboost.didCloseRewardedVideo -= didCloseRewardedVideo;
				Chartboost.didClickRewardedVideo -= didClickRewardedVideo;
				Chartboost.didCacheRewardedVideo -= didCacheRewardedVideo;
				Chartboost.shouldDisplayRewardedVideo -= shouldDisplayRewardedVideo;
				Chartboost.didCompleteRewardedVideo -= didCompleteRewardedVideo;
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
				//Chartboost.didDisplayMoreApps += didDisplayMoreApps;
				Chartboost.didDisplayRewardedVideo += didDisplayRewardedVideo;
#endif
                //Chartboost.didFailToLoadMoreApps += didFailToLoadMoreApps;
				//Chartboost.didDismissMoreApps += didDismissMoreApps;
				//Chartboost.didCloseMoreApps += didCloseMoreApps;
				//Chartboost.didClickMoreApps += didClickMoreApps;
				//Chartboost.didCacheMoreApps += didCacheMoreApps;
				//Chartboost.shouldDisplayMoreApps += shouldDisplayMoreApps;
				Chartboost.didFailToRecordClick += didFailToRecordClick;
				Chartboost.didFailToLoadRewardedVideo += didFailToLoadRewardedVideo;
				Chartboost.didDismissRewardedVideo += didDismissRewardedVideo;
				Chartboost.didCloseRewardedVideo += didCloseRewardedVideo;
				Chartboost.didClickRewardedVideo += didClickRewardedVideo;
				Chartboost.didCacheRewardedVideo += didCacheRewardedVideo;
				Chartboost.shouldDisplayRewardedVideo += shouldDisplayRewardedVideo;
				Chartboost.didCompleteRewardedVideo += didCompleteRewardedVideo;
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

		private InterstitialOperators SeletInterstitialOperator() {
			InterstitialOperators op = InterstitialOperators.None;
			bool cbready = false;
			bool admobready = false;
			if (admobInterstitial != null) {
				admobready = admobInterstitial.IsLoaded();
			}
			bool amazonready = false;
			bool applovingready = false;
            interstialList.Clear();

            if (admobready) {
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
			if (Chartboost.isAnyViewVisible() == true && Chartboost.isImpressionVisible() == false) {
#if SOFTCEN_DEBUG
				if (debuglog) Debug.Log("AdManager: Chartboost bug! Skip Chartboost");
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
            if (cbready) {
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
            if (useRandomInterstitial) {
                if (interstialList.Count > 0) {
                    int index = UnityEngine.Random.Range(0, interstialList.Count);
                    if (debuglog) Debug.Log("AdManager: RANDOM ADS 0 - " + interstialList.Count
                        + ", rand: " + index
                            );
                    op = interstialList[index];
                }
                else {
                    op = InterstitialOperators.None;
                }
            }
            else {
                if (cbready)
                    op = InterstitialOperators.Chartboost;
                else if (applovingready)
                    op = InterstitialOperators.Applovin;
                else if (admobready)
                    op = InterstitialOperators.AdMob;
                else if (m_LastShowedInterstitial != InterstitialOperators.None) {
                    if (debuglog) Debug.Log("AdManager: No Interstitial to show");
                }
			}
#endif

			if (debuglog) Debug.Log("AdManager: **** SeletInterstitialOperator"
                                    + ", CB: " + cbready
			                        + ", AdMob: " + admobready
			                        + ", amazon: " + amazonready
			                        + ", apploving: " + applovingready
			                        + ", SELECTED: " + op.ToString()
			                        + " ****"
			                        );
			return op;
		}

		private void ShowChartboostInterstitial() {
#if USE_CHARTBOOST
			m_adStartTime = Time.time;
			m_LastShowedInterstitial = InterstitialOperators.Chartboost;
			Chartboost.showInterstitial(CBLocation.Default);
#endif
		}

        public void ShowInterstitial() {
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
			if (debuglog) Debug.Log("AdManager: ShowInterstitial! " + op.ToString());
#endif
			switch(op) {
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
				if (debuglog) Debug.LogWarning("AdManager: NOT Implemented!");
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
				if (debuglog) Debug.Log("AdManager: Chartboost Interstitial loaded. Show...");
				m_adStartTime = Time.time;
				m_LastShowedInterstitial = InterstitialOperators.Chartboost;
				Chartboost.showInterstitial(CBLocation.Default);
				return;
			}
			else {
				if (debuglog) Debug.Log("AdManager: Chartboost Interstitial not ready or skip: " + skip.ToString());
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
					if (debuglog) Debug.Log("AdManager: AdMob Interstitial loaded. Show...");
					m_adStartTime = Time.time;
					m_LastShowedInterstitial = InterstitialOperators.AdMob;
					AdMobInterstitial.Show();
					return;
				}
				else {
					if (debuglog) Debug.Log("AdManager: AdMob Interstitial not loaded");
				}
			}
*/
        }
			
#region AmazonAds
		private void ShowAmazonInterstitial() {
#if SOFTCEN_AMAZON
			if (IsAmazonInterstialReady()) {
				if (debuglog) Debug.Log("AdManager: AMAZON ShowAmazonInterstitial");
				m_adStartTime = Time.time;
				m_LastShowedInterstitial = InterstitialOperators.Amazon;
                _amazonInterstitialCounter = Math.Min(_amazonInterstitialCounter+1,100);
				amazonMobileAds.ShowInterstitialAd();
			}
#endif
        }
        private void RequestAmazonBanner() {
#if SOFTCEN_AMAZON
			if (amazonMobileAds == null)
				InitAmazonAds();
			// Create Banner
			if (debuglog) Debug.Log("AdManager: AMAZON RequestAmazonBanner");
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
			if (debuglog) Debug.Log("AdManager: AMAZON AmazonAdLoaded, adtype "+args.AdType.ToString());
            if (args.AdType == AdType.FLOATING && (GameManager.Instance.IsAdFree() || _AmazonBannerHided)) {
                HideAmazonAd();
            }
		}
		
		private void AmazonAdFailedToLoad(Ad args)
		{
			//long identifier = args.Identifier;
			
			if (debuglog) Debug.Log("AdManager: AMAZON AmazonAdFailedToLoad, adtype: " + args.AdType.ToString());
			if (args.AdType == AdType.FLOATING) {
				HideAmazonAd();
				RequestAdMobBanner();
			}
		}
		
		private void AmazonAdDismissed(Ad args) {
			//long identifier = args.Identifier;
			if (debuglog) Debug.Log("AdManager: AMAZON AmazonAdDismissed, adtype: " + args.AdType.ToString());
            if (args.AdType == AdType.INTERSTITIAL)
            {
                if (OnInterstitialStatusChanged != null)
                {
                    if (debuglog) Debug.Log("AdManager: AMAZON admanager OnInterstitialStatusChanged");
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
			if (debuglog) Debug.Log("AdManager: AMAZON RequestAmazonInterstitial");
			amazonInterstitialAd = amazonMobileAds.CreateInterstitialAd();
			if (amazonInterstitialAd.AdType == AdType.INTERSTITIAL) {
				
			}
			amazonMobileAds.LoadInterstitialAd();

		}
		
		private void InitAmazonAds() {
            if (GameManager.Instance.IsAdFree())
				return;
			
			if (debuglog) Debug.Log("AdManager: AMAZON InitAmazonAds");
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

        private void HideAmazonAd() {
#if SOFTCEN_AMAZON
			if (debuglog) Debug.Log("AdManager: AMAZON HideAmazonAd AdFree: " + GameManager.Instance.IsAdFree());
			_AmazonBannerHided = true;
			//AmazonAds.removeAds();
			if (amazonMobileAds != null && amazonBannerAd != null) {
				amazonMobileAds.CloseFloatingBannerAd(amazonBannerAd);
			}
#endif
        }

        private void LoadAmazonAd() {
#if SOFTCEN_AMAZON
			if (debuglog) Debug.Log("AdManager: AMAZON LoadAmazonAd");
            if (GameManager.Instance.IsAdFree()) {
				if (debuglog) Debug.Log("AdManager: AMAZON LoadAmazonAd ADFREE! Hide");
				HideAmazonAd();
                return;
            }
			if (amazonMobileAds != null && amazonBannerAd != null) {
				LoadingStarted response = amazonMobileAds.LoadAndShowFloatingBannerAd(amazonBannerAd);
				if (!response.BooleanValue) {
#if SOFTCEN_DEBUG
					Debug.Log("AdManager: AMAZON LoadAndShowFloatingBannerAd failed");
#endif
					HideAmazonAd();
					RequestAdMobBanner();
				}
				else {
#if SOFTCEN_DEBUG
					Debug.Log("AdManager: AMAZON LoadAndShowFloatingBannerAd started");
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
				if (debuglog) Debug.Log("AdManager: AMAZON UnSubscribeAmazonAdsEvents");
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
				if (debuglog) Debug.Log("AdManager: AMAZON SubscribeAmazonAdsEvents");
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
        void onAppLovinEventReceived(string ev) {
			if (debuglog) Debug.Log("AdManager: onAppLovinEventReceived: " + ev);
            if (ev.Contains("DISPLAYEDINTER"))
            {
                if (OnInterstitialStatusChanged != null)
                {
                    OnInterstitialStatusChanged(6);
                }
            }
            else if (ev.Contains("HIDDENINTER"))
            {
                if (OnInterstitialStatusChanged != null)
                {
                    OnInterstitialStatusChanged(7);
                }
                if (!GameManager.Instance.IsAdFree())
                    AppLovin.PreloadInterstitial();

            }
            else if (ev.Contains("REWARDAPPROVED")) {
				if (OnRewardVideoCompleted != null) {
					OnRewardVideoCompleted(m_rewardVideoId);
				}
			}
			else if (ev.Contains("REWARDREJECTED") || ev.Contains("USERCLOSEDEARLY") || ev.Contains("REWARDTIMEOUT")) {
				if (OnRewardVideoCompleted != null) {
					OnRewardVideoCompleted(-1);
				}
			}
			else if (ev.Contains("HIDDENREWARDED") && m_CurrentApplovinRewardVideoCount < m_ApplovinRewardVideoCount) {
				AppLovin.LoadRewardedInterstitial();
			}
        }
#endif

#if USE_CHARTBOOST
        void didFailToLoadInterstitial(CBLocation location, CBImpressionError error) {
#if SOFTCEN_DEBUG
			if (debuglog) AddLog(string.Format("didFailToLoadInterstitial: {0} at location {1}", error, location));
#endif
			if (OnInterstitialStatusChanged != null)
            {
                OnInterstitialStatusChanged(3);
            }
        }

        void didDismissInterstitial(CBLocation location) {
#if SOFTCEN_DEBUG
			if (debuglog) AddLog("didDismissInterstitial: " + location);
#endif
		}
		
		void didCloseInterstitial(CBLocation location) {
#if SOFTCEN_DEBUG
			if (debuglog) AddLog("didCloseInterstitial: " + location);
#endif
			if (OnInterstitialStatusChanged != null)
            {
                OnInterstitialStatusChanged(3);
            }

        }

        void didClickInterstitial(CBLocation location) {
#if SOFTCEN_DEBUG
	if (debuglog) AddLog("didClickInterstitial: " + location);
#endif
		}
		
		void didCacheInterstitial(CBLocation location) {
#if SOFTCEN_DEBUG
            if (debuglog) AddLog("didCacheInterstitial: " + location);
#endif
		}
		
		bool shouldDisplayInterstitial(CBLocation location) {
			// return true if you want to allow the interstitial to be displayed
			bool showInterstitial = true;
			if (GameManager.Instance.IsAdFree ())
				showInterstitial = false;
#if SOFTCEN_DEBUG
	 if (debuglog) AddLog("shouldDisplayInterstitial @" + location + " : " + showInterstitial);
#endif
			return showInterstitial;
		}

#if !UNITY_ANDROID
        void didDisplayInterstitial(CBLocation location)
        {
#if SOFTCEN_DEBUG
            if (debuglog) AddLog("didDisplayInterstitial: " + location);
#endif
            if (OnInterstitialStatusChanged != null)
            {
                OnInterstitialStatusChanged(4);
            }

        }


  		void didDisplayRewardedVideo(CBLocation location){
#if SOFTCEN_DEBUG
            if (debuglog) AddLog("didDisplayRewardedVideo: " + location);
#endif
		}

#endif

		void didFailToRecordClick(CBLocation location, CBClickError error) {
#if SOFTCEN_DEBUG
            if (debuglog) AddLog(string.Format("didFailToRecordClick: {0} at location: {1}", error, location));
#endif
		}
		
		void didFailToLoadRewardedVideo(CBLocation location, CBImpressionError error) {
#if SOFTCEN_DEBUG
            if (debuglog) AddLog(string.Format("didFailToLoadRewardedVideo: {0} at location {1}", error, location));
#endif
		}
		
		void didDismissRewardedVideo(CBLocation location) {
#if SOFTCEN_DEBUG
            if (debuglog) AddLog("didDismissRewardedVideo: " + location);
#endif
		}
		
		void didCloseRewardedVideo(CBLocation location) {
#if SOFTCEN_DEBUG
            if (debuglog) AddLog("didCloseRewardedVideo: " + location);
#endif
		}
		
		void didClickRewardedVideo(CBLocation location) {
#if SOFTCEN_DEBUG
            if (debuglog) AddLog("didClickRewardedVideo: " + location);
#endif
		}
		
		void didCacheRewardedVideo(CBLocation location) {
#if SOFTCEN_DEBUG
            if (debuglog) AddLog("didCacheRewardedVideo: " + location);
#endif
		}
		
		bool shouldDisplayRewardedVideo(CBLocation location) {
			bool showRewardedVideo = useRewardVideo;
#if SOFTCEN_DEBUG
            if (debuglog) AddLog("shouldDisplayRewardedVideo @" + location + " : " + showRewardedVideo);
#endif
			return showRewardedVideo;
		}
		
		void didCompleteRewardedVideo(CBLocation location, int reward) {
#if SOFTCEN_DEBUG
            if (debuglog) AddLog(string.Format("didCompleteRewardedVideo: reward {0} at location {1}", reward, location));
#endif
			RewardVideoFinished();
			Chartboost.cacheRewardedVideo(CBLocation.Default);
		}
		
		
		void didCacheInPlay(CBLocation location) {
#if SOFTCEN_DEBUG
            if (debuglog) AddLog("didCacheInPlay called: "+location);
#endif
		}
		
		void didFailToLoadInPlay(CBLocation location, CBImpressionError error) {
#if SOFTCEN_DEBUG
            if (debuglog) AddLog(string.Format("didFailToLoadInPlay: {0} at location: {1}", error, location));
#endif
		}
		
		void didPauseClickForConfirmation() {
#if SOFTCEN_DEBUG
            if (debuglog) AddLog("didPauseClickForConfirmation called");
#endif
			//activeAgeGate = true;
		}
		
		void willDisplayVideo(CBLocation location) {
#if SOFTCEN_DEBUG
            if (debuglog) AddLog("willDisplayVideo: " + location);
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
			Debug.Log("AdManager: CB " + text);
#endif
		}

	}

}
#endif
