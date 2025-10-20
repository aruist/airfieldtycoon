using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_IOS || UNITY_IPHONE
using UnityEngine.SocialPlatforms;
#endif


public class SocialManager : MonoBehaviour {
#if !NO_SOCIALMANAGER
	public bool ConnectInStart = true;
	public static SocialManager Instance;

	public event Action onSocialStateChanged;

	// Google
	public GameObject goGooglePlayServices;
	public GameObject goGoogleLeaderboards;
	public GameObject goGoogleAchievements;
	public GameObject goGoolgeSignOut;
	// Amazon
	public GameObject goAmazonSignIn;
	public GameObject goAmazonLeaderboards;
	public GameObject goAmazonAchievements;
	// iOS
	public GameObject goIOSLeaderboards;
	public GameObject goIOSAchievements;

	// Social buttons
	public GameObject[] goFBButton;
	public GameObject[] goTwitterButton;


	private bool m_Authenticating = false;
	#if !UNITY_WP8
	private Dictionary<string,int> mLeaderBoard = new Dictionary<string, int>();
	#endif

	private int m_AutoSignIn;

	// Achievements:
	// list of achievements we know we have unlocked (to avoid making repeated calls to the API)
	private Dictionary<string,bool> mUnlockedAchievements = new Dictionary<string, bool>();
	//private Dictionary<string,bool> mUnlockedAchievementsDone = new Dictionary<string, bool>();
	// achievement increments we are accumulating locally, waiting to send to the games API
	//private Dictionary<string,int> mPendingIncrements = new Dictionary<string, int>();

	void Awake () {
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
		else {
			Destroy(this.gameObject);
		}
	}

	void Start() {
#if SOFTCEN_DEBUG && SOFTCEN_AMAZON
		Debug.Log("SocialManager Amazon Version");
#elif SOFTCEN_DEBUG && UNITY_ANDROID
		Debug.Log("SocialManager Google Version");
#endif
		SubscribeEvents();
		#if UNITY_IPHONE 
		for (int i = 0; i < goFBButton.Length; i++) {
			goFBButton[i].SetActive(false);
		}
		for (int i = 0; i < goTwitterButton.Length; i++) {
			goTwitterButton[i].SetActive(false);
		}

		Authenticate();
#else
		if (ConnectInStart) {
	 		if (PlayerPrefs.HasKey("AutoSignIn")) {
				m_AutoSignIn = PlayerPrefs.GetInt("AutoSignIn");
			}
			else {
				m_AutoSignIn = 1;
			}

			if (m_AutoSignIn == 1) {
				Authenticate();
			}
		}
#endif
		UpdateSocialUI();
	}

    void Update()
    {
        if (_UpdateSocialUI)
        {
            UpdateSocialUI();
        }
    }

    void OnDestroy() {
		UnSubscribeEvents();
	}

	private void UnSubscribeEvents() {
		#if SOFTCEN_AMAZON
		AGSClient.ServiceReadyEvent -= HandleAmazonServiceReadyEvent;
		AGSClient.ServiceNotReadyEvent -= HandleAmazonServiceNotReadyEvent;
		#endif

	}

	private void SubscribeEvents() {
		#if SOFTCEN_AMAZON
		AGSClient.ServiceReadyEvent += HandleAmazonServiceReadyEvent;
		AGSClient.ServiceNotReadyEvent += HandleAmazonServiceNotReadyEvent;
		#endif
	}

	#if SOFTCEN_AMAZON
	void HandleAmazonServiceNotReadyEvent (string obj)
	{
		m_Authenticating = false;
        _UpdateSocialUI = true;
	}

	void HandleAmazonServiceReadyEvent ()
	{
		m_Authenticating = false;
		#if SOFTCEN_DEBUG
		Debug.Log("HandleAmazonServiceReadyEvent IsServiceReady: " + AGSClient.IsServiceReady().ToString());
		#endif
		if (AGSClient.IsServiceReady()) {

		}
		AGSClient.SetPopUpLocation(GameCirclePopupLocation.TOP_CENTER);
        _UpdateSocialUI = true;
	}
	#endif

	void OnApplicationPause(bool pauseStatus) {
		//Debug.Log("SocialManager OnApplicationPause " + pauseStatus);
		if (!pauseStatus) {
            _UpdateSocialUI = true;
		}
	}

    private bool _UpdateSocialUI = false;
	public void UpdateSocialUI() {
        _UpdateSocialUI = false;
        GooglePlayUI();
		AmazonGameCircleUI();
		IOSGameCenterUI();
	}

	public void ShowLeaderboard() {
        /*
		AudioManager.Instance.PlayClip(AudioManager.AudioSX.Button);
		if (Authenticated) {
			#if UNITY_ANDROID && !SOFTCEN_AMAZON
			PlayGamesPlatform.Instance.ShowLeaderboardUI();
			//PlayGamesPlatform.Instance.ShowLeaderboardUI(GameConsts.LeaderBoards.TOPSCORES);
			#else
			Social.ShowLeaderboardUI();
			#endif
		}
		UpdateSocialUI();
        */
	}

	public void ShowAchievements() {
        /*
		AudioManager.Instance.PlayClip(AudioManager.AudioSX.Button);
		if (Authenticated) {
			Social.ShowAchievementsUI();
		}
		UpdateSocialUI();
        */
	}

	public void SocialLogIn() {
        /*
		AudioManager.Instance.PlayClip(AudioManager.AudioSX.Button);
		Authenticate();
		UpdateSocialUI();
        */
	}

	public void SocialLogOut() {
        /*
		#if UNITY_ANDROID
		if (Authenticated) {
			((PlayGamesPlatform) Social.Active).SignOut();
			PlayerPrefs.SetInt("AutoSignIn", 0);
			PlayerPrefs.Save();
		}
		#endif
		UpdateSocialUI();
		if (onSocialStateChanged != null) {
			onSocialStateChanged();
		}
        */
	}

	public bool Authenticated {
		get {
            /*
    #if SOFTCEN_AMAZON
                return AGSClient.IsServiceReady();
    #else
                return Social.Active.localUser.authenticated; 
    #endif
    */
            return false; 
		}
	}
	
	public bool Authenticating {
		get {
			return m_Authenticating;
		}
	}

	public void Authenticate() {
        /*
		if (Authenticated || Authenticating) {
			#if SOFTCEN_DEBUG
			Debug.LogWarning("Ignoring repeated call to Authenticate().");
			#endif
			return;
		}
		m_Authenticating = false;

		#if UNITY_ANDROID && !SOFTCEN_AMAZON
		AuthenticateGooglePlay();
		#endif
		#if UNITY_ANDROID && SOFTCEN_AMAZON
		AuthenticateAmazonGameCircle();
		#endif
		#if UNITY_IOS || UNITY_IPHONE
		AuthenticateAppStore();
		#endif
        */
	}

#if UNITY_ANDROID
	private void AuthenticateGooglePlay() {
        /*
		m_Authenticating = true;
		#if SOFTCEN_DEBUG
		// Enable/disable logs on the PlayGamesPlatform
		PlayGamesPlatform.DebugLogEnabled = false;
		#endif
		// Activate the Play Games platform. This will make it the default
		// implementation of Social.Active
		//PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
		//	.Build();
		//PlayGamesPlatform.InitializeInstance(config);
		PlayGamesPlatform.Activate();
		// Set the default leaderboard for the leaderboards UI
		//((PlayGamesPlatform) Social.Active).SetDefaultLeaderboardForUI(GameIds.LeaderboardId);
		Social.localUser.Authenticate((bool success) => {
			m_Authenticating = false;
			if (onSocialStateChanged != null) {
				onSocialStateChanged();
			}
			if (success) {
				PlayerPrefs.SetInt("AutoSignIn", 1);
				PlayerPrefs.Save();
				UpdateSocialUI();
				if (onSocialStateChanged != null) {
					onSocialStateChanged();
				}
			} else {
				// no need to show error message (error messages are shown automatically
				// by plugin)
				#if SOFTCEN_DEBUG
				Debug.LogWarning("Failed to sign in Social services.");
				#endif
				UpdateSocialUI();
				PlayerPrefs.SetInt("AutoSignIn", 0);
				PlayerPrefs.Save();
			}
		});
        */
	}
#endif

	private void AuthenticateAmazonGameCircle() {
		m_Authenticating = true;
#if SOFTCEN_AMAZON
		Social.Active = GameCircleSocial.Instance;
		AGSClient.Init(true, true, false);
#endif
	}

	private void AuthenticateAppStore() {
		m_Authenticating = true;
		#if SOFTCEN_DEBUG
		Debug.Log ("Authenticate IOS Game Center");
		#endif
		Social.localUser.Authenticate((bool success) => {
			m_Authenticating = false;
			if (onSocialStateChanged != null) {
				onSocialStateChanged();
			}
			if (success) {
                _UpdateSocialUI = true;
			} else {
#if SOFTCEN_DEBUG
				Debug.LogWarning("Failed to sign in Social services.");
#endif
                _UpdateSocialUI = true;
			}
		});

	}

	public void PostToLeaderboard(string lbId, int score )
	{
		if (!mLeaderBoard.ContainsKey(lbId))
		{
			mLeaderBoard[lbId] = 0;
		}
		if (Authenticated && score > mLeaderBoard[lbId]) {
			mLeaderBoard[lbId] = score;
			#if SOFTCEN_AMAZON
			AGSLeaderboardsClient.SubmitScore(lbId, score);
			#else
			Social.ReportScore(score, lbId, (bool success) => {});
			#endif
		}
	}
	
	public void PostToLeaderboard() {
		/*
		int score = mProgress.TotalScore;
		if (Authenticated && score > mHighestPostedScore) {
			// post score to the leaderboard
			Social.ReportScore(score, GameIds.LeaderboardId, (bool success) => {});
			mHighestPostedScore = score;
		}*/
	}

	public void UnlockAchievement(string achId) {
		if (Authenticated && !mUnlockedAchievements.ContainsKey(achId)) {
#if SOFTCEN_AMAZON
			AGSAchievementsClient.UpdateAchievementProgress(achId, 100.0f);
#else
			Social.ReportProgress(achId, 100.0f, (bool success) => {
				// handle success of failure
				if (success)
					mUnlockedAchievements[achId] = true;
			});
#endif
		}
	}

	public void IncrementAchievement(string achId, float progress) {
		#if SOFTCEN_DEBUG
		Debug.Log("IncrementAchievement " + achId + " = " + progress);
		#endif
		#if SOFTCEN_AMAZON
		if (Authenticated && !mUnlockedAchievements.ContainsKey(achId)) {
			AGSAchievementsClient.UpdateAchievementProgress(achId, progress);
		}
		#elif UNITY_ANDROID && !SOFTCEN_AMAZON
        /*
		if (Authenticated) {
			PlayGamesPlatform.Instance.IncrementAchievement(achId, 1, (bool success) => {
				// handle success of failure
			});
		}*/
		#elif UNITY_IOS || UNITY_IPHONE
		if (Authenticated && !mUnlockedAchievements.ContainsKey(achId)) {
			if (progress >= 100f) {
				Social.ReportProgress(achId, 100.0f, (bool success) => { });
			} else {
				Social.ReportProgress(achId, progress, (bool success) => { });
			}
		}
		#endif
		if (Authenticated && !mUnlockedAchievements.ContainsKey(achId) && progress >= 100f) {
			mUnlockedAchievements[achId] = true;
		}
	}

	private void AmazonGameCircleUI() {
#if SOFTCEN_AMAZON
		if (!Authenticated) {
			SetAmazonSignIn(false);
			SetAmazonLeaderboards(false);
			SetAmazonAchievements(false);
			if (Authenticating) {
				SetAmazonSignIn(false);
			}
			else {
				SetAmazonSignIn(true);
			}
		}
		else {
			SetAmazonSignIn(false);
			SetAmazonLeaderboards(true);
			SetAmazonAchievements(true);
		}
#else
		SetAmazonSignIn(false);
		SetAmazonLeaderboards(false);
		SetAmazonAchievements(false);
#endif
	}

	private void GooglePlayUI() {
		#if UNITY_ANDROID && !SOFTCEN_AMAZON
		if (!Authenticated) {
			// Not authenticated
			SetGoogleLeaderboards(false);
			SetGoogleAchievements(false);
			SetGoogleSignOut(false);
			if (Authenticating) {
				// Connecting
				SetGooglePlayServices(false);
			}
			else {
				SetGooglePlayServices(true);
			}
		}
		else {
			// Logged in
			SetGooglePlayServices(false);
			SetGoogleLeaderboards(true);
			SetGoogleAchievements(true);
			SetGoogleSignOut(true);
		}
		#else
		SetGooglePlayServices(false);
		SetGoogleLeaderboards(false);
		SetGoogleAchievements(false);
		SetGoogleSignOut(false);
		#endif
	}

	private void SetGooglePlayServices(bool value) {
		if (goGooglePlayServices != null)
			goGooglePlayServices.SetActive(value);
	}
	private void SetGoogleSignOut(bool value) {
		if (goGoolgeSignOut != null)
			goGoolgeSignOut.SetActive(value);
	}
	private void SetGoogleAchievements(bool value) {
		if (goGoogleAchievements != null)
			goGoogleAchievements.SetActive(value);
	}
	private void SetGoogleLeaderboards(bool value) {
		if (goGoogleLeaderboards != null)
			goGoogleLeaderboards.SetActive(value);
	}

	private void SetAmazonSignIn(bool value) {
		if (goAmazonSignIn != null)
			goAmazonSignIn.SetActive(value);
	}
	private void SetAmazonLeaderboards(bool value) {
		if (goAmazonLeaderboards != null)
			goAmazonLeaderboards.SetActive(value);
	}
	private void SetAmazonAchievements(bool value) {
		if (goAmazonAchievements != null)
			goAmazonAchievements.SetActive(value);
	}
	private void SetIOSLeaderboards(bool value) {
		if (goIOSLeaderboards != null)
			goIOSLeaderboards.SetActive(value);
	}
	private void SetIOSAchievements(bool value) {
		if (goIOSAchievements != null)
			goIOSAchievements.SetActive(value);
	}

	private void IOSGameCenterUI() {
#if UNITY_IPHONE
		if (!Authenticated) {
			SetIOSAchievements(false);
			SetIOSLeaderboards(false);
		}
		else {
			SetIOSAchievements(true);
			SetIOSLeaderboards(true);
		}
#else
		SetIOSAchievements(false);
		SetIOSLeaderboards(false);
#endif
	}

	#region Rate Games and More Games
	public void MoreGames() {
		AudioManager.Instance.PlayClip(AudioManager.AudioSX.Button);
		if (!string.IsNullOrEmpty(GameConsts.Links.MoreGames)) {
			Application.OpenURL(GameConsts.Links.MoreGames);
		}
		//Application.OpenURL("market://search?q=softcen");
	}

	public void RateGame() {
		AudioManager.Instance.PlayClip(AudioManager.AudioSX.Button);
		if (!string.IsNullOrEmpty(GameConsts.Links.RateGame)) {
			Application.OpenURL(GameConsts.Links.RateGame);
		}

	}
	#endregion

	public void OpenFacebookLink() {
		Application.OpenURL("http://www.facebook.com/softcen");
	}

	public void OpenTwitterLink() {
		Application.OpenURL("http://twitter.com/softcen");
	}

	public void OpenFacebookCompanyPage() {
		if(checkPackageAppIsPresent("com.facebook.katana"))
		{
			//Debug.Log("com.facebook.katana found");
			Application.OpenURL("fb://page/435248659944994"); //there is Facebook app installed so let's use it
		}
		else
		{
			//Debug.Log("com.facebook.katana NOT found");
			string fbpage = "https://www.facebook.com/435248659944994";
			Application.OpenURL(fbpage);
		}

	}

	public void OpenTwitterCompanyPage() {
		string twitterpage = "https://www.twitter.com/softcen";
		Application.OpenURL(twitterpage);
	}

	public bool checkPackageAppIsPresent(string package)
	{
		#if UNITY_ANDROID
		AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");
		
		//take the list of all packages on the device
		AndroidJavaObject appList = packageManager.Call<AndroidJavaObject>("getInstalledPackages",0);
		int num = appList.Call<int>("size");
		for(int i = 0; i < num; i++)
		{
			AndroidJavaObject appInfo = appList.Call<AndroidJavaObject>("get", i);
			string packageNew = appInfo.Get<string>("packageName");
			if(packageNew.CompareTo(package) == 0)
			{
				return true;
			}
		}
		#endif
		return false;
	}

#endif
}
